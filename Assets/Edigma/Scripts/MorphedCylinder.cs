using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphedCylinder : MonoBehaviour
{
    public bool autoRotate = false;
    public float rotFactor = 1.0f;
    public float morphForce = 1.0f;
    public int segments = 16;
    public int layers = 3;
    public float radius = 1f;
    public float insideRadius = 1f;
    public float height = 2f;

    public Mesh startMesh;
    public Material meshMaterial;
    public Morpher[] morphers;

    public GameObject vert;
    public GameObject vert2;
    Vector3[] vertices;
    float[] verticesRadius;
    int[] triangles;
    Vector3[] normals;
    Mesh mesh;
    //public float interactionRadius = .5f;
    public float rotateSpeed = 1.0f;

    public GameObject[] verts;
    float[] cos;
    float[] sin;

    bool debug = false;
    void DoTriangles()
    {
        for (int i = 0; i < (layers + (layers / 3)) - 1; i++)
        {
            for (int j = 0; j < segments; j++)
            {
                int triangleIndex = i * segments * 6 + j * 6;
                int i1 = i * (segments) + j;
                int i2 = i * (segments) + j + 1;
                int i3 = (i + 1) * (segments) + j;
                int i4 = (i + 1) * (segments) + j + 1;
                triangles[triangleIndex] = i1;
                triangles[triangleIndex + 1] = i3;
                triangles[triangleIndex + 2] = i2;

                triangles[triangleIndex + 3] = i2;
                triangles[triangleIndex + 4] = i3;
                triangles[triangleIndex + 5] = i4;
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

        int numVertices = (segments + 1) * (layers + (layers / 3));
        vertices = new Vector3[numVertices];
        normals = new Vector3[numVertices];
        triangles = new int[segments * (layers + (layers / 3)) * 6];

        verticesRadius = new float[numVertices];
        if (debug)
        {
            verts = new GameObject[numVertices];
        }

        float angleStep = 2f * Mathf.PI / segments;
        cos = new float[segments + 1];
        sin = new float[segments + 1];
        for (int j = 0; j <= segments; j++)
        {
            cos[j] = Mathf.Cos(j * angleStep);
            sin[j] = Mathf.Sin(j * angleStep);
        }

        for (int i = 0; i < layers + (layers / 3); i++)
        {

            float thick_radius = 1.0f;
            float y = 0;
            if (i > layers)
            {
                y = (float)(layers - ((i - 1) - layers)) / (float)(layers - 1) * height;
                thick_radius = insideRadius;
            }
            else
            {
                thick_radius = radius;
                y = (float)i / (float)(layers - 1) * height;
            }

            for (int j = 0; j <= segments; j++)
            {
                int vertexIndex = i * segments + j;
                float x = cos[j] * thick_radius;
                float z = sin[j] * thick_radius;
                vertices[vertexIndex] = new Vector3(x, y, z);
                normals[vertexIndex] = (i > layers ? -1 : 1) * new Vector3(x, 0f, z).normalized;
                verticesRadius[vertexIndex] = thick_radius;

                if (false)
                {
                    GameObject oo = Instantiate(vert, new Vector3(0, 0, 0), Quaternion.identity);
                    oo.transform.parent = this.transform;
                    oo.transform.localPosition = vertices[vertexIndex];
                    oo.name = " " + vertexIndex;
                    if (i == 15 && j == 0)
                    {
                        //Debug.Log("created" + vertexIndex + " " + oo.transform.localPosition);
                    }
                    //verts[vertexIndex] = oo;
                }
            }
        }

    }

    void Start()
    {
        Debug.Log("-.s" + startMesh.vertices.Length);

        GenerateInit();
        DoTriangles();

        /*
                mesh = new Mesh();

                GetComponent<MeshFilter>().mesh = mesh;
                vertices = (Vector3[])startMesh.vertices.Clone();
                normals = (Vector3[])startMesh.normals.Clone();
                triangles = (int[])startMesh.triangles.Clone();
                transform.localScale = Vector3.one * 10;
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = new Vector3(-90.0f, 0, 0);
                verticesRadius = new float[vertices.Length];
                int i = 0;
                foreach (Vector3 vertI in vertices)
                {

                    Vector3 yVWorldPos = transform.TransformPoint(vertI);
                    float rad = Mathf.Atan2(yVWorldPos.y, yVWorldPos.x);
                    verticesRadius[i] = rad;
                    i++;

                    //if (i < 500 || i > 19000)
                    //{
                    //    GameObject oo = Instantiate( i < 500 ? vert2 : vert, new Vector3(0, 0, 0), Quaternion.identity);
                    //    oo.transform.parent = this.transform;
                    //    oo.transform.localPosition = vertices[i];
                    //    oo.name = i + " o " + rad;
                    //}
                }
        */
        Apply();

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = meshMaterial;


    }

    public float anim = 1.0f;

    void FixedUpdate()
    {
        if (Time.time <= 2.0f)
        {
            return;
        }

        if (autoRotate)
        {
            transform.Rotate(new Vector3(0, rotateSpeed, 0));
        }
        else
        {
            if (PhidgetsController.Instance)
            {
                anim += Time.deltaTime;
                float offset = Time.deltaTime * (PhidgetsController.Instance.M_Position / rotFactor) / PhidgetsController.Instance.M_DataInterval;
                transform.Rotate(new Vector3(0, offset, 0));
            }
            //Debug.Log(Time.deltaTime + " " + PhidgetsController.Instance.M_Position + " " +PhidgetsController.Instance.M_DataInterval);
        }

        if (morphers.Length == 0)
        {
            return;
        }

        float angleStep = 2f * Mathf.PI / segments;
        foreach (Morpher go in morphers)
        {
            if (!go.isActiveAndEnabled)
            {
                continue;
            }

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

            for (int i = 0; i < (layers + (layers / 3) - 1); i++)
            {
                float y = 0;
                if (i > layers)
                {
                    y = (float)(layers - ((i - 1) - layers)) / (float)(layers - 1) * height;
                }
                else
                {
                    y = (float)i / (float)(layers - 1) * height;
                }

                Vector3 yV = new Vector3(0, y, 0);
                Vector3 yVWorldPos = transform.TransformPoint(yV);



                if (Mathf.Abs(pos.y - yVWorldPos.y) > go.interactionRadius)
                {
                    continue;
                }


                for (int j = 0; j <= segments; j++)
                {
                    int vertexIndex = i * segments + j;
                    Vector3 vertPos = vertices[vertexIndex];
                    float vradius = verticesRadius[vertexIndex];

                    if (i > layers)
                    {
                        int friendI = layers - (i - layers);
                        int friendVertexIndex = friendI * segments + j;

                        float vradius2 = verticesRadius[friendVertexIndex];
                        vradius = vradius2 - (radius - insideRadius);
                    }
                    else
                    {
                        Vector3 vertWorldPos = transform.TransformPoint(vertPos);
                        float force = Vector3.Magnitude(vertWorldPos - pos);

                        if (force < go.interactionRadius)
                        {
                            float realForce = (go.interactionRadius - force);

                            vradius -= 0.1f * Time.deltaTime * morphForce * realForce;
                        }


                        if (vradius <= radius * 0.1f)
                        {
                            vradius = radius * 0.1f;
                        }

                    }

                    float x = cos[j] * vradius;
                    float z = sin[j] * vradius;



                    verticesRadius[vertexIndex] = vradius;
                    vertices[vertexIndex] = new Vector3(x, vertPos.y, z);

                    if (debug)
                    {
                        verts[vertexIndex].transform.localPosition = vertices[vertexIndex];
                    }
                    //normals[vertexIndex] = new Vector3(x, 0f, z).normalized;

                }
            }
        }

        ApplyVerts();
    }
}
