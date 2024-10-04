using System;

public interface IVoronoid<Coordinates>
    where Coordinates : IEquatable<Coordinates>
{
    public void RunVoronoid();
    public void GetSide(Mine<Coordinates> A, Mine<Coordinates> B);
}
