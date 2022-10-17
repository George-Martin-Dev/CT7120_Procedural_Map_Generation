using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;

public class GenerateVertices : MonoBehaviour
{
    private Mesh mesh;

    [SerializeField] private int gridSize = 25;
    [SerializeField] private List<Vector3> vertPositions;
    [SerializeField] private int[] triangles;

    [SerializeField] private int mapAreaSize;

    [SerializeField] private GameObject vertPrefab;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateVerts();
        GenerateTris();
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateVerts() {       
        const float jitter = 0.5f;

        System.Random rng = new System.Random();

        for (int x = 0; x <= gridSize - 1; x++) {
            for (int z = 0; z <= gridSize - 1; z++) {
                float newX = x + jitter * (float)(rng.NextDouble() - rng.NextDouble());
                float newZ = z + jitter * (float)(rng.NextDouble() - rng.NextDouble());

                vertPositions.Add(new Vector3(newX, 0, newZ));
            }
        }
    }

    private void GenerateTris() {
        triangles = new int[gridSize * gridSize * 6];

        int t = 0;
        int v = 0;

        for (int z = 0; z < gridSize; z++) {
            for (int x = 0; x < gridSize; x++) {
                triangles[t + 0] = v + 0;
                triangles[t + 1] = v + x + 1;
                triangles[t + 2] = v + 1;
                triangles[t + 3] = v + 1;
                triangles[t + 4] = v + x + 1;
                triangles[t + 5] = v + x + 2;

                v++;
                t += 6;
            }
            v++;
        }
    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = vertPositions.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos() {
        if (vertPositions == null) {
            return;
        }

        for (int i = 0; i < vertPositions.Count; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(vertPositions[i], .1f);
        }
    }
}
