using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

/*
public class PolygonCutter
{
    public void CutPolygon(Mine<Vector2Int> mine, Side cut)
    {
        List<Vector3> polygon1 = new List<Vector3>();
        List<Vector3> polygon2 = new List<Vector3>();

        List<Vector3> intersections = new List<Vector3>();

        bool isFirstPolygon = true;


        for (int i = 0; i < mine.poligon.vertices.Count; i++)
        {
            Vector3 p1 = mine.poligon.vertices[i];
            Vector3 p2 = mine.poligon.vertices[(i + 1) % mine.poligon.vertices.Count];

            if (isFirstPolygon)
            {
                polygon1.Add(p1);
            }
            else
            {
                polygon2.Add(p1);
            }

            Side side1 = new Side(p1, p2);
            Side side2 = new Side(cut.start, cut.end);

            if (DetectarInterseccion(side1, side2, out Vector3 intersection))
            {
                if (!intersections.Contains(intersection) && intersections.Count < 2)
                {
                    intersections.Add(intersection);

                    polygon1.Add(intersection);
                    polygon2.Add(intersection);

                    isFirstPolygon = !isFirstPolygon;
                }
            }
        }

        if (intersections.Count == 2)
        {

            if (IsPointInPolygon(mine.currentNode.coordinate, new Poligon(polygon1)))
            {

                mine.poligon = new Poligon(polygon1);
            }
            else if (IsPointInPolygon(mine.currentNode.coordinate, new Poligon(polygon2)))
            {
                mine.poligon = new Poligon(polygon2);
            }
            else
            {
                Debug.LogError("POR FAVOR NO SALTES");
            }
        }
        else
        {
            Debug.LogWarning($"No se encontraron intersecciones o el polígono no fue cortado correctamente. {intersections.Count}");
        }
    }

    public static bool DetectarInterseccion(Side side1, Side side2, out Vector3 puntoInterseccion)
    {
        puntoInterseccion = new Vector3();

        float denominator = (side1.start.x - side1.end.x) * (side2.start.y - side2.end.y) -
                            (side1.start.y - side1.end.y) * (side2.start.x - side2.end.x);

        if (Mathf.Abs(denominator) < Mathf.Epsilon)
        {
            return false;
        }

        float t = ((side1.start.x - side2.start.x) * (side2.start.y - side2.end.y) -
                   (side1.start.y - side2.start.y) * (side2.start.x - side2.end.x)) / denominator;
        float u = ((side1.start.x - side2.start.x) * (side1.start.y - side1.end.y) -
                   (side1.start.y - side2.start.y) * (side1.start.x - side1.end.x)) / denominator;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            puntoInterseccion = new Vector3(
                side1.start.x + t * (side1.end.x - side1.start.x),
                0,
                side1.start.y + t * (side1.end.y - side1.start.y)
            );
            return true;
        }

        return false;
    }

    public bool IsPointInPolygon(Vector2 point, Poligon polygon)
    {
        int intersectCount = 0;

        for (int i = 0; i < polygon.vertices.Count; i++)
        {
            Vector3 vertex1 = polygon.vertices[i];
            Vector3 vertex2 = polygon.vertices[(i + 1) % polygon.vertices.Count];

            if (RayIntersectsSegment(point, vertex1, vertex2))
            {
                intersectCount++;
            }
        }

        return (intersectCount % 2) == 1;
    }

    public bool RayIntersectsSegment(UnityEngine.Vector2 point, UnityEngine.Vector3 v1, Vector3 v2)
    {
        if (v1.y > v2.y)
        {
            Vector3 temp = v1;
            v1 = v2;
            v2 = temp;
        }

        if (Mathf.Abs(point.y - v1.y) < 0.001f || Mathf.Abs(point.y - v2.y) < 0.001f)
        {
            point.y += 0.0001f;  
        }

        if (point.y < v1.y || point.y > v2.y)
        {
            return false;
        }

        if (point.x > Mathf.Max(v1.x, v2.x))
        {
            return false;
        }

        if (point.x < Mathf.Min(v1.x, v2.x))
        {
            return true;
        }

        float deltaZ = v2.y - v1.y;
        float deltaX = v2.x - v1.x;

        if (Mathf.Abs(deltaZ) < 0.001f)
        {
            return false;
        }

        float slope = deltaX / deltaZ;
        float xIntersect = v1.x + (point.y - v1.y) * slope;

        return point.x < xIntersect;
    }
}
 */
