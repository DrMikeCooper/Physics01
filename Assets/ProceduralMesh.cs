using UnityEngine;
using System.Collections;

public class ProceduralMesh : MonoBehaviour {

    public enum TerrainType { DiamondSquare, Perlin, Faultlines, Mandelbrot };

    public int span = 32;
    public float height = 8;

    public float FaultlineCount = 50;
    public float PerlinFrequency = 5.0f;
    public float MandelbrotScale = 3.0f;
    public Vector2 MandelbrotOrigin = new Vector2(0, 0);

    public TerrainType type = TerrainType.DiamondSquare;
    TerrainType cachedType;
    Vector2 cachedOrigin;

    float[] heightField;

    // Use this for initialization
    void Start() {
        GenerateTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        if (cachedType != type)
            GenerateTerrain();
        if (cachedOrigin != MandelbrotOrigin)
        {
            cachedOrigin = MandelbrotOrigin;
            GenerateTerrain();
        }
    }

    void GenerateTerrain()
    {
        cachedType = type;
        // create a mesh
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        heightField = new float[(span + 1) * (span + 1)];

        Vector3[] vertices = new Vector3[(span + 1) * (span + 1)];
        Vector2[] uvs = new Vector2[(span + 1) * (span + 1)];
        Vector3[] normals = new Vector3[(span + 1) * (span + 1)];
        int[] indices = new int[span * span * 6];

        // zero all data
        for (int i = 0; i < (span + 1) * (span + 1); i++)
            heightField[i] = 0;

        // use the algorithm of our choice to generate heights
        if (type == TerrainType.DiamondSquare)
            MakeDiamondSquares();
        else if (type == TerrainType.Faultlines)
            MakeFaultlines();
        else if (type == TerrainType.Perlin)
            MakePerlinNoise();
        else if (type == TerrainType.Mandelbrot)
            MakeMandelbrot();

        // fill in the data as a flat plane centered on the origin
        int k = 0;
        for (int i = 0; i <= span; i++)
        {
            int iNext = i != span ? i + 1 : -1;
            for (int j = 0; j <= span; j++)
            {
                int jNext = j != span ? j + 1 : -1;
                int index00 = i + j * (span + 1);
                int index01 = iNext + j * (span + 1);
                int index10 = i + jNext * (span + 1);
                int index11 = iNext + jNext * (span + 1);

                vertices[index00] = new Vector3(i - span * 0.5f, heightField[index00], j - span * 0.5f);
                // take the normal by comparing neighbouring points
                normals[index00] = new Vector3(getH(Mathf.Max(i - 1, 0), j) - getH(Mathf.Min(i + 1, span), j), 1, getH(i, Mathf.Max(j - 1, 0)) - getH(i, Mathf.Min(j + 1, span)));
                uvs[index00] = new Vector2(i / (1.0f * span), j / (1.0f * span));

                if (iNext != -1 && jNext != -1)
                {
                    // first triangle for quad
                    indices[k] = index00; k++;
                    indices[k] = index10; k++;
                    indices[k] = index01; k++;

                    //second triangle for quad
                    indices[k] = index01; k++;
                    indices[k] = index10; k++;
                    indices[k] = index11; k++;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        filter.mesh = mesh;
        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider)
            collider.sharedMesh = mesh;
    }

    float getH(int i, int j)
    {
        return heightField[i + j * (span + 1)];
    }
    void setH(int i, int j, float h)
    {
        heightField[i + j * (span + 1)] = h;
    }
    void incH(int i, int j, float h)
    {
        heightField[i + j * (span + 1)] += h;
    }

    void MakeFaultlines()
    {
        for (int k = 0; k < FaultlineCount; k++)
        {
            // take a random gradient
            float angle = Random.Range(0, 6.28f);

            // 50% chance to invert it
            float grad = Mathf.Tan(angle);

            //pick a random point on the grid that our line passes through
            int x0 = Random.Range(0, span);
            int y0 = Random.Range(0, span);

            // displace everything on the left by somewhere between +/-1 unit
            float disp = height * Random.Range(-0.1f, 0.1f);

            for (int i = 0; i <= span; i++)
                for (int j = 0; j <= span; j++)
                    if ((i - x0) + grad * (j - y0) < 0)
                        incH(i, j, disp);
        }
    }

    void MakePerlinNoise()
    {
        float scale = PerlinFrequency / span;
        for (int i = 0; i <= span; i++)
            for (int j = 0; j <= span; j++)
                setH(i, j, height * Mathf.PerlinNoise(i * scale, j * scale));
    }

    void MakeDiamondSquares()
    {
        // large scale - displacement of up to 1 unit, and a stride covering the whole grid
        float disp = height;
        for (int stride = span; stride > 1; stride = stride / 2)
        {
            // halve the displacement each iteration
            disp = disp * 0.5f;

            // joggle all the square corners along the grid
            for (int i = 0; i <= span; i += stride)
            {
                for (int j = 0; j <= span; j += stride)
                {
                    // initialise the four corners
                    if (stride == span)
                        setH(i, j, Random.Range(0, disp));

                    if (i > 0 && j > 0)
                    {
                        // center point is the average of the four corners
                        setH(i - stride / 2, j - stride / 2, 0.25f * (
                            getH(i, j)
                            + getH(i - stride, j)
                            + getH(i, j - stride)
                            + getH(i - stride, j - stride)
                            ) + Random.Range(0, disp));

                        //edges
                        setH(i - stride / 2, j, 0.33f * (getH(i - stride / 2, j - stride / 2) + getH(i, j) + getH(i - stride, j)) + Random.Range(0, disp));
                        setH(i - stride / 2, j - stride, 0.33f * (getH(i - stride / 2, j - stride / 2) + getH(i, j - stride) + getH(i - stride, j - stride)) + Random.Range(0, disp));

                        setH(i, j - stride / 2, 0.33f * (getH(i - stride / 2, j - stride / 2) + getH(i, j) + getH(i, j - stride)) + Random.Range(0, disp));
                        setH(i - stride, j - stride / 2, 0.33f * (getH(i - stride / 2, j - stride / 2) + getH(i - stride, j) + getH(i - stride, j - stride)) + Random.Range(0, disp));
                    }
                }
            }

        }
    }

    void MakeMandelbrot()
    {
        for (int i = 0; i <= span; i++)
        {
            float x0 = (i - 0.5f * span - MandelbrotOrigin.x) * MandelbrotScale / span;
            for (int j = 0; j <= span; j++)
            {
                float y0 = (j - 0.5f * span - MandelbrotOrigin.y) * MandelbrotScale / span;

                int count = 0;
                float x = 0, y = 0;
                while(count < 16 && (x*x+y*y)<2)
                {
                    float xt = x * x - y * y + x0;
                    y = 2 * x * y + y0;
                    x = xt;
                    count++;
                }
                setH(i, j, (16 - count)/16.0f * height);
            }
        }
    }


}
