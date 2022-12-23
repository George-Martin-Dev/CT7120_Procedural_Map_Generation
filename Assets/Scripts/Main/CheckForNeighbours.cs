using UnityEngine;

public class CheckForNeighbours : MonoBehaviour {
    public bool TopGen = false;
    public bool BotGen = false;
    public bool LeftGen = false;
    public bool RightGen = false;

    private bool topChecked = false;
    private bool botChecked = false;
    private bool leftChecked = false;
    private bool rightChecked = false;

    private bool topJoined = false;
    private bool botJoined = false;
    private bool leftJoined = false;
    private bool rightJoined = false;

    [HideInInspector] public GameObject neighbour = null;

    void Update() {
        NeighbourCheck();
        JoinNeighbours();
    }

    void NeighbourCheck() {
        Vector3[] verts = GetComponent<MeshFilter>().mesh.vertices;

        float lowestX = Mathf.Infinity;
        float lowestZ = Mathf.Infinity;
        float highestX = Mathf.NegativeInfinity;
        float highestZ = Mathf.NegativeInfinity;

        int i = 0;
        while (i < verts.Length) {
            if (verts[i].x < lowestX) {
                lowestX = verts[i].x;
            }

            if (verts[i].z < lowestZ) {
                lowestZ = verts[i].z;
            }

            if (verts[i].x > highestX) {
                highestX = verts[i].x;
            }

            if (verts[i].z > highestZ) {
                highestZ = verts[i].z;
            }
            i++;
        }

        int neighbours = 1 << 17;

        RaycastHit hit;

        if (!topChecked && gameObject.layer == 18) {
            if (Physics.Raycast(new Vector3(transform.position.x, 1, highestZ - 5), Vector3.forward, out hit, 50, neighbours)) {
                Debug.DrawRay(new Vector3(transform.position.x, 1, highestZ - 5), transform.TransformDirection(Vector3.forward) * 50, Color.black, 30);
                TopGen = true;
                topChecked = true;
                neighbour = hit.transform.gameObject;
            } else {
                Debug.DrawRay(new Vector3(transform.position.x, 1, highestZ - 5), transform.TransformDirection(Vector3.forward) * 50, Color.blue, 30);
                TopGen = false;
            }
        }


        if (!botChecked) {
            if (Physics.Raycast(new Vector3(transform.position.x, 0, lowestZ), Vector3.back, out hit, 50, neighbours)) {
                BotGen = true;
                botChecked = true;
            } else {
                BotGen = false;
            }
        }

        if (!rightChecked) {
            if (Physics.Raycast(new Vector3(highestX, 0, transform.position.z), Vector3.right, out hit, 50, neighbours)) {
                RightGen = true;
                rightChecked = true;
            } else {
                RightGen = false;
            }
        }

        if (!leftChecked) {
            if (Physics.Raycast(new Vector3(lowestX, 0, transform.position.z), Vector3.left, out hit, 50, neighbours)) {
                LeftGen = true;
                leftChecked = true;
            } else {
                LeftGen = false;
            }
        }
    }

    private Vector3[] vertsToChange;
    private Vector3[] neighbourVerts;
    void JoinNeighbours() {
        vertsToChange = GetComponent<MeshFilter>().mesh.vertices;

        if (neighbour != null) {
            neighbourVerts = neighbour.GetComponent<MeshFilter>().mesh.vertices;
        }

        int k = 0;

        if (!topJoined) {
            k = 8010;
            if (TopGen) {
                for (int i = 0; i < 90; i++) {
                    vertsToChange[k] = neighbourVerts[i];
                    k++;
                }
            }
            UpdateMesh();
            topJoined = true;
        }

        if (!botJoined) {
            k = 8010;
            if (BotGen) {
                for (int i = 0; i < 90; i++) {
                    vertsToChange[i] = neighbourVerts[k];
                    k++;
                }
            }
            UpdateMesh();
            botJoined = true;
        }

        if (!leftJoined) {
            k = 90;
            if (LeftGen) {
                for (int i = 0; i < 90; i += 90) {
                    vertsToChange[i] = neighbourVerts[k];
                    k += 90;
                }
            }
            UpdateMesh();
            leftJoined = true;
        }

        if (!rightJoined) {
            k = 90;
            if (RightGen) {
                for (int i = 0; i < 90; i += 90) {
                    vertsToChange[k] = neighbourVerts[i];
                    k += 90;
                }
            }
            UpdateMesh();
            rightJoined = true;
        }

    }

    void UpdateMesh() {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        mesh.vertices = vertsToChange;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
