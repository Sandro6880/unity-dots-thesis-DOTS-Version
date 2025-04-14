using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GameObjectCountCSVWriter : MonoBehaviour
{
    public string csvFilePath = "/Csv/GameObjectCounts.csv";
    public GameObject[] prefabs;
    public Dictionary<string, GameObject> models = new Dictionary<string, GameObject>(); // Fill with model names and corresponding prefabs

    void Start()
    {
        models.Add("Cube", prefabs[0]);
        models.Add("Car_low", prefabs[1]);
        models.Add("Car_mid", prefabs[2]);
        models.Add("Car_high", prefabs[3]);
        models.Add("Car_Prediction", prefabs[4]);
        WriteObjectDataToCSV();
    }

    void WriteObjectDataToCSV()
    {
        using (StreamWriter writer = new StreamWriter(csvFilePath))
        {
            // Write the header row
            writer.Write("Object");
            for (int step = 100; step <= 10000; step += 100)
            {
                writer.Write($";{step}");
            }
            writer.WriteLine();

            // Process each model
            foreach (var model in models)
            {
                int baseObjectCount = CountGameObjects(model.Value.transform);

                // Write GameObject Counts
                writer.Write(model.Key);
                for (int step = 100; step <= 10000; step += 100)
                {
                    int totalObjects = baseObjectCount * step;
                    writer.Write($";{totalObjects}");
                }
                writer.WriteLine();
            }
        }
        Debug.Log($"GameObject count data written to {csvFilePath}");
    }

    int CountGameObjects(Transform parent)
    {
        int count = 1; // Include the parent object itself
        foreach (Transform child in parent)
        {
            count += CountGameObjects(child);
        }
        return count;
    }
}
