using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneration : MonoBehaviour {
    private TerrainGeneration TG;
    private MeshGeneration MCMG;

    private UnityEngine.Mesh mesh;

    [SerializeField] private MeshFilter chunkPrefab;
    public GameObject middleChunk;
    private List<MeshFilter> AllMeshFilters = new List<MeshFilter>();

    [SerializeField] private int mapAreaSize;
    [SerializeField] private int xSize;
    [SerializeField] private int zSize;
    private int[] triangles;
    private int meshCount = 4;

    public float sideLength;

    private Vector3[] triangleCentres;

    private Vector3[] vertices;

    [SerializeField] private GameObject vertPrefab;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject currentChunk;

    public bool vertsRepositioned = false;
    void Start() {
        TG = GameObject.Find("TerrainGen").GetComponent<TerrainGeneration>();
        middleChunk = GameObject.FindGameObjectWithTag("middleChunk");
        MCMG = middleChunk.GetComponent<MeshGeneration>();

        xSize = (int)Mathf.Sqrt(mapAreaSize) - 1;
        zSize = (int)Mathf.Sqrt(mapAreaSize) - 1;

        sideLength = xSize + 1;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //for (int i = 0; i < meshCount; i++) {
        //    CreateMesh();
        //    UpdateMesh();
        //}

        CreateMesh();
        UpdateEdgeVerts();
        UpdateMesh();
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

        botLeftCorner = vertices[0];
        topLeftCorner = vertices[vertices.Length - 1 - xSize];
        botRightCorner = vertices[xSize];
        topRightCorner = vertices[vertices.Length - 1];
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

    [HideInInspector] public Vector3 botLeftCorner;
    [HideInInspector] public Vector3 topLeftCorner;
    [HideInInspector] public Vector3 botRightCorner;
    [HideInInspector] public Vector3 topRightCorner;

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

    public Vector3[] botVerts;
    public Vector3[] leftVerts;
    public Vector3[] topVerts;
    public Vector3[] rightVerts;

    //middle chunk side vertices
    public Vector3[] midBotVerts;
    public Vector3[] midLeftVerts;
    public Vector3[] midTopVerts;
    public Vector3[] midRightVerts;

    //top-left chunk side vertices
    public Vector3[] TLBotVerts;
    public Vector3[] TLLeftVerts;
    public Vector3[] TLTopVerts;
    public Vector3[] TLRightVerts;

    //top-right chunk side vertices
    public Vector3[] TRBotVerts;
    public Vector3[] TRLeftVerts;
    public Vector3[] TRTopVerts;
    public Vector3[] TRRightVerts;

    //bottom-right chunk side vertices
    public Vector3[] BRBotVerts;
    public Vector3[] BRLeftVerts;
    public Vector3[] BRTopVerts;
    public Vector3[] BRRightVerts;

    //bottom-left chunk side vertices
    public Vector3[] BLBotVerts;
    public Vector3[] BLLeftVerts;
    public Vector3[] BLTopVerts;
    public Vector3[] BLRightVerts;

    void UpdateEdgeVerts() {

        if (!CompareTag("middleChunk")) {
            midBotVerts = MCMG.botVerts;
            midLeftVerts = MCMG.leftVerts;
            midTopVerts = MCMG.topVerts;
            midRightVerts = MCMG.rightVerts;
        }

        sideLength = xSize + 1;

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

        switch (tag) {
            case "leftChunk":
                //connecting left chunk to middle chunk
                k = 0;
                for (int i = (int)sideLength - 1; i < vertices.Length; i += (int)sideLength) {
                    vertices[i] = midLeftVerts[k];
                    vertices[i] += new Vector3(30, 0, 0);
                    rightVerts[k] = vertices[i];
                    k++;
                }
                break;
            case "topLeftChunk":
                TLBotVerts = botVerts;
                TLLeftVerts = leftVerts;
                TLTopVerts = topVerts;
                TLRightVerts = rightVerts;
                break;
            case "topChunk":
                //connecting top chunk to middle chunk
                for (int i = 0; i < (int)sideLength; i++) {
                    vertices[i] = midTopVerts[i];
                    vertices[i] -= new Vector3(0, 0, 30);
                }
                break;
            case "topRightChunk":
                TRBotVerts = botVerts;
                TRLeftVerts = leftVerts;
                TRTopVerts = topVerts;
                TRRightVerts = rightVerts;
                break;
            case "rightChunk":
                //connecting right chunk to middle chunk
                k = 0;
                for (int i = 0; i < vertices.Length - (int)sideLength + 1; i += (int)sideLength) {
                    vertices[i] = midRightVerts[k];
                    k++;
                }
                break;
            case "bottomRightChunk":
                BRBotVerts = botVerts;
                BRLeftVerts = leftVerts;
                BRTopVerts = topVerts;
                BRRightVerts = rightVerts;
                break;
            case "bottomChunk":
                //connecting bottom chunk to middle chunk
                break;
            case "bottomLeftChunk":
                BLBotVerts = botVerts;
                BLLeftVerts = leftVerts;
                BLTopVerts = topVerts;
                BLRightVerts = rightVerts;
                break;
        }

        vertsRepositioned = true;
    }

    private void OnDrawGizmos() {
        if (vertices == null) {
            return;
        }

        for (int i = 0; i < vertices.Length; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
}
