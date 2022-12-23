using UnityEngine;

public class GetPosition : MonoBehaviour {

    [SerializeField] private GameObject player;
    private GameObject previousChunk;
    /*[HideInInspector] */
    public GameObject currentChunk;
    [HideInInspector] public GameObject currentSector;

    [SerializeField] private LayerMask groundCheck;

    [SerializeField] private bool aboveGround;
    private bool newChunk = false;

    // Update is called once per frame
    void Update() {
        GetCurrentChunk();
    }

    /// <summary>
    /// Checks whether the player is currently above a chunk, and then sets the 'current chunk' to that chunk.
    /// Also checks whether the player enters a new chunk and changes the layers accordingly, so that the neighbour detection can work correctly.
    /// </summary>
    void GetCurrentChunk() {

        int layerMask = 1 << 7;

        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, player.transform.TransformDirection(Vector3.down), out hit, 100, groundCheck)) {
            Debug.DrawRay(player.transform.position, player.transform.TransformDirection(Vector3.down) * hit.distance, Color.green);

            aboveGround = true;

            if (currentChunk != null) {
                previousChunk = currentChunk;
            }

            currentChunk = hit.transform.gameObject;
            currentChunk.layer = 18;
            currentChunk.tag = "currentChunk";

            if (previousChunk != currentChunk) {
                previousChunk.layer = 17;
                previousChunk.tag = "Default";
            }
        } else {
            aboveGround = false;
            Debug.DrawRay(player.transform.position, player.transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
        }
    }
}
