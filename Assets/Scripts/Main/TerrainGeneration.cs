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
        chunks = new GameObject[7];

        newChunk = Instantiate(chunk, Vector3.zero, Quaternion.identity);
        newChunk.tag = "middleChunk";

        middleChunk = GameObject.FindGameObjectWithTag("middleChunk");
        MG = middleChunk.GetComponent<MeshGeneration>();

        PositionChunk();
    }

    public void PositionChunk() {
        for (int i = 0; i < 7; i++) {
            chunks[i] = Instantiate(chunk, Vector3.zero, Quaternion.identity);
            chunks[i].name = $"newChunk{i}";
        }
    }

    // Update is called once per frame
    void Update() {

        if (!chunksMoved) {
            for (int i = 0; i < chunks.Length; i++) {
                switch (chunks[i].name) {
                    case "newChunk0":
                        newChunk.tag = "leftChunk";
                        newChunk.transform.position = middleChunk.transform.position + new Vector3(-30, 0, 0);
                        break;
                    case "newChunk1":
                        newChunk.tag = "topLeftChunk";
                        newChunk.transform.position = middleChunk.transform.position + new Vector3(-30, 0, 30);
                        break;
                    case "newChunk2":
                        newChunk.tag = "topChunk";
                        newChunk.transform.position = middleChunk.transform.position + new Vector3(0, 0, 30);
                        break;
                    case "newChunk3":
                        newChunk.tag = "topRightChunk";
                        newChunk.transform.position = middleChunk.transform.position + new Vector3(30, 0, 30);
                        break;
                    case "newChunk4":
                        newChunk.tag = "rightChunk";
                        newChunk.transform.position = middleChunk.transform.position + new Vector3(30, 0, 0);
                        break;
                    case "newChunk5":
                        newChunk.tag = "bottomRightChunk";
                        newChunk.transform.position = middleChunk.transform.position + new Vector3(30, 0, -30);
                        break;
                    case "newChunk6":
                        newChunk.tag = "bottomChunk";
                        newChunk.transform.position = middleChunk.transform.position + new Vector3(0, 0, -30);
                        break;
                    case "newChunk7":
                        newChunk.tag = "bottomLeftChunk";
                        newChunk.transform.position = middleChunk.transform.position + new Vector3(-30, 0, -30);
                        break;
                }
            }
            chunksMoved = true;
        }
    }
}
