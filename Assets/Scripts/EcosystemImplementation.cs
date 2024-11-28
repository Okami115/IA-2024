using System.Collections.Generic;
using System.Threading.Tasks;
using carnivorous;
using herbivorous;
using manager;
using scavenger;
using UnityEngine;

public class EcosystemImplementation : MonoBehaviour
{
    public EcosystemManager ecosystemManager;
    private List<BrainData> herbBrainData;
    private List<BrainData> carnivoreBrainData;
    private List<BrainData> scavBrainData;
    [Header("Play variables")]
    [SerializeField] private int gridSizeX;
    [SerializeField] private int gridSizeY;
    [SerializeField] private int hervivoreCount;
    [SerializeField] private int carnivoreCount;
    [SerializeField] private int scavengerCount;
    [SerializeField] private bool loadOnActive;
    public int turnCount;
    public int currentGeneration;
    public bool isSimulationActive;
    
    ParallelOptions parallel = new ParallelOptions();
    [Header("Draw variables")]
    [SerializeField] private Mesh CubeMesh;
    [SerializeField] private Material plantMaterial;
    [SerializeField] private Material deadHerbivoreMaterial;
    [SerializeField] private Material herbivoreMaterial;
    [SerializeField] private Material carnivoreMaterial;
    [SerializeField] private Material scavengerMaterial;
    [Header("Save variables")]
    [SerializeField] private string filePath;
    [SerializeField] private string fileExtension;
    [SerializeField] private string fileToLoad;

    void Start()
    {
        herbBrainData = new List<BrainData>();
        herbBrainData.Add(new BrainData(11, new[] { 7, 5, 3 }, 3, 0, 0)); //main
        herbBrainData.Add(new BrainData(4, new[] { 5, 4 }, 4, 0, 0)); //move
        herbBrainData.Add(new BrainData(5, new[] { 3 }, 1, 0, 0)); //eat
        herbBrainData.Add(new BrainData(8, new[] { 5, 3 }, 4, 0, 0)); //run
        
        carnivoreBrainData = new List<BrainData>();
        carnivoreBrainData.Add(new BrainData(5, new[] { 3, 2 }, 2, 0, 0));
        carnivoreBrainData.Add(new BrainData(4, new[] { 3, 2 }, 2, 0, 0));
        carnivoreBrainData.Add(new BrainData(5, new[] { 2, 2 }, 1, 0, 0));
        
        scavBrainData = new List<BrainData>();
        scavBrainData.Add(new BrainData(5, new int[] { 4, 3 }, 2, 0, 0)); // Move
        scavBrainData.Add(new BrainData(8, new int[] { 7, 6, 5 }, 6, 0, 0)); // Flocking
        
        parallel.MaxDegreeOfParallelism = 32;
        
        ecosystemManager = new EcosystemManager(herbBrainData, carnivoreBrainData, scavBrainData, gridSizeX, gridSizeY, hervivoreCount, carnivoreCount ,scavengerCount, turnCount);

        ecosystemManager.filepath = Application.dataPath + filePath;
        ecosystemManager.fileType = fileExtension;
        ecosystemManager.fileToLoad = Application.dataPath + fileToLoad;
        
        if (loadOnActive)
        {
            Load();
        }
    }
    
    void Update()
    {
        currentGeneration = ecosystemManager.generation;
        ecosystemManager.isActive = isSimulationActive;
        ecosystemManager.Tick(Time.deltaTime);
    }
    
    [ContextMenu("Load Save")]
    private void Load()
    {
        ecosystemManager.Load();
    }
    
    private void DrawEntities()
    {
        foreach (Plant agent in ecosystemManager.plants)
        {
            DrawSquare(new Vector3(agent.position.X, agent.position.Y, 0), plantMaterial, 1);
        }

        foreach (Herbivorous agent in ecosystemManager.herbis)
        {
            if (agent.lives <= 0)
            {
                DrawSquare(new Vector3(agent.position.X, agent.position.Y, 0), deadHerbivoreMaterial, 1);
            }
            else
            {
                DrawSquare(new Vector3(agent.position.X, agent.position.Y, 0), herbivoreMaterial, 1);
            }
        }

        foreach (Carnivorous agent in ecosystemManager.carnivores)
        {
            DrawSquare(new Vector3(agent.position.X, agent.position.Y, 0), carnivoreMaterial, 1);
        }

        foreach (Scavenger agent in ecosystemManager.scavengers)
        {
            DrawSquare(new Vector3(agent.position.X, agent.position.Y, 0), scavengerMaterial, 1);
        }
    }

    private void DrawSquare(Vector3 position, Material color, float squareSize)
    {
        color.SetPass(0);
        Matrix4x4 matrix =
            Matrix4x4.TRS(position, Quaternion.identity, new Vector3(squareSize, squareSize, squareSize));
        Graphics.DrawMeshNow(CubeMesh, matrix);
    }


    public void OnRenderObject()
    {
        DrawEntities();
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.green;
        
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                Gizmos.DrawSphere(new Vector3(i,j,0), 0.05f);
            }
        }
    }
}
