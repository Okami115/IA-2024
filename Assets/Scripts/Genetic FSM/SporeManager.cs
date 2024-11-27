using System;
using System.Collections.Generic;
using System.Numerics;
using ECS;

public class SporeManager
{
    public int generation = 0;

    public int hervivoreCount = 30;
    public int carnivoreCount = 20;
    public int scavengerCount = 20;
    public int gridSizeX = 10;
    public int gridSizeY = 10;

    public int EliteCount = 4;
    public float MutationChance = 0.10f;
    public float MutationRate = 0.01f;
    public int turnCount = 100;
    private int currentTurn = 0;

    public List<Hervivoro> herbis = new List<Hervivoro>();
    public List<Plant> plants = new List<Plant>();
    public List<Carnivoro> carnivores = new List<Carnivoro>();
    public List<Carroniero> scavengers = new List<Carroniero>();
    
    private Dictionary<Vector2, Plant> plantPositions = new Dictionary<Vector2, Plant>();

    public List<NeuralNetwork> herbMainBrains = new List<NeuralNetwork>();
    public List<NeuralNetwork> herbEatBrains = new List<NeuralNetwork>();
    public List<NeuralNetwork> herbMoveBrains = new List<NeuralNetwork>();
    public List<NeuralNetwork> herbEscapeBrains = new List<NeuralNetwork>();
    public List<NeuralNetwork> carnMainBrains = new List<NeuralNetwork>();
    public List<NeuralNetwork> carnMoveBrains = new List<NeuralNetwork>();
    public List<NeuralNetwork> carnEatBrains = new List<NeuralNetwork>();
    public List<NeuralNetwork> scavMainBrains = new List<NeuralNetwork>();
    public List<NeuralNetwork> scavFlokingBrains = new List<NeuralNetwork>();
    List<BrainData> herbBrainData;
    List<BrainData> carnivoreBrainData;
    List<BrainData> scavBrainData;

    public GeneticAlgorithmData HMainB;
    public GeneticAlgorithmData HEatB;
    public GeneticAlgorithmData HEscapeB;
    public GeneticAlgorithmData HMoveB;
    public GeneticAlgorithmData CMainB;
    public GeneticAlgorithmData CEatB;
    public GeneticAlgorithmData CMoveB;
    public GeneticAlgorithmData SMainB;
    public GeneticAlgorithmData SFlockB;

    public string fileToLoad;
    public string filepath;
    private string filetype = "abrazo";
    public List<GeneticAlgorithmData> data = new List<GeneticAlgorithmData>();
    Random random = new Random();

    public bool isActive;
    private Dictionary<uint, NeuralNetwork> entities;
    private GeneticAlgorithmDataManager manager = new GeneticAlgorithmDataManager();

    public SporeManager(List<BrainData> herbBrainData, List<BrainData> carnivoreBrainData,
        List<BrainData> scavBrainData, int gridSizeX, int gridSizeY, int hervivoreCount, int carnivoreCount,
        int scavengerCount, int turnCount)
    {
        this.herbBrainData = herbBrainData;
        this.carnivoreBrainData = carnivoreBrainData;
        this.scavBrainData = scavBrainData;
        this.turnCount = turnCount;
        this.gridSizeX = gridSizeX;
        this.gridSizeY = gridSizeY;
        this.hervivoreCount = hervivoreCount;
        this.carnivoreCount = carnivoreCount;
        this.scavengerCount = scavengerCount;


        const int SCAV_BRAINS = 2;
        const int CARN_BRAINS = 3;
        const int HERB_BRAINS = 4;
        if (herbBrainData.Count != HERB_BRAINS || carnivoreBrainData.Count != CARN_BRAINS ||
            scavBrainData.Count != SCAV_BRAINS)
        {
            throw new Exception(
                "The brainData is invalid. The herbivore data should be: 4, carnivore data should be: 3, scav data should be: 2");
        }

        CreateAgents();
        ECSManager.Init();
        entities = new Dictionary<uint, NeuralNetwork>();
        InitEntities();
        CreateNewGeneration();
    }

    public void Tick(float deltaTime)
    {
        if (!isActive)
            return;
        if (currentTurn < turnCount)
        {
            PreUpdateAgents(deltaTime);
            UpdateInputs();
            ECSManager.Tick(deltaTime);
            AfterTick(deltaTime);
            currentTurn++;
        }
        else
        {
            EpochAllBrains();
            CreateNewGeneration();
        }
    }

    private void CreateNewGeneration()
    {
        generation++;
        foreach (var herb in herbis)
        {
            herb.Reset(GetRandomHerbPosition());
        }

        foreach (var carn in carnivores)
        {
            carn.Reset(GetRandomCarnivorePosition());
        }

        foreach (var scav in scavengers)
        {
            scav.Reset(GetRandomScavPosition());
        }

        plantPositions.Clear();
        foreach (var plant in plants)
        {
            plant.Reset(GetRandomPlantPosition(plant));
        }

        currentTurn = 0;
    }

    private Vector2 GetRandomHerbPosition()
    {
        Random random = new Random();
        float randomX = random.Next(0, gridSizeX);
        float randomY = random.Next(0, gridSizeY / 2);
        return new Vector2(randomX, randomY);
    }

    private Vector2 GetRandomCarnivorePosition()
    {
        Random random = new Random();
        float randomX = random.Next(0, gridSizeX);
        float randomY = random.Next(gridSizeY / 2, gridSizeY);
        return new Vector2(randomX, randomY);
    }

    private Vector2 GetRandomScavPosition()
    {
        Random random = new Random();
        float randomX = random.Next(0, gridSizeX);
        float randomY = random.Next(0, gridSizeY);
        return new Vector2(randomX, randomY);
    }
    
    private Vector2 GetRandomPlantPosition(Plant plant)
    {
        float randomX = random.Next(0, gridSizeX);
        float randomY = random.Next(0, gridSizeY);

        Vector2 randomPlantPosition = new Vector2(randomX, randomY);
        while (plantPositions.ContainsKey(randomPlantPosition))
        {
            randomX = random.Next(0, gridSizeX);
            randomY = random.Next(0, gridSizeY);

            randomPlantPosition.X = randomX;
            randomPlantPosition.Y = randomY;
        }

        plantPositions.Add(randomPlantPosition, plant);

        return randomPlantPosition;
    }

    private void InitEntities()
    {
        for (int i = 0; i < hervivoreCount; i++)
        {
            CreateEntity(herbis[i].mainBrain);
            CreateEntity(herbis[i].moveBrain);
            CreateEntity(herbis[i].eatBrain);
            CreateEntity(herbis[i].runBrain);
        }

        for (int i = 0; i < carnivoreCount; i++)
        {
            CreateEntity(carnivores[i].mainBrain);
            CreateEntity(carnivores[i].moveBrain);
            CreateEntity(carnivores[i].eatBrain);
        }

        for (int i = 0; i < scavengerCount; i++)
        {
            CreateEntity(scavengers[i].mainBrain);
            CreateEntity(scavengers[i].flockingBrain);
        }
    }

    private void CreateAgents()
    {
        for (int i = 0; i < hervivoreCount; i++)
        {
            herbis.Add(new Hervivoro(this, herbBrainData[0].ToBrain(), herbBrainData[1].ToBrain(),
                herbBrainData[2].ToBrain(), herbBrainData[3].ToBrain()));
            herbMainBrains.Add(herbis[i].mainBrain);
            herbMoveBrains.Add(herbis[i].moveBrain);
            herbEatBrains.Add(herbis[i].eatBrain);
            herbEscapeBrains.Add(herbis[i].runBrain);
        }


        for (int i = 0; i < carnivoreCount; i++)
        {
            carnivores.Add(new Carnivoro(this, carnivoreBrainData[0].ToBrain(), carnivoreBrainData[1].ToBrain(),
                carnivoreBrainData[2].ToBrain()));
            carnMainBrains.Add(carnivores[i].mainBrain);
            carnEatBrains.Add(carnivores[i].eatBrain);
            carnMoveBrains.Add(carnivores[i].moveBrain);
        }


        for (int i = 0; i < scavengerCount; i++)
        {
            scavengers.Add(new Carroniero(this, scavBrainData[0].ToBrain(), scavBrainData[1].ToBrain()));
            scavMainBrains.Add(scavengers[i].mainBrain);
            scavFlokingBrains.Add(scavengers[i].flockingBrain);
        }

        HMainB = new GeneticAlgorithmData(EliteCount, MutationChance, MutationRate, herbMainBrains[0]);
        HEatB = new GeneticAlgorithmData(EliteCount, MutationChance, MutationRate, herbEatBrains[0]);
        HEscapeB = new GeneticAlgorithmData(EliteCount, MutationChance, MutationRate, herbEscapeBrains[0]);
        HMoveB = new GeneticAlgorithmData(EliteCount, MutationChance, MutationRate, herbMoveBrains[0]);
        CMainB = new GeneticAlgorithmData(EliteCount, MutationChance, MutationRate, carnMainBrains[0]);
        CEatB = new GeneticAlgorithmData(EliteCount, MutationChance, MutationRate, carnEatBrains[0]);
        CMoveB = new GeneticAlgorithmData(EliteCount, MutationChance, MutationRate, carnMoveBrains[0]);
        SMainB = new GeneticAlgorithmData(EliteCount, MutationChance, MutationRate, scavMainBrains[0]);
        SFlockB = new GeneticAlgorithmData(EliteCount, MutationChance, MutationRate, scavFlokingBrains[0]);
        data.Add(HMainB);
        data.Add(HEatB);
        data.Add(HEscapeB);
        data.Add(HMoveB);
        data.Add(CMainB);
        data.Add(CEatB);
        data.Add(CMoveB);
        data.Add(SMainB);
        data.Add(SFlockB);
        
        foreach (GeneticAlgorithmData algorithmData in data)
        {
            manager.AddDataset(algorithmData);
        }
        
        for (int i = 0; i < hervivoreCount * 2; i++)
        {
            plants.Add(new Plant());
        }
    }

    private void CreateEntity(NeuralNetwork brain)
    {
        uint entityID = ECSManager.CreateEntity();
        ECSManager.AddComponent<BiasComponent>(entityID, new BiasComponent(brain.bias));
        ECSManager.AddComponent<SigmoidComponent>(entityID, new SigmoidComponent(brain.p));
        ECSManager.AddComponent<InputLayerComponent>(entityID, new InputLayerComponent(brain.GetInputLayer()));
        ECSManager.AddComponent<HiddenLayerComponent>(entityID, new HiddenLayerComponent(brain.GetHiddenLayers()));
        ECSManager.AddComponent<OutputLayerComponent>(entityID, new OutputLayerComponent(brain.GetOutputLayer()));
        ECSManager.AddComponent<OutputComponent>(entityID, new OutputComponent(brain.outputs));
        ECSManager.AddComponent<InputComponent>(entityID, new InputComponent(brain.inputs, brain.InputsCount));
        entities.Add(entityID, brain);
    }

    private void EpochAllBrains()
    {
        foreach (var geneticAlgorithmData in data)
        {
            geneticAlgorithmData.generationCount = generation;
        }

        EpochHerbivore();
        EpochCarnivore();
        EpochScavenger();

        string file = $"{filepath}{generation}.{filetype}";
        manager.SaveAll(file);
        foreach (KeyValuePair<uint, NeuralNetwork> entity in entities)
        {
            HiddenLayerComponent inputComponent = ECSManager.GetComponent<HiddenLayerComponent>(entity.Key);
            inputComponent.hiddenLayers = entity.Value.GetHiddenLayers();
            OutputLayerComponent outputComponent = ECSManager.GetComponent<OutputLayerComponent>(entity.Key);
            outputComponent.layer = entity.Value.GetOutputLayer();
        }
    }

    private void EpochScavenger()
    {
        int count = 0;
        foreach (var scav in scavengers)
        {
            if (scav.hasEaten)
            {
                count++;
            }
        }

        bool isGenerationDead = count <= 1;

        BrainEpoch(scavMainBrains, isGenerationDead, SMainB);
        BrainEpoch(scavFlokingBrains, isGenerationDead, SFlockB);
    }

    void EpochCarnivore()
    {
        int count = 0;
        foreach (var carnivore in carnivores)
        {
            if (carnivore.hasEatenEnoughFood)
            {
                count++;
            }
            else
            {
                carnivore.mainBrain.DestroyFitness();
                carnivore.eatBrain.DestroyFitness();
                carnivore.moveBrain.DestroyFitness();
            }
        }

        bool isGenerationDead = count <= 1;


        BrainEpoch(carnMainBrains, isGenerationDead, CMainB);
        BrainEpoch(carnEatBrains, isGenerationDead, CEatB);
        BrainEpoch(carnMoveBrains, isGenerationDead, CMoveB);
    }

    private void EpochHerbivore()
    {
        int count = 0;
        foreach (Hervivoro herbivore in herbis)
        {
            if (herbivore.lives > 0 && herbivore.hasEatenFood)
            {
                count++;
            }
            else
            {
                herbivore.mainBrain.DestroyFitness();
                herbivore.eatBrain.DestroyFitness();
                herbivore.runBrain.DestroyFitness();
                herbivore.moveBrain.DestroyFitness();
            }
        }

        bool isGenerationDead = count <= 1;

        BrainEpoch(herbMainBrains, isGenerationDead, HMainB);
        BrainEpoch(herbMoveBrains, isGenerationDead, HMoveB);
        BrainEpoch(herbEatBrains, isGenerationDead, HEatB);
        BrainEpoch(herbEscapeBrains, isGenerationDead, HEscapeB);
    }

    private void BrainEpoch(List<NeuralNetwork> brains, bool force, GeneticAlgorithmData info)
    {
        Genome[] newGenomes = GeneticAlgorithm.Epoch(GetGenomes(brains), info);
        info.lastGenome = newGenomes;
        for (int i = 0; i < brains.Count; i++)
        {
            // Brain brain =;
            brains[i] = new NeuralNetwork(info.brainStructure);
            // brain.CopyStructureFrom(info.brainStructure);
            brains[i].SetWeights(newGenomes[i].genome);
        }
    }

    private void RestoreSave()
    {
        List<GeneticAlgorithmData> dataToPaste = manager.GetAllDatasets();
            
        HMainB = dataToPaste[0];
        HEatB = dataToPaste[1];
        HEscapeB = dataToPaste[2];
        HMoveB = dataToPaste[3];
        CMainB = dataToPaste[4];
        CEatB = dataToPaste[5];
        CMoveB = dataToPaste[6];
        SMainB = dataToPaste[7];
        SFlockB = dataToPaste[8];
        manager.ClearDatasets();
        manager.AddDataset(HMainB);
        manager.AddDataset(HEatB);
        manager.AddDataset(HEscapeB);
        manager.AddDataset(HMoveB);
        manager.AddDataset(CMainB);
        manager.AddDataset(CEatB);
        manager.AddDataset(CMoveB);
        manager.AddDataset(SMainB);
        manager.AddDataset(SFlockB);
            
            
        generation = HMainB.generationCount;
        RestoreBrainsData(herbMainBrains, HMainB);
        RestoreBrainsData(herbMoveBrains, HMoveB);
        RestoreBrainsData(herbEatBrains, HEatB);
        RestoreBrainsData(herbEscapeBrains, HEscapeB);
        RestoreBrainsData(carnMainBrains, CMainB);
        RestoreBrainsData(carnEatBrains, CEatB);
        RestoreBrainsData(carnMoveBrains, CMoveB);
        RestoreBrainsData(scavMainBrains, SMainB);
        RestoreBrainsData(scavFlokingBrains, SFlockB);
        
        foreach (var herb in herbis)
        {
            herb.Reset(GetRandomHerbPosition());
        }

        foreach (var carn in carnivores)
        {
            carn.Reset(GetRandomCarnivorePosition());
        }

        foreach (var scav in scavengers)
        {
            scav.Reset(GetRandomScavPosition());
        }

        plantPositions.Clear();
        foreach (var plant in plants)
        {
            plant.Reset(GetRandomPlantPosition(plant));
        }
    }

    private void RestoreBrainsData(List<NeuralNetwork> brains, GeneticAlgorithmData info)
    {
        for (int i = 0; i < brains.Count; i++)
        {
            brains[i] = new NeuralNetwork(info.brainStructure);
            brains[i].SetWeights(info.lastGenome[i].genome);
        }
    }

    private static Genome[] GetGenomes(List<NeuralNetwork> brains)
    {
        List<Genome> genomes = new List<Genome>();
        foreach (var brain in brains)
        {
            Genome genome = new Genome(brain.GetTotalWeightsCount());
            genomes.Add(genome);
        }

        return genomes.ToArray();
    }

    private void PreUpdateAgents(float deltaTime)
    {
        foreach (Hervivoro herbi in herbis)
        {
            herbi.PreUpdate(deltaTime);
        }

        foreach (Carnivoro carn in carnivores)
        {
            carn.PreUpdate(deltaTime);
        }

        foreach (Carroniero scav in scavengers)
        {
            scav.PreUpdate(deltaTime);
        }
    }

    private void UpdateInputs()
    {
        foreach (KeyValuePair<uint, NeuralNetwork> entity in entities)
        {
            InputComponent inputComponent = ECSManager.GetComponent<InputComponent>(entity.Key);
            inputComponent.inputs = entity.Value.inputs;
        }
    }

    public void AfterTick(float deltaTime = 0)
    {
        foreach (KeyValuePair<uint, NeuralNetwork> entity in entities)
        {
            OutputComponent output = ECSManager.GetComponent<OutputComponent>(entity.Key);
            entity.Value.outputs = output.outputs;
        }

        foreach (Hervivoro herbi in herbis)
        {
            herbi.Update(deltaTime);
        }

        foreach (Carnivoro carn in carnivores)
        {
            carn.Update(deltaTime);
        }

        foreach (Carroniero scav in scavengers)
        {
            scav.Update(deltaTime);
        }
    }

    public void Save()
    {
    }

    public void Load()
    {
        manager.LoadAll($"{fileToLoad}.{filetype}");
        RestoreSave();
        isActive = false;
        foreach (var entity in entities)
        {
            ECSManager.GetComponent<BiasComponent>(entity.Key).X = entity.Value.bias;
            ECSManager.GetComponent<SigmoidComponent>(entity.Key).X = entity.Value.p;
            ECSManager.GetComponent<InputLayerComponent>(entity.Key).layer = entity.Value.GetInputLayer();
            HiddenLayerComponent hiddenLayerComponent = ECSManager.GetComponent<HiddenLayerComponent>(entity.Key);
            hiddenLayerComponent.hiddenLayers = entity.Value.GetHiddenLayers();
            hiddenLayerComponent.SetHighestLayerSize();
            ECSManager.GetComponent<OutputLayerComponent>(entity.Key).layer = entity.Value.GetOutputLayer();
            ECSManager.GetComponent<OutputComponent>(entity.Key).outputs = entity.Value.outputs;
            ECSManager.GetComponent<InputComponent>(entity.Key).inputs = entity.Value.inputs;
        }

        currentTurn = 0;
    }

    public Hervivoro GetNearHerbivore(Vector2 position)
    {
        Hervivoro nearest = herbis[0];
        float distance = (position.X * nearest.position.X) + (position.Y * nearest.position.Y);

        foreach (Hervivoro go in herbis)
        {
            float newDist = (go.position.X * position.X) + (go.position.Y * position.Y);
            if (newDist < distance)
            {
                nearest = go;
                distance = newDist;
            }
        }

        return nearest;
    }

    public virtual Plant GetNearPlant(Vector2 position)
    {
        Plant nearest = plants[0];
        float distance = (position.X * nearest.position.X) + (position.Y * nearest.position.Y);

        foreach (var plant in plantPositions)
        {
            if (plant.Value.isAvailable)
            {
                float newDist = (plant.Value.position.X * position.X) + (plant.Value.position.Y * position.Y);
                if (newDist < distance)
                {
                    nearest = plant.Value;
                    distance = newDist;
                }
            }
        }

        return nearest;
    }
    public List<Carroniero> GetNearScavs(Vector2 position)
    {
        var nearbyScav = new List<(Carroniero scav, float distance)>();
        
        foreach (Carroniero go in scavengers)
        {
            float distance = (position.X - go.position.X) * (position.X - go.position.X)
                             + (position.Y - go.position.Y) * (position.Y - go.position.Y);
            nearbyScav.Add((go, distance));
        }

        nearbyScav.Sort((a, b) => a.distance.CompareTo(b.distance));

        List<Carroniero> nearScav = new List<Carroniero>();
        nearScav.Add(nearbyScav[0].scav);
        nearScav.Add(nearbyScav[1].scav);
        nearScav.Add(nearbyScav[2].scav);
        return nearScav;
    }

    public List<Vector2> GetNearCarnivores(Vector2 position)
    {
        List<(Carnivoro scav, float distance)> nearCarn = new List<(Carnivoro scav, float distance)>();

        foreach (Carnivoro go in carnivores)
        {
            float distance = (position.X - go.position.X) * (position.X - go.position.X)
                             + (position.Y - go.position.Y) * (position.Y - go.position.Y);
            nearCarn.Add((go, distance));
        }

        nearCarn.Sort((a, b) => a.distance.CompareTo(b.distance));

        List<Vector2> carnToReturn = new List<Vector2>();
        carnToReturn.Add(nearCarn[0].scav.position);
        carnToReturn.Add(nearCarn[1].scav.position);
        carnToReturn.Add(nearCarn[2].scav.position);
        return carnToReturn;
    }
}

public class BrainData
{
    public int InputsCount = 4;

    public int OutputsCount = 2;
    public int[] NeuronsCountPerHL;
    public float Bias = 1f;
    public float P = 1f;

    public BrainData(int inputsCount, int[] NeuronsCountPerHL, int outputsCount, float bias, float p)
    {
        InputsCount = inputsCount;
        this.NeuronsCountPerHL = NeuronsCountPerHL;
        OutputsCount = outputsCount;
        Bias = bias;
        P = p;
    }

    public NeuralNetwork ToBrain()
    {
        return NeuralNetwork.CreateBrain(InputsCount, NeuronsCountPerHL, OutputsCount, Bias, P);
    }
}

public class GeneticAlgorithmData
{
     public float totalFitness = 0;
    public int eliteCount = 0;
    public float mutationChance = 0.0f;
    public float mutationRate = 0.0f;
    public readonly int maxStalledGenerationsUntilEvolve = 5;
    public NeuralNetwork brainStructure;
    public Genome[] lastGenome = Array.Empty<Genome>();
    public int generationStalled = 0;
    public int generationCount = 0;

    public GeneticAlgorithmData()
    {
        eliteCount = 5;
        mutationChance = 0.2f;
        mutationRate = 0.4f;
    }

    public GeneticAlgorithmData(byte[] data, ref  int offset)
    {
        eliteCount = BitConverter.ToInt32(data, offset);
        offset += sizeof(int);
        
        mutationChance = BitConverter.ToSingle(data, offset);
        offset += sizeof(float);

        mutationRate = BitConverter.ToSingle(data, offset);
        offset += sizeof(float);
        
        brainStructure = new NeuralNetwork(data, ref offset);
        
        lastGenome = CreateGenomeArray(data, ref offset);
        
        generationStalled = BitConverter.ToInt32(data, offset);
        offset += sizeof(int);
        
        generationCount = BitConverter.ToInt32(data, offset);
        offset += sizeof(int);
    }

    public byte[] Serialize()
    {
        List<byte> bytes = new List<byte>();
        
        bytes.AddRange(BitConverter.GetBytes(eliteCount));

        // Serialize mutationChance (4 bytes for a float)
        bytes.AddRange(BitConverter.GetBytes(mutationChance));

        // Serialize mutationRate (4 bytes for a float)
        bytes.AddRange(BitConverter.GetBytes(mutationRate));

        // Serialize brainStructure
        bytes.AddRange(brainStructure.Serialize());

        // Serialize lastGenome
        bytes.AddRange(SerializeGenomeArray(lastGenome));

        // Serialize generationStalled (4 bytes for an integer)
        bytes.AddRange(BitConverter.GetBytes(generationStalled));

        // Serialize generationCount (4 bytes for an integer)
        bytes.AddRange(BitConverter.GetBytes(generationCount));

        return bytes.ToArray();
    }

    private Genome[] CreateGenomeArray(byte[] data, ref int currentOffset)
    {

        int arrayLength = BitConverter.ToInt32(data, currentOffset);
        currentOffset += sizeof(int);


        Genome[] genomes = new Genome[arrayLength];


        for (int i = 0; i < arrayLength; i++)
        {
            genomes[i] = new Genome(data, ref currentOffset);
        }

        return genomes;
    }

    private byte[] SerializeGenomeArray(Genome[] genomes)
    {
        List<byte> bytes = new List<byte>();

        // Serialize the array length (4 bytes for an integer)
        bytes.AddRange(BitConverter.GetBytes(genomes.Length));

        // Serialize each Genome
        foreach (var genome in genomes)
        {
            bytes.AddRange(genome.Serialize());
        }

        return bytes.ToArray();
    }
    public GeneticAlgorithmData(int eliteCount, float mutationChance, float mutationRate, NeuralNetwork brain,
        int maxStalledGenerationsUntilEvolve = 5)
    {
        this.eliteCount = eliteCount;
        this.mutationChance = mutationChance;
        this.mutationRate = mutationRate;
        this.brainStructure = brain;
        this.maxStalledGenerationsUntilEvolve = maxStalledGenerationsUntilEvolve;
    }

    public GeneticAlgorithmData(GeneticAlgorithmData data)
    {
        this.eliteCount = data.eliteCount;
        this.mutationChance = data.mutationChance;
        this.mutationRate = data.mutationRate;
        this.brainStructure = data.brainStructure;
        this.maxStalledGenerationsUntilEvolve = data.maxStalledGenerationsUntilEvolve;
    }
}