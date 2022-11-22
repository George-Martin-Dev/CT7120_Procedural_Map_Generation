using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen1 : MonoBehaviour {
    private UnityEngine.Mesh mesh;

    Vector3[] verts;
    int[] tris;

    void Start() {
        mesh = new UnityEngine.Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateMeshInfo();
        UpdateMesh();
    }

    private void Update() {       
        
    }

    void GenerateMeshInfo() {
        int width = 20;
        int height = 20;
        float y;

        verts = new Vector3[(width + 1) * (height + 1)];

        for (int o = 0, i = 0; i <= height; i++) {
            for(int j = 0; j <= width; j++) {
                y = Mathf.PerlinNoise(j * .3f, i * .3f) * 2f;
                verts[o] = new Vector3(j, y, i);
                o++;
            }
        }

        tris = new int[width * height * 6];

        int t = 0;
        int v = 0;

        for (int j = 0; j < height; j++) {
            for (int i = 0; i < width; i++) {
                tris[t + 0] = v + 0;
                tris[t + 1] = v + width + 1;
                tris[t + 2] = v + 1;
                tris[t + 3] = v + 1;
                tris[t + 4] = v + width + 1;
                tris[t + 5] = v + width + 2;

                v++;
                t += 6;
            }
            v++;
        }     
    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        mesh.RecalculateBounds();
        MeshCollider collider = gameObject.GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }

    private void OnDrawGizmos() {

        if (verts == null) {
            return;
        }

        for (int i = 0; i < verts.Length; i++) {
            Gizmos.DrawSphere(verts[i], .1f);
        }
    }
}
