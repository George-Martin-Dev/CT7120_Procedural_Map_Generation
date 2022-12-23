using UnityEngine;
using UnityEngine.Pool;
using System;

public class TerrainGeneration : MonoBehaviour {
    [SerializeField] private GetPosition GP;

    [SerializeField] private Transform terrain;
    [SerializeField] private Transform terrainPrefab;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject chunk;
    private GameObject[] chunks;
    [HideInInspector] public GameObject middleChunk;

    [SerializeField] Chunk chunkPrefab;

    [HideInInspector] public ObjectPool<Chunk> chunkPool;

    [SerializeField] private float genDistance;

    public bool collectionChecks = true;
    private void Awake() {
        player = GameObject.Find("Player");

        chunkPool = new ObjectPool<Chunk>(CreatePooledObject, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false, 200, 10000);
    }

    // Start is called before the first frame update
    void Start() {
        chunks = new GameObject[10000];

        for (chunkNo = 0; chunkNo < 9; chunkNo++) {
            chunkPool.Get();
        }
        PositionChunk();
    }

    private void Update() {
        if (terrain != null) {
            GetDistanceFromEdge();
        }
    }

    void OnTakeFromPool(Chunk chunk) {
        chunk.gameObject.SetActive(true);
        spawnChunk(chunk);
    }

    void OnReturnedToPool(Chunk chunk) {
        chunk.gameObject.SetActive(false);
    }

    private Chunk CreatePooledObject() {
        Chunk instance = Instantiate(chunkPrefab, Vector3.zero, Quaternion.identity);
        instance.Disable += ReturnObjectToPool;
        instance.gameObject.SetActive(false);

        return instance;
    }

    private void ReturnObjectToPool(Chunk chunk) {
        chunkPool.Release(chunk);
    }

    void OnDestroyPoolObject(Chunk chunk) {
        Destroy(chunk);
    }

    private int chunkNo;

    /// <summary>
    /// Assigns various values that are unique to each chunk segment
    /// </summary>
    /// <param name="chunk"></param>
    public void spawnChunk(Chunk chunk) {
        chunks[chunkNo] = chunk.gameObject;

        if (chunkNo == 0) {
            chunk.tag = "middleChunk";
            middleChunk = chunk.gameObject;
            middleChunk.tag = "middleChunk";
        }

        chunk.gameObject.layer = 7;
        chunk.GetComponent<MeshGeneration>().middleChunk = middleChunk;
        chunk.name = $"newChunk{chunkNo}";
    }

    [SerializeField] private char genDir;
    public bool generated = false;
    [SerializeField] private Vector3[] verts;
    [SerializeField] private float lowestX;
    [SerializeField] private float lowestZ;
    [SerializeField] private float highestX;
    [SerializeField] private float highestZ;
    [SerializeField] private float[] distances = new float[3];

    /// <summary>
    /// Attempts to get the distance between the player and each side of the chunk they are currently above.
    /// Calls the functions for generating and positioning new chunks, when the player gets close enough to an edge.
    /// </summary>
    void GetDistanceFromEdge() {
        float topPoint = GP.currentChunk.transform.position.z + 60;
        float botPoint = GP.currentChunk.transform.position.z - 30;
        float leftPoint = GP.currentChunk.transform.position.x - 30;
        float rightPoint = GP.currentChunk.transform.position.x + 60;

        distances[0] = topPoint - player.transform.position.z; //distance from top
        distances[1] = botPoint - player.transform.position.z;  //distance from bottom
        distances[2] = leftPoint - player.transform.position.x;  //distance from left
        distances[3] = rightPoint - player.transform.position.x; //distance from right

        ForcePos.pos(distances);

        CheckForNeighbours checkForNeighbours = GP.currentChunk.GetComponent<CheckForNeighbours>();

        if (!generated) {
            if (distances[0] < genDistance) {
                genDir = 'T';
                if (!checkForNeighbours.TopGen) {
                    GenerateChunks();
                    PositionChunk();
                }
            }

            if (distances[1] < genDistance) {
                genDir = 'B';
                if (!checkForNeighbours.BotGen) {
                    GenerateChunks();
                    PositionChunk();
                }
            }

            if (distances[2] < genDistance) {
                genDir = 'L';
                if (!checkForNeighbours.LeftGen) {
                    GenerateChunks();
                    PositionChunk();
                }
            }

            if (distances[3] < genDistance) {
                genDir = 'R';
                if (!checkForNeighbours.RightGen) {
                    GenerateChunks();
                    PositionChunk();
                }
            }

        } else {
            switch (genDir) {
                case 'T':
                    if (distances[0] > genDistance + 2) {
                        generated = false;
                    }
                    break;
                case 'B':
                    if (distances[1] > genDistance + 2) {
                        generated = false;
                    }
                    break;
                case 'L':
                    if (distances[2] > genDistance + 2) {
                        generated = false;
                    }
                    break;
                case 'R':
                    if (distances[3] > genDistance + 2) {
                        generated = false;
                    }
                    break;
            }
        }
    }

    private int layerMask = 1 << 17;
    [SerializeField] private bool occupied = false;
    /// <summary>
    /// Checks to see if there is already a chunk generated on the side of the chunk, that the player is closest to.
    /// </summary>
    /// <returns>true/false</returns>
    private bool CheckForNeighbour() {
        RaycastHit hit;

        switch (genDir) {
            case 'T':
                //Debug.DrawRay(new Vector3(terrain.position.x, 0, highestZ), transform.TransformDirection(Vector3.forward) * 50, Color.red, 30);
                if (Physics.Raycast(new Vector3(terrain.position.x, 0, highestZ - 5), transform.TransformDirection(Vector3.forward), out hit, 50, layerMask)) {
                    return true;
                    occupied = true;
                    generated = true;
                } else {
                    occupied = false;
                }
                break;
            case 'B':
                //Debug.DrawRay(new Vector3(terrain.position.x, 0, lowestZ), transform.TransformDirection(Vector3.back) * 50, Color.red, 30);
                if (Physics.Raycast(new Vector3(terrain.position.x, 0, lowestZ + 5), transform.TransformDirection(Vector3.back), out hit, 50, layerMask)) {
                    return true;
                    occupied = true;
                } else {
                    occupied = false;
                    generated = true;
                }
                break;
            case 'L':
                //Debug.DrawRay(new Vector3(lowestX, 0, terrain.position.z), transform.TransformDirection(Vector3.left) * 50, Color.red, 30);
                if (Physics.Raycast(new Vector3(lowestX + 5, 0, terrain.position.z), transform.TransformDirection(Vector3.left), out hit, 50, layerMask)) {
                    return true;
                    occupied = true;
                } else {
                    occupied = false;
                    generated = true;
                }
                break;
            case 'R':
                //Debug.DrawRay(new Vector3(highestX, 0, terrain.position.z), transform.TransformDirection(Vector3.right) * 50, Color.red, 30);
                if (Physics.Raycast(new Vector3(highestX - 5, 0, terrain.position.z), transform.TransformDirection(Vector3.right), out hit, 50, layerMask)) {
                    return true;
                    occupied = true;
                } else {
                    occupied = false;
                    generated = true;
                }
                break;
        }

        return false;
    }

    /// <summary>
    /// Spawns the segments that make up each chunk and adds them to the object pool.
    /// </summary>
    void GenerateChunks() {
        for (chunkNo = 0; chunkNo < 9; chunkNo++) {
            chunkPool.Get();
        }
        generated = true;
    }

    /// <summary>
    /// Responsible for positioning the segments that make up the chunks, and the chunks themselves.
    /// Also sets the parent of the segments to the chunk that the player is currently above.
    /// </summary>
    public void PositionChunk() {

        for (int i = 0; i < 9; i++) {

            if (i == 0) {
                terrain = Instantiate(terrainPrefab, Vector3.zero, Quaternion.identity);
                terrain.gameObject.layer = 17;
                switch (genDir) {
                    case 'T':
                        terrain.position = GP.currentChunk.transform.position + new Vector3(0, 0, 90);
                        break;
                    case 'B':
                        terrain.position = GP.currentChunk.transform.position - new Vector3(0, 0, 90);
                        break;
                    case 'L':
                        terrain.position = GP.currentChunk.transform.position - new Vector3(90, 0, 0);
                        break;
                    case 'R':
                        terrain.position = GP.currentChunk.transform.position + new Vector3(90, 0, 0);
                        break;
                    default:
                        Debug.Log("First Gen");
                        break;
                }
            }

            chunks[i].transform.parent = terrain;
            Debug.Log("Reached");

            switch (chunks[i].name) {
                case "newChunk1":
                    chunks[i].tag = "leftChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(-30, 0, 0);
                    break;
                case "newChunk2":
                    chunks[i].tag = "topLeftChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(-30, 0, 30);
                    break;
                case "newChunk3":
                    chunks[i].tag = "topChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(0, 0, 30);
                    break;
                case "newChunk4":
                    chunks[i].tag = "topRightChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(30, 0, 30);
                    break;
                case "newChunk5":
                    chunks[i].tag = "rightChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(30, 0, 0);
                    break;
                case "newChunk6":
                    chunks[i].tag = "bottomRightChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(30, 0, -30);
                    break;
                case "newChunk7":
                    chunks[i].tag = "bottomChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(0, 0, -30);
                    break;
                case "newChunk8":
                    chunks[i].tag = "bottomLeftChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(-30, 0, -30);
                    break;
            }
        }
    }
}

