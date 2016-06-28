using UnityEngine;
using System.Collections;


public class SoftBody2D : MonoBehaviour {

    public int numRows = 10;
    public int numColumns = 10;
    public Vector3 xAxis = new Vector3(1, 0, 0);
    public Vector3 yAxis = new Vector3(0, 0, 1);
    public float spring = 500;
    public float damping = 20;
    public float scale = 0.5f;
    public float mass = 1.0f;
    public bool showBalls = true;
    public bool clampEdge = false;


    public Rigidbody leftHandle;
    public Rigidbody midHandle;
    public Rigidbody rightHandle;

    MeshFilter cloth;
    GameObject[][] links;

    // Use this for initialization
    void Start () {
        // get the prefab we set up earlier
        GameObject ball = Resources.Load<GameObject>("PhysicsSphere");

        Vector3 pos = transform.position;

        // create a number of links
        links = new GameObject[numColumns][];

        for (int i = 0; i < numColumns; i++)
        {
            links[i] = new GameObject[numRows];
            for (int j = 0; j < numRows; j++)
            {
                // move each one down by one unit (could replace this with spacing)
                Vector3 pos0 = transform.TransformPoint(xAxis * i + yAxis * j);
                links[i][j] = Instantiate(ball) as GameObject;
                links[i][j].transform.parent = gameObject.transform;
                links[i][j].transform.localPosition = pos0;
                links[i][j].name = "Link_" + i + "_" + j;
                links[i][j].GetComponent<Rigidbody>().isKinematic = false;
                links[i][j].transform.localScale = new Vector3(scale, scale, scale);
                links[i][j].GetComponent<Rigidbody>().mass = mass;
                links[i][j].GetComponent<MeshRenderer>().enabled = showBalls;
                if (clampEdge && i == 0)
                    links[i][j].GetComponent<Rigidbody>().isKinematic = true;

                // add a spring to the anchor to the previous row
                if (i != 0)
                {
                    SpringJoint joint = links[i][j].AddComponent<SpringJoint>();
                    joint.connectedBody = links[i - 1][j].GetComponent<Rigidbody>();
                    joint.anchor = new Vector3(0, 0, 0);
                    joint.connectedAnchor = -xAxis;
                    joint.spring = spring;
                    joint.damper = damping;
                }

                if (j != 0)
                {
                    SpringJoint joint = links[i][j].AddComponent<SpringJoint>();
                    joint.connectedBody = links[i][j-1].GetComponent<Rigidbody>();
                    joint.anchor = new Vector3(0, 0, 0);
                    joint.connectedAnchor = -yAxis;
                    joint.spring = spring;
                    joint.damper = damping;
                }
                else
                {
                    Rigidbody handle = null;
                    if (i == 0)
                        handle = rightHandle;
                    if (i == numColumns-1)
                        handle = leftHandle;
                    if (i == numColumns / 2)
                        handle = midHandle;

                    if (handle != null)
                    {
                        links[i][j].transform.position = handle.position;
                        FixedJoint joint = links[i][j].AddComponent<FixedJoint>();
                        joint.connectedBody = handle;
                        joint.anchor = new Vector3(0, 0, 0);
                        joint.connectedAnchor = new Vector3(0,0,0);
                    }
                }

                // diagonals
                if (i != 0 && j!=0)
                {
                    SpringJoint joint = links[i][j].AddComponent<SpringJoint>();
                    joint.connectedBody = links[i - 1][j-1].GetComponent<Rigidbody>();
                    joint.anchor = new Vector3(0, 0, 0);
                    joint.connectedAnchor = -xAxis-yAxis;
                    joint.spring = spring;
                    joint.damper = damping;
                }

                // bend springs
                if (i > 1)
                {
                    SpringJoint joint = links[i][j].AddComponent<SpringJoint>();
                    joint.connectedBody = links[i - 2][j].GetComponent<Rigidbody>();
                    joint.anchor = new Vector3(0, 0, 0);
                    joint.connectedAnchor = -2*xAxis;
                    joint.spring = spring;
                    joint.damper = damping;
                }

                if (j > 1)
                {
                    SpringJoint joint = links[i][j].AddComponent<SpringJoint>();
                    joint.connectedBody = links[i][j - 2].GetComponent<Rigidbody>();
                    joint.anchor = new Vector3(0, 0, 0);
                    joint.connectedAnchor = -2*yAxis;
                    joint.spring = spring;
                    joint.damper = damping;
                }
            }
        }

        cloth = GetComponent<MeshFilter>();
        if (cloth)
            SetUpCloth();
	}
	
	// Update is called once per frame
	void Update () {
        if (cloth)
            UpdateCloth();
	}

    void SetUpCloth()
    {
        cloth.mesh = new Mesh();
        UpdateCloth();

        // set up UV coordinates
        Vector2[] uvs = new Vector2[numRows * numColumns];
        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                uvs[i + j * numColumns] = new Vector2((float)i/(float)numColumns, (float)j/(float)numRows);
            }
        }
        cloth.mesh.uv = uvs;

        // set up the topolgy
        int[] triangles = new int[(numRows - 1) * (numColumns - 1) * 12];
        int k = 0;
        for (int i = 0; i < numColumns - 1; i++)
        {
            for (int j = 0; j < numRows - 1; j++)
            {
                triangles[k] = i + j * numColumns; k++;
                triangles[k] = (i + 1) + j * numColumns; k++;
                triangles[k] = i + (j + 1) * numColumns; k++;

                triangles[k] = i + (j + 1) * numColumns; k++;
                triangles[k] = (i + 1) + j * numColumns; k++;
                triangles[k] = (i + 1) + (j + 1) * numColumns; k++;

                /*triangles[k] = i + j * numColumns; k++;
                triangles[k] = i + (j + 1) * numColumns; k++;
                triangles[k] = (i + 1) + j * numColumns; k++;

                triangles[k] = (i + 1) + j * numColumns; k++;
                triangles[k] = i + (j + 1) * numColumns; k++;
                triangles[k] = (i + 1) + (j + 1) * numColumns; k++;*/
            }
        }
        cloth.mesh.triangles = triangles;
    }

    void UpdateCloth()
    {
        Vector3[] vertices = new Vector3[numRows * numColumns];
        Vector3[] normals = new Vector3[numRows * numColumns];
        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                Vector3 pos = links[i][j].transform.localPosition;
                //pos = transform.InverseTransformVector(pos);
                vertices[i + j * numColumns] = pos;
                links[i][j].GetComponent<MeshRenderer>().enabled = showBalls;
            }
        }
        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                Vector3 left = i == 0 ? vertices[i + j * numColumns] : vertices[i - 1 + j * numColumns];
                Vector3 right = i == numColumns - 1 ? vertices[i + j * numColumns] : vertices[i + 1 + j * numColumns];
                Vector3 down = j == 0 ? vertices[i + j * numColumns] : vertices[i + (j - 1) * numColumns];
                Vector3 up = j == numRows - 1 ? vertices[i + j * numColumns] : vertices[i + (j + 1) * numColumns];
                normals[i + j * numColumns] = Vector3.Cross(right - left, up - down);
                normals[i + j * numColumns].Normalize();
            }
        }

        cloth.mesh.vertices = vertices;
        cloth.mesh.normals = normals;
        cloth.mesh.RecalculateBounds();

    }
}
