using UnityEngine;

public class UpdateTerrain : MonoBehaviour {
    private TerrainGeneration TG;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject currentChunk;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        GetCurrentChunk();
    }

    private Vector3[] vertices;

    private Vector3 botLeftCorner;
    private Vector3 topLeftCorner;
    private Vector3 botRightCorner;
    private Vector3 topRightCorner;
    void GetCurrentChunk() {
        int width;
        int height;

        int layerMask = 1 << 7;

        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, player.transform.TransformDirection(Vector3.down), out hit, 15, layerMask)) {
            Debug.DrawRay(player.transform.position, player.transform.TransformDirection(Vector3.down) * hit.distance, Color.green);

            currentChunk = hit.transform.gameObject;
            vertices = currentChunk.GetComponent<MeshGeneration>().vertices;
            //mesh = currentChunk.GetComponent<MeshFilter>().mesh;
            //vertices = mesh.vertices;

            width = vertices.Length / 2;
            height = vertices.Length / 2;

            botLeftCorner = vertices[0];
            topLeftCorner = vertices[vertices.Length - width];
            botRightCorner = vertices[width];
            topRightCorner = vertices[vertices.Length - 1];
        } else {
            Debug.DrawRay(player.transform.position, player.transform.TransformDirection(Vector3.down) * hit.distance, Color.red);

            Debug.Log("Out of bounds!");
        }
    }

    [SerializeField] private float genDistance;
    void GenerateNewChunks() {
        Vector3 playerPos = player.transform.position;
        float distanceToTop = (playerPos.z - topLeftCorner.z);
        float distanceToBottom = (playerPos.z - botLeftCorner.z);
        float distanceToLeft = (playerPos.x - botLeftCorner.x);
        float distanceToRight = (playerPos.x - botRightCorner.x);

        if (distanceToTop < genDistance) {
            TG.chunkPool.Get();
        }
    }
}
