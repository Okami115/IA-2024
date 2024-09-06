using System;
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

    public List<Node<Vector2Int>> mines;

    public Node<Vector2Int> urbanCenter;

    [SerializeField] private TypeOfPathFinder typeOfPathFinder;

    [SerializeField] private GameObject panel;

    [SerializeField] private InputField inputX;
    [SerializeField] private InputField inputY;
    [SerializeField] private InputField Offset;
    [SerializeField] private InputField minesCount;

    [SerializeField] private Button play;

    public int OffsetPublic;

    public Action<int, int> ChangePathFinder;
    private bool isRunning = false;

    public TypeOfPathFinder GetTypeOfPathFinder { get => typeOfPathFinder; }

    private void Start()
    {
        play.onClick.AddListener(OnPlaySimulation);
        mines = new List<Node<Vector2Int>>();
    }

    private void OnPlaySimulation()
    {
        OffsetPublic = int.Parse(Offset.text);

        panel.SetActive(false);
        grapf = new Vector2IntGrapf<Node<Vector2Int>>(int.Parse(inputX.text), int.Parse(inputY.text), 
            int.Parse(Offset.text), typeOfPathFinder);

        for (int i = 0; i < int.Parse(minesCount.text); i++)
        {
            mines.Add(grapf.nodes[UnityEngine.Random.Range(0, grapf.nodes.Count)]);
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
