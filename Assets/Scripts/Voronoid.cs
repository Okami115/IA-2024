using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Voronoid : MonoBehaviour
{
    // Tamaño de la grilla
    public int gridWidth = 10;
    public int gridHeight = 10;

    // Puntos generadores (sitios)
    public Vector2Int[] generadores;

    // Colores para los sitios de Voronoi
    public Color[] colores;

    void Start()
    {
        // Crear y dibujar el diagrama de Voronoi
        CrearDiagramaDeVoronoi();
    }

    void CrearDiagramaDeVoronoi()
    {
        // Verifica que haya suficientes colores para los generadores
        if (colores.Length < generadores.Length)
        {
            Debug.LogError("No hay suficientes colores para los generadores.");
            return;
        }

        // Crear un objeto vacío para contener las celdas del diagrama
        GameObject voronoiContainer = new GameObject("VoronoiDiagram");

        // Recorrer cada celda de la grilla
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2Int cellPosition = new Vector2Int(x, y);

                // Encuentra el generador más cercano a esta celda
                int closestGeneratorIndex = 0;
                float minDistance = float.MaxValue;

                for (int i = 0; i < generadores.Length; i++)
                {
                    float distance = Vector2Int.Distance(cellPosition, generadores[i]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestGeneratorIndex = i;
                    }
                }

                // Crear una representación visual para la celda
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cell.transform.position = new Vector3(cellPosition.x, cellPosition.y, 0);
                cell.transform.parent = voronoiContainer.transform;

                // Asigna un color basado en el generador más cercano
                Renderer cellRenderer = cell.GetComponent<Renderer>();
                cellRenderer.material.color = colores[closestGeneratorIndex];
            }
        }
    }
}
