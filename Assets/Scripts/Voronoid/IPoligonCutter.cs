using System;

public interface IPoligonCutter<Coordinates>
    where Coordinates : IEquatable<Coordinates>
{
    public void CutPolygon(Mine<Coordinates> mine, Side cut);
    public bool DetectarInterseccion(Side side1, Side side2, out Coordinates puntoInterseccion);
    public bool IsPointInPolygon(Coordinates point, Poligon polygon);
    public bool RayIntersectsSegment(Coordinates point, Coordinates v1, Coordinates v2);
}
