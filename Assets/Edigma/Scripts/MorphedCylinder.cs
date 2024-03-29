using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class MorphedCylinder : MonoBehaviour
{
    //public bool extrude = false;
    public bool autoRotate = false;
    public float rotFactor = 1.0f;
    public float morphForce = 1.0f;
    public int segments = 16;
    public int layers = 3;
    public float radius = 1f;
    public float insideRadius = 1f;
    public float height = 2f;

    public float heightSpeed = .5f;
    public float heightScale = .1f;
    float currentHeightScale = .0f;
    public float maxHeightScale = 1.2f;
    public Mesh startMesh;
    public Material meshMaterial;
    public Morpher[] morphers;
    public float rotateSpeed = 1.0f;
    bool finished = false;

    public GameObject vert;
    public GameObject vert2;
    Vector3[] vertices;
    float[] verticesRadius;
    int[] triangles;
    Vector3[] normals;
    Mesh mesh;
    //public float interactionRadius = .5f;



    GameObject[] verts;
    float[] cos;
    float[] sin;

    public bool debug = false;

    public bool sizeUp = false;
    public bool sizeDown = false;
    public bool expand = false;
    bool allocated = false;

    void DoTriangles()
    {
        for (int i = 0; i < (layers + (layers / 1)) - 1; i++)
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

    public float CalculateForce(Vector3 staticPos, Vector3 movablePos, float R)
    {
        float distanceSquared = (movablePos - staticPos).sqrMagnitude;
        if (distanceSquared > R * R)
        {
            return 0f;
        }
        float force = 1f - (distanceSquared / (R * R));
        return Mathf.Clamp01(force);
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
        if(!allocated) {
            Allocate();
        }
        float angleStep = 2f * Mathf.PI / segments;

        for (int j = 0; j <= segments; j++)
        {
            cos[j] = Mathf.Cos(j * angleStep);
            sin[j] = Mathf.Sin(j * angleStep);
        }

        for (int i = 0; i < layers + (layers / 1); i++)
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

                if (debug)
                {
                    GameObject oo = Instantiate(vert, new Vector3(0, 0, 0), Quaternion.identity);
                    oo.transform.parent = this.transform;
                    oo.transform.localPosition = vertices[vertexIndex];
                    oo.name = " " + vertexIndex + " " + j + " " + i;
                    verts[vertexIndex] = oo;
                }
            }
        }
    }

    void Allocate()
    {
        mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        int numVertices = (segments + 1) * (layers + (layers / 1));
        vertices = new Vector3[numVertices];
        normals = new Vector3[numVertices];
        triangles = new int[segments * (layers + (layers / 1)) * 6];
        cos = new float[segments + 1];
        sin = new float[segments + 1];
        verticesRadius = new float[numVertices];
        allocated = true;
    }

    public void Reset()
    {
        CouchChanged();
        Debug.Log("RESET MESH");
        finished = false;
        currentHeightScale = heightScale;
        startTime = .0f;
        GenerateInit();
        DoTriangles();
        Apply();
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = meshMaterial;
    }

    public void Stop()
    {
        finished = true;
    }

    void Awake()
    {
        Allocate();
    }

    void Start()
    {
        Reset();
        BDController.Instance.Loaded.AddListener(CouchChanged);
    }

    public void CouchChanged()
    {
        autoRotate = BDController.Instance.AutoRotate();
        rotFactor = BDController.Instance.RotateFactor();
    }

    public float anim = 1.0f;
    public float startTime = 0.0f;

    void FixedUpdate()
    {

        if (finished)
        {
            return;
        }
        transform.localScale = new Vector3(1, currentHeightScale, 1);

        startTime += Time.deltaTime;
        if (startTime <= 2.0f)
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
            if (!go.isActiveAndEnabled || !go.gameObject.activeInHierarchy)
            {
                continue;
            }

            Vector3 posY = go.transform.position;
            posY.y = 0;
            Vector3 target = transform.position;
            target.y = 0;


            bool handIsUp = go.mainMorpher && go.transform.up.y < -0.5f;
            bool handIsDown = go.mainMorpher && go.transform.up.y > 0.5f;
            bool extrude = false;

            if (go.mainMorpher && !handIsDown && !handIsUp)
            {
                if (target.x > posY.x)
                {
                    extrude = go.transform.up.x > .5f;
                }
                else
                {
                    extrude = go.transform.up.x < -.5f;
                }

                // Debug.Log(extrude + " " + (target.x - posY.x) + " " + go.transform.up.x);

            }

            float dist = Vector3.Magnitude(target - posY);

            if (dist > radius * 2.5f)
            {
                continue;
            }

            Vector3 pos = go.transform.position;

            for (int i = 0; i < (layers + (layers / 1)); i++)
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

                if (handIsDown || handIsUp)
                {
                    if (i == layers)
                    {

                        if (handIsUp)
                        {
                            if (pos.y > yVWorldPos.y)
                            {
                                currentHeightScale += heightSpeed * Time.deltaTime;
                                if (currentHeightScale >= maxHeightScale)
                                {
                                    currentHeightScale = maxHeightScale;
                                }
                            }
                        }

                        if (handIsDown && Mathf.Abs(pos.y - yVWorldPos.y) > go.interactionRadius / 2)
                        {
                            if (pos.y < yVWorldPos.y)
                            {
                                currentHeightScale -= heightSpeed * Time.deltaTime;
                                if (currentHeightScale <= heightScale)
                                {
                                    currentHeightScale = heightScale;
                                }
                            }
                        }
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                for (int j = 0; j <= (segments - 1); j++)
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

                        float force = CalculateForce(vertWorldPos, pos, go.interactionRadius);

                        if (extrude)
                        {
                            if (force < vradius)
                            {
                                float realForce = (go.interactionRadius - force);

                                vradius += 0.1f * Time.deltaTime * morphForce * realForce;
                            }
                        }
                        else
                        {
                            vradius -= 0.1f * Time.deltaTime * morphForce * force;
                        }


                        if (vradius <= radius * 0.1f)
                        {
                            vradius = radius * 0.1f;
                        }

                        if (vradius >= radius * 1.3f)
                        {
                            vradius = radius * 1.3f;
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
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
