using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Vector2IntGrapf<Node<Vector2Int>> grapf;
    public Node<Vector2Int> urbanCenter;

    [Header("For Caravanas")]
    public int foodForCaravana;
    public List<Caravana> caravanas;

    [Header("For Miners")]
    public List<Mine<Vector2Int>> mines;
    public List<Traveler> travelers;

    [Header("Variables")]
    [SerializeField] private TypeOfPathFinder typeOfPathFinder;
    public GameObject prefab;
    public GameObject prefab2;
    public int OffsetPublic;

    [Header("Menu")]
    [SerializeField] private GameObject panel;
    [SerializeField] private InputField inputX;
    [SerializeField] private InputField inputY;
    [SerializeField] private InputField Offset;
    [SerializeField] private InputField minesCount;
    [SerializeField] private Button play;

    public Action<int, int> ChangePathFinder;
    private bool isRunning = false;

    public TypeOfPathFinder GetTypeOfPathFinder { get => typeOfPathFinder; }

    private void Start()
    {
        play.onClick.AddListener(OnPlaySimulation);
        mines = new List<Mine<Vector2Int>>();
        caravanas = new List<Caravana>();
        travelers = new List<Traveler>();
    }

    private void OnPlaySimulation()
    {
        OffsetPublic = int.Parse(Offset.text);

        panel.SetActive(false);
        grapf = new Vector2IntGrapf<Node<Vector2Int>>(int.Parse(inputX.text), int.Parse(inputY.text), 
            int.Parse(Offset.text), typeOfPathFinder);


        Vector3 newCamPos = new Vector3(int.Parse(inputX.text) * int.Parse(Offset.text) / 2, 
            int.Parse(inputY.text) * int.Parse(Offset.text) / 2, 
            (int.Parse(Offset.text) * ((int.Parse(inputY.text) + int.Parse(inputX.text)) / 2)) * -1);

        Camera.main.transform.position = newCamPos;


        urbanCenter = grapf.nodes[UnityEngine.Random.Range(0, grapf.nodes.Count)];
        Instantiate(prefab2,
                       new Vector3(urbanCenter.GetCoordinate().x * OffsetPublic, urbanCenter.GetCoordinate().y * OffsetPublic, 0),
                       Quaternion.identity);

        for (int i = 0; i < int.Parse(minesCount.text); i++)
        {
            bool isUsed = true;
            Mine<Vector2Int> tempNode = new Mine<Vector2Int>(new Node<Vector2Int>(), 0, 0);

            while (isUsed)
            {
                tempNode = new Mine<Vector2Int>(grapf.nodes[UnityEngine.Random.Range(0, grapf.nodes.Count)], 7, 0);

                if(!tempNode.Equals(urbanCenter) && !mines.Contains(tempNode))
                {
                    isUsed = false;
                    Instantiate(prefab, 
                        new Vector3(tempNode.currentNode.GetCoordinate().x * OffsetPublic, tempNode.currentNode.GetCoordinate().y * OffsetPublic, 0), 
                        Quaternion.identity);
                }
            }

            mines.Add(tempNode);
        }

        ChangePathFinder?.Invoke(UnityEngine.Random.Range(0, grapf.nodes.Count), UnityEngine.Random.Range(0, mines.Count));
        isRunning = true;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !isRunning)
            return;

        foreach (Node<Vector2Int> node in grapf.nodes)
        {
            if (node == null)
                return;

            Vector3 nodeCordinates = new Vector3(OffsetPublic * node.GetCoordinate().x, OffsetPublic * node.GetCoordinate().y);
            foreach (INode<Vector2Int> neighbor in node.GetNeighbors())
            {
                Gizmos.color = Color.white;
                Vector2Int neighborCordinates = neighbor.GetCoordinate();
                Gizmos.DrawLine(nodeCordinates, new Vector3(OffsetPublic * neighborCordinates.x, OffsetPublic * neighborCordinates.y));
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

    public Mine(Node<Coordinates> startNode, float startGold, int startFood)
    {
        currentNode = startNode;
        currentGold = startGold;
        currentFood = startFood;
        miners = new List<Traveler>();
    }
}