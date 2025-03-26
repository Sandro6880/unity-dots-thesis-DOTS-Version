using TMPro;
using Unity.Entities;
using UnityEngine;

public class GetVertices : MonoBehaviour
{

    public TMP_Text verticesCountText;
    public string prefabName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int totalVertices = CountVertices(transform);
        verticesCountText.text = $"Number of Vertices for {prefabName}: {totalVertices}";
    }
    int CountVertices(Transform parent)
    {
        int count = 0;

        // Check if this object has a MeshFilter and add its vertex count
        MeshFilter meshFilter = parent.GetComponent<MeshFilter>();
        if (meshFilter && meshFilter.sharedMesh)
        {
            count += meshFilter.sharedMesh.vertexCount;
        }

        // Recursively check all child objects
        foreach (Transform child in parent)
        {
            count += CountVertices(child);
        }

        return count;
    }
}
