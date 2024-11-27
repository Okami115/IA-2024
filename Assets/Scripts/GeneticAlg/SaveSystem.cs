using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem
{
    public static void SerializeToFile<T>(T obj, string filePath)
    {
        try
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Serialization error: {ex.Message}");
        }
    }

    public static T DeserializeFromFile<T>(string filePath) where T : class
    {
        try
        {
            if (!File.Exists(filePath))
                return null;

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as T;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deserialization error: {ex.Message}");
            return null;
        }
    }

    public static void SerializeCollectionToFile<T>(IEnumerable<T> objects, string filePath)
    {
        try
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, objects.ToList());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Serialization error: {ex.Message}");
        }
    }

    public static List<T> DeserializeCollectionFromFile<T>(string filePath) where T : class
    {
        try
        {
            if (!File.Exists(filePath))
                return null;

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as List<T>;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deserialization error: {ex.Message}");
            return null;
        }
    }
}

public static class GeneticAlgorithmDataBatchHandler
{
    private const string FileName = "GeneticAlgorithmDataBatch.bin";

    public static void SaveBatch(IEnumerable<GeneticAlgorithmData> dataList, string filePath)
    {
        SaveSystem.SerializeCollectionToFile(dataList, filePath);
    }

    public static List<GeneticAlgorithmData> LoadBatch(string filePath)
    {
        return SaveSystem.DeserializeCollectionFromFile<GeneticAlgorithmData>(filePath);
    }
}

public class GeneticAlgorithmDataManager
{
    private List<GeneticAlgorithmData> _datasets = new List<GeneticAlgorithmData>();
            
    public void AddDataset(GeneticAlgorithmData data)
    {
        _datasets.Add(data);
    }

    public void SaveAll(string filePath)
    {
        MemoryStream stream = new MemoryStream();
        foreach (GeneticAlgorithmData data in _datasets)
        {
            byte[] dataArray = data.Serialize();
            stream.Capacity += dataArray.Length;
            stream.Write(dataArray, 0, dataArray.Length);
        }

        File.WriteAllBytes(filePath, stream.ToArray());
    }

    public void LoadAll(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Save file not found.");

        byte[] data = File.ReadAllBytes(filePath);

        int offset = 0;
        for (int index = 0; index < _datasets.Count; index++)
        {
            _datasets[index] = new GeneticAlgorithmData(data, ref offset);
        }

    }

    public List<GeneticAlgorithmData> GetAllDatasets()
    {
        return _datasets;
    }  
    public void ClearDatasets()
    {
        _datasets.Clear();
    }
}