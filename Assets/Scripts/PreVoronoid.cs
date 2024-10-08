using UnityEngine;

public class PreVoronoid : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;

    public Vector3[] generadores;

    public Color[] colores;

    void Start()
    {
        CrearDiagramaDeVoronoi();
    }

    void CrearDiagramaDeVoronoi()
    {
        if (colores.Length < generadores.Length)
        {
            Debug.LogError("No hay suficientes colores para los generadores.");
            return;
        }

        GameObject voronoiContainer = new GameObject("VoronoiDiagram");

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 cellPosition = new Vector3(x, y);

                int closestGeneratorIndex = 0;
                float minDistance = float.MaxValue;

                for (int i = 0; i < generadores.Length; i++)
                {
                    float distance = Vector3.Distance(cellPosition, generadores[i]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestGeneratorIndex = i;
                    }
                }

                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cell.transform.position = new Vector3(cellPosition.x, cellPosition.y, 0);
                cell.transform.parent = voronoiContainer.transform;

                Renderer cellRenderer = cell.GetComponent<Renderer>();
                cellRenderer.material.color = colores[closestGeneratorIndex];
            }
        }
    }
}
