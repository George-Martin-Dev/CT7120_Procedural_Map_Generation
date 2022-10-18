using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateVertices : MonoBehaviour {
    private Mesh mesh;

    [SerializeField] private int xSize;
    [SerializeField] private int zSize;
    private int[] triangles;

    private Vector3[] triangleCentres;

    private Vector3[] vertices;
    

    [SerializeField] private int mapAreaSize;

    [SerializeField] private GameObject vertPrefab;

    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        StartCoroutine(CreateShape());
    }

    // Update is called once per frame
    void Update() {
        UpdateMesh();
    }

    private IEnumerator CreateShape() {

        const float jitter = 0.5f;

        System.Random rng = new System.Random();

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int z = 0, i = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                float newX = x + jitter * (float)(rng.NextDouble() - rng.NextDouble());
                float newZ = z + jitter * (float)(rng.NextDouble() - rng.NextDouble());
                float newY = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;

                //vertices[i] = new Vector3(x, 0, z);
                vertices[i] = new Vector3(newX, newY, newZ);
                //Instantiate(vertPrefab, vertices[i], Quaternion.identity);
                i++;
            }
        }

        triangleCentres = new Vector3[vertices.Length / 3];

        for (int z = 0; z < zSize; z++) {
            for (int x = 0; x < xSize; x++) {
                
            }
        }

        triangles = new int[xSize * zSize * 6];        

        int t = 0;
        int v = 0;

        for (int z = 0; z < zSize; z++) {
            for (int x = 0; x < xSize; x++) {
                triangles[t + 0] = v + 0;
                triangles[t + 1] = v + xSize + 1;
                triangles[t + 2] = v + 1;
                triangles[t + 3] = v + 1;
                triangles[t + 4] = v + xSize + 1;
                triangles[t + 5] = v + xSize + 2;

                v++;
                t += 6;

                yield return new WaitForSeconds(.1f);
            }
            v++;
        }   
    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        mesh.RecalculateBounds();
        MeshCollider collider = gameObject.GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }


    private void OnDrawGizmos() {
        if (vertices == null) {
            return;
        }

        for (int i = 0; i < vertices.Length; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(vertices[i], .1f);
        }

        if (triangleCentres == null) {
            return;
        }

        for (int i = 0; i < triangleCentres.Length; i++) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(triangleCentres[i], .1f);
        }
    }
}
