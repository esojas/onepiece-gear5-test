// Generates a plane with custom vertex density, attach to any GameObject
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DensePlane : MonoBehaviour
{
    public int subdivisions = 100; // vertices per side — 100 gives 1-unit spacing at scale 10
    public float size = 10f;       // local size before scale is applied

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = GenerateMesh();
    }

    Mesh GenerateMesh()
    {
        var mesh = new Mesh();
        int vCount = subdivisions + 1;
        var verts = new Vector3[vCount * vCount];
        var uvs = new Vector2[verts.Length];
        float step = size / subdivisions;

        for (int z = 0; z <= subdivisions; z++)
            for (int x = 0; x <= subdivisions; x++)
            {
                int i = z * vCount + x;
                verts[i] = new Vector3(x * step - size / 2f, 0, z * step - size / 2f);
                uvs[i] = new Vector2((float)x / subdivisions, (float)z / subdivisions);
            }

        var tris = new int[subdivisions * subdivisions * 6];
        int t = 0;
        for (int z = 0; z < subdivisions; z++)
            for (int x = 0; x < subdivisions; x++)
            {
                int i = z * vCount + x;
                tris[t++] = i; tris[t++] = i + vCount; tris[t++] = i + 1;
                tris[t++] = i + 1; tris[t++] = i + vCount; tris[t++] = i + vCount + 1;
            }

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        return mesh;
    }
}