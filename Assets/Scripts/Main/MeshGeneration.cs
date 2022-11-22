using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneration : MonoBehaviour {
    private UnityEngine.Mesh mesh;

    [SerializeField] private MeshFilter chunkPrefab;
    private List<MeshFilter> AllMeshFilters = new List<MeshFilter>();

    [SerializeField] private int mapAreaSize;
    [SerializeField] private int xSize;
    [SerializeField] private int zSize;
    private int[] triangles;
    private int meshCount = 4;

    private Vector3[] triangleCentres;

    private Vector3[] vertices;

    [SerializeField] private GameObject vertPrefab;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject currentChunk;

    void Start() {
        xSize = (int)Mathf.Sqrt(mapAreaSize) - 1;
        zSize = (int)Mathf.Sqrt(mapAreaSize) - 1;

        mesh = new UnityEngine.Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //for (int i = 0; i < meshCount; i++) {
        //    CreateMesh();
        //    UpdateMesh();
        //}

        CreateMesh();
        UpdateMesh();
        GetEdgeVerts();
    }

    private void CreateMesh() {

        const float jitter = 0.5f;

        System.Random rng = new System.Random();

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int z = 0, i = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                float newX = x + jitter * (float)(rng.NextDouble() - rng.NextDouble());
                float newZ = z + jitter * (float)(rng.NextDouble() - rng.NextDouble());
                float newY = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;

                vertices[i] = new Vector3(newX, newY, newZ);
                i++;
            }
        }

        triangleCentres = new Vector3[vertices.Length / 3];

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

    void NewUpdateMesh() {
        MeshFilter filter = Instantiate(chunkPrefab);

        UnityEngine.Mesh mesh = filter.mesh;
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        filter.gameObject.SetActive(false);
        AllMeshFilters.Add(filter);
    }

    private Vector3 botLeftCorner;
    private Vector3 topLeftCorner;
    private Vector3 botRightCorner;
    private Vector3 topRightCorner;

    void GetCurrentChunk() {
        UnityEngine.Mesh mesh;

        Vector3[] vertices;

        int width;
        int height;

        int layerMask = 1 << 6;

        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, player.transform.TransformDirection(Vector3.down), out hit, 5, layerMask)) {
            Debug.DrawRay(player.transform.position, player.transform.TransformDirection(Vector3.down) * hit.distance, Color.green);

            currentChunk = hit.transform.gameObject;

            mesh = currentChunk.GetComponent<MeshFilter>().mesh;
            vertices = mesh.vertices;

            width = vertices.Length / 2;
            height = vertices.Length / 2;

            botLeftCorner = vertices[0];
            topLeftCorner = vertices[width * height - width];
            botRightCorner = vertices[width];
            topRightCorner = vertices[vertices.Length];
        } else {
            Debug.DrawRay(player.transform.position, player.transform.TransformDirection(Vector3.down) * hit.distance, Color.red);

            Debug.Log("Out of bounds!");
        }
    }

    float GetSqrMagFromEdge(Vector3 vertex1, Vector3 vertex2, Vector3 point) {
        float n = Vector3.Cross(point - vertex1, point - vertex2).sqrMagnitude;
        return n / (vertex1 - vertex2).sqrMagnitude;
    }

    private List<float> disFromEdges;
    private Vector3 vertex1;
    private Vector3 vertex2;

    void FindClosestEdge() {
        for (int i = 0; i < 3; i++) {
            switch (i) {
                case 0:
                    vertex1 = botLeftCorner;    //] Bottom Edge
                    vertex2 = botRightCorner;   //] 
                    break;
                case 1:
                    vertex1 = botLeftCorner;    //] Left Edge
                    vertex2 = topLeftCorner;    //]
                    break;
                case 2:
                    vertex1 = topLeftCorner;    //] Top Edge
                    vertex2 = topRightCorner;   //]
                    break;
                case 3:
                    vertex1 = topRightCorner;   //] Right Edge
                    vertex2 = botRightCorner;   //]
                    break;
            }

            disFromEdges.Add(GetSqrMagFromEdge(vertex1, vertex2, player.transform.position));
        }

        int smallestDisIndex = disFromEdges.IndexOf(disFromEdges.Min());

        switch (smallestDisIndex) {
            case 0:

                break;
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
        }
    }

    [SerializeField] private Vector3[] botVerts;
    [SerializeField] private Vector3[] leftVerts;
    [SerializeField] private Vector3[] topVerts;
    [SerializeField] private Vector3[] rightVerts;
    void GetEdgeVerts() {
        UnityEngine.Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        float sideLength = xSize + 1;

        botVerts = new Vector3[(int)sideLength];
        leftVerts = new Vector3[(int)sideLength];
        topVerts = new Vector3[(int)sideLength];
        rightVerts = new Vector3[(int)sideLength];

        //Storing bottom vertices
        for (int i = 0; i < (int)sideLength; i++) {
            botVerts[i] = vertices[i];
        }

        //storing left vertices
        int j = 0;
        int k = 0;
        for (int i = 0; i < vertices.Length - (int)sideLength + 1; i++) {

            if (i == j) {
                j += (int)sideLength;
                leftVerts[k] = vertices[i];
                k++;
            }
        }

        j = 0;
        //storing top vertices
        for (int i = vertices.Length - (int)sideLength; i < vertices.Length; i++) {
            topVerts[j] = vertices[i];
            j++;
        }

        //storing right vertices
        j = (int)sideLength - 1;
        k = 0;
        for (int i = (int)sideLength - 1; i < vertices.Length; i++) {
            if (i == j) {
                j += (int)sideLength;
                rightVerts[k] = vertices[i];
                k++;
            }

        }
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
