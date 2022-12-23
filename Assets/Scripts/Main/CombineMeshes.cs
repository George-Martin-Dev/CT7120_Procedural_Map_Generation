using System.Linq;
using UnityEngine;

public class CombineMeshes : MonoBehaviour {

    private MeshFilter[] chunkMeshes;
    private MeshRenderer meshRenderer;
    private CombineInstance[] combine;
    [SerializeField] private Material terrainMat;

    bool combined = false;

    private void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Takes the meshes of each of the child chunk segments and combines them in to one mesh for the chunk.
    /// Assigns the UV coordinates, so that a texture can be assigned.
    /// Applies a mesh collider to the new mesh.
    /// Then deactivates all child objects, as they are no longer needed.
    /// </summary>
    void Combine() {
        chunkMeshes = GetComponentsInChildren<MeshFilter>();
        combine = new CombineInstance[chunkMeshes.Length];

        for (int i = 0; i < chunkMeshes.Length; i++) {
            combine[i].mesh = chunkMeshes[i].sharedMesh;
            combine[i].transform = chunkMeshes[i].transform.localToWorldMatrix;
            chunkMeshes[i].gameObject.SetActive(false);
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);

        Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] UVs = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++) {
            UVs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        mesh.uv = UVs;
        mesh.RecalculateBounds();
        MeshCollider collider = gameObject.GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }



    // Update is called once per frame
    void Update() {
        if (transform.childCount == 9 && !combined) {

            bool[] rendered = new bool[transform.childCount];

            for (int i = 0; i < transform.childCount; i++) {
                rendered[i] = transform.GetChild(i).GetComponent<MeshGeneration>().created;
            }

            if (rendered.All(x => x)) {
                Combine();
                combined = true;
            }
        }
    }
}
