using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    private float[,] heights; // Array to store the heights of each vertex
    public int gridSizeX = 10; // Number of vertices along the X-axis
    public int gridSizeY = 10; // Number of vertices along the Y-axis
    public float spacing = 1f; // Distance between each vertex
    public float minHeight = 0f; // Minimum height of the terrain
    public float maxHeight = 2f; // Maximum height of the terrain

    private void Start()
    {
        heights = new float[gridSizeX + 1, gridSizeY + 1]; // Initialize the heights array

        GenerateHeights();
        SmoothHeights();
        CreateMesh();
    }

    void GenerateHeights()
    {
        // Loop through the grid size
        for (int x = 0; x <= gridSizeX; x++)
        {
            for (int y = 0; y <= gridSizeY; y++)
            {
                // Assign a random height and store it in the heights array
                heights[x, y] = Random.Range(minHeight, maxHeight); // Store the random height
            }
        }
    }

    void SmoothHeights()
    {
        float[,] smoothedHeights = new float[gridSizeX + 1, gridSizeY + 1]; // Temporary array to store smoothed heights

        // Loop through the grid size
        for (int x = 0; x <= gridSizeX; x++)
        {
            for (int y = 0; y <= gridSizeY; y++)
            {
                // Get neighbors (including edges)
                float totalHeight = heights[x, y];
                int count = 1; // Start with the current vertex

                // Check and add heights of neighbors
                if (x > 0) // Left
                {
                    totalHeight += heights[x - 1, y];
                    count++;
                }
                if (x < gridSizeX) // Right
                {
                    totalHeight += heights[x + 1, y];
                    count++;
                }
                if (y > 0) // Down
                {
                    totalHeight += heights[x, y - 1];
                    count++;
                }
                if (y < gridSizeY) // Up
                {
                    totalHeight += heights[x, y + 1];
                    count++;
                }

                // Calculate the average height
                smoothedHeights[x, y] = totalHeight / count;
            }
        }

        // Copy the smoothed heights back into the original heights array
        for (int x = 0; x <= gridSizeX; x++)
        {
            for (int y = 0; y <= gridSizeY; y++)
            {
                heights[x, y] = smoothedHeights[x, y];
            }
        }
    }

    void CreateMesh()
    {
        Mesh mesh = new Mesh();

        // Create arrays for vertices, UVs, and triangles
        Vector3[] vertices = new Vector3[(gridSizeX + 1) * (gridSizeY + 1)];
        int[] triangles = new int[gridSizeX * gridSizeY * 6]; // 2 triangles per quad
        Vector2[] uv = new Vector2[(gridSizeX + 1) * (gridSizeY + 1)];

        // Populate vertices and UVs
        for (int x = 0; x <= gridSizeX; x++)
        {
            for (int y = 0; y <= gridSizeY; y++)
            {
                int index = x + y * (gridSizeX + 1);
                vertices[index] = new Vector3(x * spacing, heights[x, y], y * spacing);
                uv[index] = new Vector2((float)x / gridSizeX, (float)y / gridSizeY);
            }
        }

        // Populate triangles
        int triangleIndex = 0;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                int topLeft = x + y * (gridSizeX + 1);
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + (gridSizeX + 1);
                int bottomRight = bottomLeft + 1;

                // First triangle
                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topRight;

                // Second triangle
                triangles[triangleIndex++] = topRight;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = bottomRight;
            }
        }

        // Assign the arrays to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        // Recalculate normals for proper lighting
        mesh.RecalculateNormals();

        // Create a GameObject and assign the mesh
        GameObject terrainObject = new GameObject("Terrain");
        MeshFilter meshFilter = terrainObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        terrainObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        // Optional: Set the terrain object as a child of the GridGenerator object for better organization
        terrainObject.transform.parent = transform;
    }
}
