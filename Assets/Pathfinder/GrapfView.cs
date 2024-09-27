using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[Serializable]
public enum TypeOfPathFinder
{
    BreadthPathfinder,
    DepthFirstPathfinder,
    DijstraPathfinder,
    AStarPathfinder
}

public class GrapfView : MonoBehaviour
{
    public Vector3Grapf<Node<Vector3>> grapf;
    public Node<Vector3> urbanCenter;
    public VoronoidController Vcontroller;
    public List<Vector3> vertex = new List<Vector3>();

    [Header("For Caravanas")]
    public int foodForCaravana;
    public List<Caravana> caravanas;

    [Header("For Miners")]
    public int currentGold;
    public List<Mine<Vector3>> mines;
    public List<Traveler> travelers;

    [Header("Variables")]
    [SerializeField] private TypeOfPathFinder typeOfPathFinder;
    public GameObject prefab;
    public GameObject prefab2;
    public int OffsetPublic;
    public int GridX;
    public int GridY;
    public bool isAlert;

    [Header("UI")]
    [SerializeField] private GameObject ui;
    [SerializeField] private UnityEngine.UI.Button Alarm;


    [Header("Menu")]
    [SerializeField] private GameObject panel;
    [SerializeField] private InputField inputX;
    [SerializeField] private InputField inputY;
    [SerializeField] private InputField Offset;
    [SerializeField] private InputField minesCount;
    [SerializeField] private UnityEngine.UI.Button play;

    public Action<int, int> ChangePathFinder;
    public Action start;
    private bool isRunning = false;

    public TypeOfPathFinder GetTypeOfPathFinder { get => typeOfPathFinder; }

    private void Start()
    {
        Vcontroller = new VoronoidController(this);
        play.onClick.AddListener(OnPlaySimulation);
        Alarm.onClick.AddListener(OnAlert);
        mines = new List<Mine<Vector3>>();
        caravanas = new List<Caravana>();
        travelers = new List<Traveler>();
        isAlert = false;
    }

    private void OnPlaySimulation()
    {
        OffsetPublic = int.Parse(Offset.text);
        GridX = int.Parse(inputX.text) * OffsetPublic;
        GridY = int.Parse(inputY.text) * OffsetPublic;

        vertex.Add(new Vector3(GridX, 0, GridY));
        vertex.Add(new Vector3(GridX, 0, -1));
        vertex.Add(new Vector3(-1, 0, -1));
        vertex.Add(new Vector3(-1, 0, GridY));

        panel.SetActive(false);
        ui.SetActive(true);
        grapf = new Vector3Grapf<Node<Vector3>>(GridX, GridY,
            OffsetPublic, typeOfPathFinder);


        Vector3 newCamPos = new Vector3(GridX * OffsetPublic / 2, GridY * OffsetPublic / 2, OffsetPublic * (GridY + GridX / 2) * -1);

        Camera.main.transform.position = newCamPos;


        urbanCenter = grapf.nodes[UnityEngine.Random.Range(0, grapf.nodes.Count)];

        for (int i = 0; i < int.Parse(minesCount.text); i++)
        {
            bool isUsed = true;
            Mine<Vector3> tempNode = new Mine<Vector3>(new Node<Vector3>(), 0, 0, new Poligon(vertex));

            while (isUsed)
            {
                tempNode = new Mine<Vector3>(grapf.nodes[UnityEngine.Random.Range(0, grapf.nodes.Count)], 7, 0, new Poligon(vertex));

                if (!tempNode.Equals(urbanCenter) && !mines.Contains(tempNode))
                {
                    isUsed = false;
                }
            }

            mines.Add(tempNode);
        }

        ChangePathFinder?.Invoke(UnityEngine.Random.Range(0, grapf.nodes.Count), UnityEngine.Random.Range(0, mines.Count));
        start?.Invoke();
        Vcontroller.RunVoronoid();
        isRunning = true;
    }

    private void OnAlert()
    {
        isAlert = !isAlert; 
    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !isRunning)
            return;

        foreach (Node<Vector3> node in grapf.nodes)
        {
            if (node == null)
                return;

            Vector3 nodeCordinates = new Vector3(OffsetPublic * node.GetCoordinate().x, OffsetPublic * node.GetCoordinate().y, OffsetPublic * node.GetCoordinate().z);
            foreach (INode<Vector3> neighbor in node.GetNeighbors())
            {
                Gizmos.color = Color.white;
                Vector3 neighborCordinates = neighbor.GetCoordinate();
                Gizmos.DrawLine(nodeCordinates, new Vector3(OffsetPublic * neighborCordinates.x, OffsetPublic * neighborCordinates.y, OffsetPublic * neighborCordinates.z));
            }
        }

        Gizmos.color = UnityEngine.Color.magenta;
        for (int i = 0; i < vertex.Count; i++)
        {
            Vector3 current = new Vector3(vertex[i].x, vertex[i].y, vertex[i].z);
            Vector3 next = new Vector3(vertex[(i + 1) % vertex.Count].x, vertex[(i + 1) % vertex.Count].y, vertex[(i + 1) % vertex.Count].z);
            Gizmos.DrawLine(current, next);
        }

        for (int i = 0; i < mines.Count; i++)
        {
            Gizmos.color = mines[i].color;

            Gizmos.DrawCube(mines[i].currentNode.GetCoordinate(), Vector3.one / 2);
        }

        for (int i = 0; i < mines.Count; i++)
        {
            Gizmos.color = mines[i].color;
            Handles.color = mines[i].color;

            Handles.DrawAAConvexPolygon(mines[i].poligon.vertices.ToArray());
        }

        Gizmos.color = UnityEngine.Color.black;
        for (int j = 0; j < mines.Count; j++)
        {
            for (int i = 0; i < mines[j].poligon.vertices.Count; i++)
            {
                Vector3 vertex1 = mines[j].poligon.vertices[i];
                Vector3 vertex2 = mines[j].poligon.vertices[(i + 1) % mines[j].poligon.vertices.Count];

                Gizmos.DrawLine(vertex1, vertex2);
            }
        }
    }
}

[Serializable]
public class Mine<Coordinates>
    where Coordinates : IEquatable<Coordinates>
{
    public Node<Coordinates> currentNode;
    public float currentGold;
    public int currentFood;
    public List<Traveler> miners;
    public Poligon poligon;
    public Color color;

    public Mine(Node<Coordinates> startNode, float startGold, int startFood, Poligon poligon)
    {
        currentNode = startNode;
        currentGold = startGold;
        currentFood = startFood;
        miners = new List<Traveler>();
        this.poligon = poligon;
        color = new UnityEngine.Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 0.25f);
    }
}

public struct Side
{
    public Vector3 start;
    public Vector3 end;

    public Side(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }
}

[Serializable]
public struct Poligon
{
    public List<Vector3> vertices;

    public Poligon(List<Vector3> vertices)
    {
        this.vertices = vertices;
    }
}