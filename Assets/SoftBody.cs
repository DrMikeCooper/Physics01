using UnityEngine;
using System.Collections;

public class SoftBody : MonoBehaviour {

    public int numRows = 5;
    public Vector3 xAxis = new Vector3(0, -1, 0);

	// Use this for initialization
	void Start () {
        // get the prefab we set up earlier
        GameObject ball = Resources.Load<GameObject>("PhysicsSphere");

        Vector3 pos = transform.position;

        //instantiate an anchor and make it our child 
        GameObject anchor = Instantiate(ball, pos, transform.rotation) as GameObject;
        anchor.transform.parent = gameObject.transform;
        anchor.name = "Anchor";
        anchor.GetComponent<Rigidbody>().isKinematic = true;

        // create a number of links
        GameObject[] links = new GameObject[numRows];
        for (int i=0;i< numRows; i++)
        {
            // move each one down by one unit (could replace this with spacing)
            pos += xAxis;
            links[i] = Instantiate(ball, pos, transform.rotation) as GameObject;
            links[i].transform.parent = gameObject.transform;
            links[i].name = "Link" + i;
            links[i].GetComponent<Rigidbody>().isKinematic = false;

            // add a hinge to the anchor (for the first link) or the previous link
            HingeJoint joint = links[i].AddComponent<HingeJoint>();
            joint.connectedBody = (i == 0 ? anchor : links[i - 1]).GetComponent<Rigidbody>();

            // set up the anchors manually from centre to centre
            joint.anchor = new Vector3(0, 0, 0);
            joint.connectedAnchor = xAxis;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
