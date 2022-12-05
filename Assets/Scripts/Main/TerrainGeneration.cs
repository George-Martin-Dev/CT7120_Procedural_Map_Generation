using UnityEngine;
using UnityEngine.Pool;

public class TerrainGeneration : MonoBehaviour {
    [SerializeField] private GameObject chunk;
    private GameObject[] chunks;
    [HideInInspector] public GameObject middleChunk;

    [SerializeField] Chunk chunkPrefab;

    [HideInInspector] public ObjectPool<Chunk> chunkPool;

    private GameObject[,] TerrainSegments;

    public bool collectionChecks = true;
    public int maxPoolSize = 9;

    private void Awake() {
        chunkPool = new ObjectPool<Chunk>(CreatePooledObject, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false, 200, 100_000);
    }

    // Start is called before the first frame update
    void Start() {
        chunks = new GameObject[9];


        //middleChunk = Instantiate(chunk, Vector3.zero, Quaternion.identity);
        //middleChunk.tag = "middleChunk";

        //MG = middleChunk.GetComponent<MeshGeneration>();
        for (chunkNo = 0; chunkNo < chunks.Length; chunkNo++) {
            chunkPool.Get();
        }
        PositionChunk();
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
    public void spawnChunk(Chunk chunk) {
        chunks[chunkNo] = chunk.gameObject;

        if (chunkNo == 0) {
            middleChunk = chunk.gameObject;
            middleChunk.tag = "middleChunk";
        }

        chunk.gameObject.layer = 7;
        chunk.GetComponent<MeshGeneration>().middleChunk = middleChunk;
        chunk.name = $"newChunk{chunkNo}";
    }

    public void PositionChunk() {
        for (int i = 0; i < chunks.Length; i++) {
            switch (chunks[i].name) {
                case "newChunk1":
                    chunks[i].tag = "leftChunk";
                    chunks[i].layer = 9;
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(-30, 0, 0);
                    break;
                case "newChunk2":
                    chunks[i].tag = "topLeftChunk";
                    chunks[i].layer = 10;
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(-30, 0, 30);
                    break;
                case "newChunk3":
                    chunks[i].tag = "topChunk";
                    chunks[i].layer = 11;
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(0, 0, 30);
                    break;
                case "newChunk4":
                    chunks[i].tag = "topRightChunk";
                    chunks[i].layer = 12;
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(30, 0, 30);
                    break;
                case "newChunk5":
                    chunks[i].tag = "rightChunk";
                    chunks[i].layer = 13;
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(30, 0, 0);
                    break;
                case "newChunk6":
                    chunks[i].tag = "bottomRightChunk";
                    chunks[i].layer = 14;
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(30, 0, -30);
                    break;
                case "newChunk7":
                    chunks[i].tag = "bottomChunk";
                    chunks[i].layer = 15;
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(0, 0, -30);
                    break;
                case "newChunk8":
                    chunks[i].tag = "bottomLeftChunk";
                    chunks[i].layer = 16;
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(-30, 0, -30);
                    break;
            }
        }
    }
}

