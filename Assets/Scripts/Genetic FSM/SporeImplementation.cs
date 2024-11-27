using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class SporeImplementation : MonoBehaviour
{
    public SporeManager sporeManager;
    private List<BrainData> herbBrainData;
    private List<BrainData> carnivoreBrainData;
    private List<BrainData> scavBrainData;
    [SerializeField] private int gridSizeX;
    [SerializeField] private int gridSizeY;
    [SerializeField] private int hervivoreCount;
    [SerializeField] private int carnivoreCount;
    [SerializeField] private int scavengerCount;
    public int turnCount;
    public float timer;
    public int currentGeneration;
    public bool isSimulationActive;

    ParallelOptions parallel = new ParallelOptions();
    
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
        scavBrainData.Add(new BrainData(5, new[] { 3, 5 }, 2, 0, 0));
        scavBrainData.Add(new BrainData(8, new[] { 5, 5, 5}, 4, 0, 0));
        
        parallel.MaxDegreeOfParallelism = 64;
        
        sporeManager = new SporeManager(herbBrainData, carnivoreBrainData, scavBrainData, gridSizeX, gridSizeY, hervivoreCount, carnivoreCount ,scavengerCount, turnCount);
    }
    
    void Update()
    {
        timer += Time.deltaTime;

        // Tick the simulation at a fixed interval
        // if (timer >= simulationDeltaTime)
        // {
        currentGeneration = sporeManager.generation;
        sporeManager.isActive = isSimulationActive;
        sporeManager.Tick(Time.deltaTime);
        timer = 0;
        // }
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
        
        for (int i = 0; i < sporeManager.plants.Count; i++)
        {
            Gizmos.color = Color.cyan;
            Vector3 aux = new Vector3(sporeManager.plants[i].position.X, sporeManager.plants[i].position.Y, 0);
            
            Gizmos.DrawSphere(aux, 0.07f);
        }
        

        for (int i = 0; i < sporeManager.herbis.Count; i++)
        {
            Gizmos.color = Color.blue;
            Vector3 aux = new Vector3(sporeManager.herbis[i].position.X, sporeManager.herbis[i].position.Y, 0);
            
            Gizmos.DrawSphere(aux, 0.1f);
        }
        
        for (int i = 0; i < sporeManager.carnivores.Count; i++)
        {
            Gizmos.color = Color.red;
            Vector3 aux = new Vector3(sporeManager.carnivores[i].position.X, sporeManager.carnivores[i].position.Y, 0);
            
            Gizmos.DrawCube(aux, new Vector3(0.1f, 0.1f, 0.1f));
        }
        
        for (int i = 0; i < sporeManager.scavengers.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Vector3 aux = new Vector3(sporeManager.scavengers[i].position.X, sporeManager.scavengers[i].position.Y, 0);
            
            Gizmos.DrawCube(aux, new Vector3(0.1f, 0.1f, 0.1f));
        }
    }
}
