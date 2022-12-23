using System.Collections.Generic;
using UnityEngine;

public class MeshGeneration : MonoBehaviour {
    private TerrainGeneration TG;
    private MeshGeneration MCMG;

    private Mesh mesh;

    [SerializeField] private MeshFilter chunkPrefab;
    public GameObject middleChunk;
    private List<MeshFilter> AllMeshFilters = new List<MeshFilter>();

    [SerializeField] private int mapAreaSize;
    [SerializeField] private int xSize;
    [SerializeField] private int zSize;
    private int[] triangles;

    public float sideLength;

    public Vector3[] vertices;

    [SerializeField] GameObject terrain;

    [SerializeField] private GameObject vertPrefab;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject currentChunk;

    public bool vertsRepositioned = false;
    void Start() {
        terrain = transform.parent.gameObject;

        TG = GameObject.Find("TerrainGen").GetComponent<TerrainGeneration>();
        xSize = (int)Mathf.Sqrt(mapAreaSize) - 1;
        zSize = (int)Mathf.Sqrt(mapAreaSize) - 1;

        sideLength = xSize + 1;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    [HideInInspector] public bool created = false;
    private void Update() {

        if (middleChunk == null) {
            middleChunk = GameObject.FindGameObjectWithTag("middleChunk");
        } else {
            if (!created) {
                CreateMesh();
                UpdateEdgeVerts();
                UpdateMesh();
                created = true;
            }
        }
    }

    /// <summary>
    /// Generates a grid of slightly randomized vertices.
    /// Also gets and stores the vertices in each corner, in their own separate variable.
    /// Using the vertices, a list of ints that represent the triangles for the mesh is created.
    /// </summary>
    public void CreateMesh() {

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

    /// <summary>
    /// Clears and updates the mesh on the chunk by assigning the randomly generated vertices and triangles
    /// generated in the CreateMesh function.
    /// </summary>
    public void UpdateMesh() {
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

    //side vertices for this chunk
    public Vector3[] botVerts;
    public Vector3[] leftVerts;
    public Vector3[] topVerts;
    public Vector3[] rightVerts;

    //middle chunk side vertices
    public Vector3[] midBotVerts;
    public Vector3[] midLeftVerts;
    public Vector3[] midTopVerts;
    public Vector3[] midRightVerts;

    //left chunk side vertices
    public Vector3[] leftTopVerts;
    public Vector3[] leftBotVerts;

    //right chunk side vertices
    public Vector3[] rightTopVerts;
    public Vector3[] rightBotVerts;

    //top chunk side vertices
    public Vector3[] topLeftVerts;
    public Vector3[] topRightVerts;

    //bottom chunk side vertices
    public Vector3[] botLeftVerts;
    public Vector3[] botRightVerts;

    /// <summary>
    /// Finds and stores the edge vertices for each chunk, in their own list.
    /// Then appropriately updates the vertices on the neighbouring chunks, so that they appear to connect.
    /// </summary>
    public void UpdateEdgeVerts() {
        MCMG = middleChunk.GetComponent<MeshGeneration>();
        GameObject leftChunk = terrain.transform.GetChild(1).gameObject;
        GameObject topChunk = terrain.transform.GetChild(3).gameObject;
        GameObject rightChunk = terrain.transform.GetChild(5).gameObject;
        GameObject bottomChunk = terrain.transform.GetChild(7).gameObject;

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

        leftTopVerts = new Vector3[(int)sideLength];
        leftBotVerts = new Vector3[(int)sideLength];

        rightTopVerts = new Vector3[(int)sideLength];
        rightBotVerts = new Vector3[(int)sideLength];

        topLeftVerts = new Vector3[(int)sideLength];
        topRightVerts = new Vector3[(int)sideLength];

        //Storing bottom vertices
        for (int i = 0; i < (int)sideLength; i++) {
            botVerts[i] = vertices[i];
        }

        //storing left vertices
        int k = 0;
        for (int i = 0; i < vertices.Length - (int)sideLength + 1; i += (int)sideLength) {
            leftVerts[k] = vertices[i];
            k++;
        }

        //storing top vertices
        k = 0;
        for (int i = vertices.Length - (int)sideLength; i < vertices.Length; i++) {
            topVerts[k] = vertices[i];
            k++;
        }

        //storing right vertices
        k = 0;
        for (int i = (int)sideLength - 1; i < vertices.Length; i += (int)sideLength) {
            rightVerts[k] = vertices[i];
            k++;
        }

        switch (tag) {
            case "topChunk":
                topLeftVerts = leftVerts;
                topRightVerts = rightVerts;
                break;
            case "bottomChunk":
                botLeftVerts = leftVerts;
                botRightVerts = rightVerts;
                break;
            case "leftChunk":
                leftTopVerts = topVerts;
                leftBotVerts = botVerts;
                break;
            case "rightChunk":
                rightTopVerts = topVerts;
                rightBotVerts = botVerts;
                break;
            case "topLeftChunk":
                topLeftVerts = topChunk.GetComponent<MeshGeneration>().leftVerts;
                leftTopVerts = leftChunk.GetComponent<MeshGeneration>().topVerts;
                break;
            case "topRightChunk":
                topRightVerts = topChunk.GetComponent<MeshGeneration>().rightVerts;
                rightTopVerts = rightChunk.GetComponent<MeshGeneration>().topVerts;
                break;
            case "bottomRightChunk":
                rightBotVerts = rightChunk.GetComponent<MeshGeneration>().botVerts;
                botRightVerts = bottomChunk.GetComponent<MeshGeneration>().rightVerts;
                break;
            case "bottomLeftChunk":
                botLeftVerts = bottomChunk.GetComponent<MeshGeneration>().leftVerts;
                leftBotVerts = leftChunk.GetComponent<MeshGeneration>().botVerts;
                break;
        }

        switch (tag) {
            case "leftChunk":
                //Connecting left chunk to middle chunk
                k = 0;
                for (int i = (int)sideLength - 1; i < vertices.Length; i += (int)sideLength) {
                    vertices[i] = midLeftVerts[k];
                    vertices[i] += new Vector3(30, 0, 0);
                    k++;
                }
                break;
            case "topLeftChunk":
                //Connecting top-left chunk to left and top chunks
                k = 0;
                for (int i = (int)sideLength - 1; i < vertices.Length; i += (int)sideLength) {
                    vertices[i] = topLeftVerts[k];
                    vertices[i] += new Vector3(30, 0, 0);
                    k++;
                }

                for (int i = 0; i < (int)sideLength; i++) {
                    vertices[i] = leftTopVerts[i];
                    vertices[i] -= new Vector3(0, 0, 30);
                }
                break;
            case "topChunk":
                //Connecting top chunk to middle chunk
                for (int i = 0; i < (int)sideLength; i++) {
                    vertices[i] = midTopVerts[i];
                    vertices[i] -= new Vector3(0, 0, 30);
                }
                break;
            case "topRightChunk":
                //Connecting top-right chunk to top and right chunks
                k = 0;
                for (int i = 0; i < vertices.Length - (int)sideLength + 1; i += (int)sideLength) {
                    vertices[i] = topRightVerts[k];
                    vertices[i] -= new Vector3(30, 0, 0);
                    k++;
                }

                for (int i = 0; i < (int)sideLength; i++) {
                    vertices[i] = rightTopVerts[i];
                    vertices[i] -= new Vector3(0, 0, 30);
                }
                break;
            case "rightChunk":
                //Connecting right chunk to middle chunk
                k = 0;
                for (int i = 0; i < vertices.Length - (int)sideLength + 1; i += (int)sideLength) {
                    vertices[i] = midRightVerts[k];
                    vertices[i] -= new Vector3(30, 0, 0);
                    k++;
                }
                break;
            case "bottomRightChunk":
                //Connecting bottom-right chunk to right and bottom chunks
                k = 0;
                for (int i = vertices.Length - (int)sideLength; i < vertices.Length; i++) {
                    vertices[i] = rightBotVerts[k];
                    vertices[i] += new Vector3(0, 0, 30);
                    k++;
                }

                k = 0;
                for (int i = 0; i < vertices.Length - (int)sideLength; i += (int)sideLength) {
                    vertices[i] = botRightVerts[k];
                    vertices[i] -= new Vector3(30, 0, 0);
                    k++;
                }
                break;
            case "bottomChunk":
                //Connecting bottom chunk to middle chunk
                k = 0;
                for (int i = vertices.Length - (int)sideLength; i < vertices.Length; i++) {
                    vertices[i] = midBotVerts[k];
                    vertices[i] += new Vector3(0, 0, 30);
                    k++;
                }
                break;
            case "bottomLeftChunk":
                //Connecting bottom-left chunk to bottom and left chunks
                k = 0;
                for (int i = (int)sideLength - 1; i < vertices.Length; i += (int)sideLength) {
                    vertices[i] = botLeftVerts[k];
                    vertices[i] += new Vector3(30, 0, 0);
                    k++;
                }

                k = 0;
                for (int i = vertices.Length - (int)sideLength; i < vertices.Length; i++) {
                    vertices[i] = leftBotVerts[k];
                    vertices[i] += new Vector3(0, 0, 30);
                    k++;
                }
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
