using System;
using System.Linq;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour {
    private MeshGeneration MG;

    [SerializeField] private GameObject chunk;
    private GameObject[] chunks;
    [HideInInspector] public GameObject middleChunk;
    private GameObject newChunk;

    private bool chunksMoved = false;

    /*[HideInInspector] */
    public Vector3[] leftVerts;
    /*[HideInInspector] */
    public Vector3[] topVerts;
    /*[HideInInspector] */
    public Vector3[] rightVerts;
    /*[HideInInspector] */
    public Vector3[] bottomVerts;

    // Start is called before the first frame update
    void Start() {
        chunks = new GameObject[8];

        middleChunk = Instantiate(chunk, Vector3.zero, Quaternion.identity);
        middleChunk.tag = "middleChunk";

        //MG = middleChunk.GetComponent<MeshGeneration>();

        PositionChunk();
    }

    public void PositionChunk() {
        for (int i = 0; i < 8; i++) {
            chunks[i] = Instantiate(chunk, Vector3.zero, Quaternion.identity);
            chunks[i].GetComponent<MeshGeneration>().middleChunk = middleChunk;
            chunks[i].name = $"newChunk{i}";
        }

        for (int i = 0; i < chunks.Length; i++) {
            switch (chunks[i].name) {
                case "newChunk0":
                    chunks[i].tag = "leftChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(-30, 0, 0);
                    break;
                case "newChunk1":
                    chunks[i].tag = "topLeftChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(-30, 0, 30);
                    break;
                case "newChunk2":
                    chunks[i].tag = "topChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(0, 0, 30);
                    break;
                case "newChunk3":
                    chunks[i].tag = "topRightChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(30, 0, 30);
                    break;
                case "newChunk4":
                    chunks[i].tag = "rightChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(30, 0, 0);
                    break;
                case "newChunk5":
                    chunks[i].tag = "bottomRightChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(30, 0, -30);
                    break;
                case "newChunk6":
                    chunks[i].tag = "bottomChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(0, 0, -30);
                    break;
                case "newChunk7":
                    chunks[i].tag = "bottomLeftChunk";
                    chunks[i].transform.position = middleChunk.transform.position + new Vector3(-30, 0, -30);
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update() {


    }
}
