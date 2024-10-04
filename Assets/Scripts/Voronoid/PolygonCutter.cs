using System.Collections.Generic;
using UnityEngine;

public class PolygonCutter : IPoligonCutter<Vector3>
{
    public void CutPolygon(Mine<Vector3> mine, Side cut)
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

            if (IsPointInPolygon(mine.currentNode.GetCoordinate(), new Poligon(polygon1)))
            {
                mine.poligon = new Poligon(polygon1);
            }
            else if (IsPointInPolygon(mine.currentNode.GetCoordinate(), new Poligon(polygon2)))
            {
                mine.poligon = new Poligon(polygon2);
            }
        }
    }

    public bool DetectarInterseccion(Side side1, Side side2, out Vector3 puntoInterseccion)
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
                puntoInterseccion = new Vector3(side1.start.x + t * (side1.end.x - side1.start.x), 0, side1.start.z + t * (side1.end.z - side1.start.z));
                return true;
            }
        }
        return false;
    }

    public bool IsPointInPolygon(Vector3 point, Poligon polygon)
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

    public bool RayIntersectsSegment(Vector3 point, Vector3 v1, Vector3 v2)
    {
        if (v1.z > v2.z)
        {
            Vector3 temp = v1;
            v1 = v2;
            v2 = temp;
        }

        if (point.z.Equals(v1.z) || point.z.Equals(v2.z))
        {
            point.z += 0.0001f;
        }

        if (point.z <= v1.z || point.z >= v2.z)
        {
            return false;
        }

        if (point.x >= Mathf.Max(v1.x, v2.x))
        {
            return false;
        }

        if (point.x <= Mathf.Min(v1.x, v2.x))
        {
            return true;
        }

        float slope = (v2.x - v1.x) / (v2.z - v1.z);
        float xIntersect = v1.x + (point.z - v1.z) * slope;

        return (point.x < xIntersect || point.x.Equals(xIntersect));
    }
}
