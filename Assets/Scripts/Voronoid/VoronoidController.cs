using UnityEngine;

public class VoronoidController : IVoronoid<Vector3>
{
    private GrapfView grapfView;

    public VoronoidController(GrapfView grapfView)
    {
        this.grapfView = grapfView;
    }

    public void RunVoronoid()
    {
        for (int i = 0; i < grapfView.mines.Count; i++)
        {
            grapfView.mines[i].poligon = new Poligon(grapfView.vertex);
        }

        for (int i = 0; i < grapfView.mines.Count; i++)
        {
            for (int j = 0; j < grapfView.mines.Count; j++)
            {
                GetSide(grapfView.mines[i], grapfView.mines[j]);
            }
        }
    }

    public void GetSide(Mine<Vector3> A, Mine<Vector3> B)
    {
        Vector3 mid = new Vector3((A.currentNode.GetCoordinate().x + B.currentNode.GetCoordinate().x) / 2.0f, 
            (A.currentNode.GetCoordinate().y + B.currentNode.GetCoordinate().y) / 2, (A.currentNode.GetCoordinate().z + B.currentNode.GetCoordinate().z) / 2.0f);

        Vector3 connector = A.currentNode.GetCoordinate() - B.currentNode.GetCoordinate();

        Vector3 direction = new Vector3(-connector.z, 0, connector.x).normalized;

        Side outCut = new Side(mid - direction * 100.0f, mid + direction * 100.0f);

        PolygonCutter polygonCutter = new PolygonCutter();
        polygonCutter.CutPolygon(A, outCut);
    }
}
