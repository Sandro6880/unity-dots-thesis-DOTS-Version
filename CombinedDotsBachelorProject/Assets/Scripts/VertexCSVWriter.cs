using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class VertexCSVWriter : MonoBehaviour
{
    public string csvFilePath = "D:\\Bachelor\\VertexCounts.csv";
    public Dictionary<string, int> modelsVertexCounts = new Dictionary<string, int>(); // Fill with model names and base vertex counts

    void Start()
    {
        modelsVertexCounts.Add("Cube", 24);
        modelsVertexCounts.Add("Car_low", 2067);
        modelsVertexCounts.Add("Car_mid", 9032);
        modelsVertexCounts.Add("Car_high", 29233);
        modelsVertexCounts.Add("Car_Prediction", 23476);
        WriteVertexDataToCSV();
    }

    void WriteVertexDataToCSV()
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

            // Write data rows for each model
            foreach (var model in modelsVertexCounts)
            {
                writer.Write(model.Key); // Model name
                for (int step = 100; step <= 10000; step += 100)
                {
                    int totalVertices = model.Value * step;
                    writer.Write($";{totalVertices}");
                }
                writer.WriteLine();
            }
        }
        Debug.Log($"Vertex data written to {csvFilePath}");
    }
}
