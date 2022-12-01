using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMesh1 : MonoBehaviour
{
    Vector3[] vertices =
    {
        new Vector3(1, 1, 1),
        new Vector3(1, -1, 1),
        new Vector3(1, -1, -1),
        new Vector3(1, 1, -1),

        new Vector3(-1, 1, -1),
        new Vector3(-1, 1, 1),
        new Vector3(1, 1, 1),
        new Vector3(1, 1, -1),

        new Vector3(1, 1, 1),
        new Vector3(-1, 1, 1),
        new Vector3(-1, -1, 1),
        new Vector3(1, -1, 1),

        new Vector3(-1, -1, -1),
        new Vector3(-1, -1, 1),
        new Vector3(-1, 1, 1),
        new Vector3(-1, 1, -1),

        new Vector3(-1, -1, -1),
        new Vector3(-1, -1, 1),
        new Vector3(1, -1, 1),
        new Vector3(1, -1, -1),

        new Vector3(1, 1, -1),
        new Vector3(1, -1, -1),
        new Vector3(-1, -1, -1),
        new Vector3(-1, 1, -1)
    };

    int[] triangles =
    {
        0, 1, 2,
        2, 3, 0,

        4, 5, 6,
        6, 7, 4,

        8, 9, 10,
        10, 11, 8,

        12, 13, 14,
        14, 15, 12,

        16, 17, 18,
        18, 19, 16,

        20, 21, 22,
        22, 23, 20
    };

    Vector2[] uvs =
    {
        new Vector2(1, 1),
        new Vector2(0, 1),
        new Vector2(0, 0),
        new Vector2(1, 0),

        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),

        new Vector2(1, 1),
        new Vector2(0, 1),
        new Vector2(0, 0),
        new Vector2(1, 0),

        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),

        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),

        new Vector2(1, 1),
        new Vector2(1, 0),
        new Vector2(0, 0),
        new Vector2(0, 1)
    };
    
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
