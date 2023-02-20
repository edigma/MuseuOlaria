using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphedCylinder : MonoBehaviour
{
    public float morphForce = 1.0f;
    public int segments = 16;
    public int layers = 3;
    public float radius = 1f;
    public float height = 2f;

    public GameObject[] morphers;

    public GameObject vert;
    public GameObject vert2;
    Vector3[] vertices;
    float[] verticesRadius;
    int[] triangles;
    Vector3[] normals;
    Mesh mesh;

    public GameObject[] verts;


    bool debug = false;
    void DoTriangles()
    {
        int triangleIndex = 0;
        for (int i = 0; i < layers - 1; i++)
        {
            for (int j = 0; j < segments; j++)
            {
                int i1 = i * (segments + 1) + j;
                int i2 = i * (segments + 1) + j + 1;
                int i3 = (i + 1) * (segments + 1) + j;
                int i4 = (i + 1) * (segments + 1) + j + 1;

                triangles[triangleIndex++] = i1;
                triangles[triangleIndex++] = i3;
                triangles[triangleIndex++] = i2;

                triangles[triangleIndex++] = i2;
                triangles[triangleIndex++] = i3;
                triangles[triangleIndex++] = i4;
            }
        }
    }

    void Apply()
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
    }

    void ApplyVerts()
    {
        mesh.vertices = vertices;
        mesh.normals = normals;
    }

    void GenerateInit()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int numVertices = (segments + 1) * layers;
        vertices = new Vector3[numVertices];
        normals = new Vector3[numVertices];
        triangles = new int[segments * layers * 6];

        verticesRadius = new float[numVertices];
        if (debug)
        {
            verts = new GameObject[numVertices];
        }

        float angleStep = 2f * Mathf.PI / segments;
        float angle = 0f;
        int vertexIndex = 0;
        for (int i = 0; i < layers; i++)
        {
            float y = (float)i / (float)(layers - 1) * height;

            for (int j = 0; j <= segments; j++)
            {
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                vertices[vertexIndex] = new Vector3(x, y, z);
                normals[vertexIndex] = new Vector3(x, 0f, z).normalized;
                verticesRadius[vertexIndex] = radius;

                if (debug)
                {
                    GameObject oo = Instantiate(i == 15 && j == 0 ? vert2 : vert, new Vector3(0, 0, 0), Quaternion.identity);
                    oo.transform.parent = this.transform;
                    oo.transform.localPosition = vertices[vertexIndex];

                    if (i == 15 && j == 0)
                    {
                        Debug.Log("created" + vertexIndex + " " + oo.transform.localPosition);
                    }
                    verts[vertexIndex] = oo;
                }
                vertexIndex++;
                angle += angleStep;
            }
        }
    }

    void Start()
    {
        GenerateInit();
        DoTriangles();
        Apply();

    }

    public float anim = 1.0f;

    void Update()
    {
        transform.Rotate(new Vector3(0, 1, 0));

        anim += Time.deltaTime * 5;


        if (morphers.Length == 0)
        {
            return;
        }

        float angleStep = 2f * Mathf.PI / segments;
        foreach (GameObject go in morphers)
        {
            Vector3 posY = go.transform.position;
            posY.y = 0;
            Vector3 target = transform.position;
            target.y = 0;

            float dist = Vector3.Magnitude(target - posY);

            if (dist > radius * 2.5f)
            {
                continue;
            }



            Vector3 pos = go.transform.position;
            int vertexIndex = 0;
            float angle = 0f;
            for (int i = 0; i < layers; i++)
            {
                for (int j = 0; j <= segments; j++)
                {
                    Vector3 vertPos = vertices[vertexIndex];

                    float radius = verticesRadius[vertexIndex];

                    Vector3 vertWorldPos = transform.TransformPoint(vertPos);



                    float force = Vector3.Magnitude(vertWorldPos - pos);
                    if (i == 15 && j == 0 && debug)
                    {
                        Vector3 loo = vertPos - pos;
                        Debug.Log(vertexIndex + " " + verts[vertexIndex].transform.position + " " + verts[vertexIndex].transform.localPosition);
                        Debug.Log(pos);
                        Debug.Log(loo.x + " " + loo.y + " " + loo.z + "\n\n");
                    }

                    if (force < 0.01f)
                    {
                        radius -= 0.1f * Time.deltaTime * morphForce * (0.01f - force);
                    }
                    else
                    {

                        vertexIndex++;
                        angle += angleStep;
                        continue;
                    }

                    float x = Mathf.Cos(angle) * radius;
                    float z = Mathf.Sin(angle) * radius;

                    verticesRadius[vertexIndex] = radius;
                    vertices[vertexIndex] = new Vector3(x, vertPos.y, z);
                    if (debug)
                    {
                        verts[vertexIndex].transform.localPosition = vertices[vertexIndex];
                    }
                    normals[vertexIndex] = new Vector3(x, 0f, z).normalized;

                    vertexIndex++;
                    angle += angleStep;


                }
            }
        }

        ApplyVerts();
    }
}
