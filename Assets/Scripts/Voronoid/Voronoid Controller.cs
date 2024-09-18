using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class VoronoidController : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private List<Vector3> nodes = new List<Vector3>();
    private List<Vector3> mines = new List<Vector3>();
    Vector3 mid;

    List<Vector3> vertex = new List<Vector3>();
    List<Side> sides = new List<Side>();

    Side side1;
    Side side2;
    Side side3;
    void Start()
    {
        sides.Add(new Side(new Vector3(9, 0, 9), new Vector3(9, 0, 0)));
        sides.Add(new Side(new Vector3(9, 0, 9), new Vector3(0, 0, 9)));
        sides.Add(new Side(new Vector3(0, 0, 0), new Vector3(0, 0, 9)));
        sides.Add(new Side(new Vector3(0, 0, 0), new Vector3(9, 0, 0)));

        vertex.Add(new Vector3(9, 0, 9));
        vertex.Add(new Vector3(0, 0, 0));
        vertex.Add(new Vector3(0, 0, 9));
        vertex.Add(new Vector3(9, 0, 0));

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Vector3Int pos = new Vector3Int(i, 0, j);

                nodes.Add(pos);
            }
        }

        mines.Add(nodes[Random.Range(0, nodes.Count)]);
        mines.Add(nodes[Random.Range(0, nodes.Count)]);
        //mines.Add(nodes[Random.Range(0, nodes.Count)]);

        side1 = GetSide(mines[0], mines[1]);
        //side2 = GetSide(mines[1], mines[0]);

        GetVertex(side1);
        GetVertex(side2);
    }

    public Side GetSide(Vector3 A, Vector3 B)
    {
        Vector3 mid = new Vector3((A.x + B.x) / 2, (A.y + B.y) / 2, (A.z + B.z) / 2);

        Vector3 connector = A - B;

        Vector3 direction = new Vector3(-connector.z, 0, connector.x).normalized;

        return new Side(mid - direction * 20 / 2, mid + direction * 20 / 2);
    }

    public void GetVertex(Side side)
    {
        for (int i = 0; i < sides.Count; i++)
        {
            Vector3 aux;

            if (DetectarInterseccion(side, sides[i], out aux))
            {
                vertex.Add(aux);
            }
        }
    }

    public static bool DetectarInterseccion(Side side1, Side side2, out Vector3 puntoInterseccion)
    {
        puntoInterseccion = new Vector3();

        float denominator = (side1.start.x - side1.end.x) * (side2.start.z - side2.end.z) - 
            (side1.start.z - side1.end.z) * (side2.start.x - side2.end.x);

        if (denominator != 0)
        {
            float t = ((side1.start.x - side2.start.x) * (side2.start.z - side2.end.z) - (side1.start.z - side2.start.z) * (side2.start.x - side2.end.x)) / denominator;
            float u = ((side1.start.x - side2.start.x) * (side1.start.z - side1.end.z) - (side1.start.z - side2.start.z) * (side1.start.x - side1.end.x)) / denominator;

            if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                puntoInterseccion = new Vector3(side1.start.x + t * (side1.end.x - side1.start.x), 0 ,side1.start.z + t * (side1.end.z - side1.start.z));
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = UnityEngine.Color.green;

        for (int i = 0; i < mines.Count; i++)
        {
            Gizmos.DrawCube(mines[i], Vector3.one);
        }

        Gizmos.color = UnityEngine.Color.magenta;

        Gizmos.DrawLine(side1.start, side1.end);
        Gizmos.DrawLine(side2.start, side2.end);
        Gizmos.DrawLine(side3.start, side3.end);

        for (int i = 0; i < sides.Count; i++)
        {
            Gizmos.DrawLine(sides[i].start, sides[i].end);
        }

        Gizmos.color = UnityEngine.Color.cyan;
        Gizmos.DrawSphere(mid, 0.125f);

        Gizmos.color = UnityEngine.Color.yellow;

        for (int i = 0;i < vertex.Count; i++)
        {
            Gizmos.DrawSphere(vertex[i], 0.25f);
        }
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

public struct Poligon
{
    public List<Vector3> vertices;

    public Poligon(List<Vector3> vertices)
    {
        this.vertices = vertices;
    }
}