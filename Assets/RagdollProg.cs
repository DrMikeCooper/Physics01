using UnityEngine;
using System.Collections;

public class RagdollProg : MonoBehaviour {

    GameObject sphere;
    GameObject capsule;

	// Use this for initialization
	void Start () {
        sphere = Resources.Load("PhysicsSphere") as GameObject;
        capsule = Resources.Load("PhysicsCapsule") as GameObject;

        // make a pelvis root node
        GameObject pelvis = Instantiate(sphere, transform.position, transform.rotation) as GameObject;
        pelvis.name = "Pelvis";
        pelvis.transform.parent = gameObject.transform;

        GameObject body = AddPart("Body", capsule, pelvis, new Vector3(0, 1, 0), new Vector3(0, 0, 1), -20, 20, new Vector3(-1, 0, 0), 10);
        GameObject head = AddPart("Head", sphere, body, new Vector3(0, 1, 0), new Vector3(1, 0, 0), -40, 25, new Vector3(0, 0, 1), 25);
        GameObject armUL = AddPart("ArmUL", capsule, body, new Vector3(-0.5f, 0, 0), new Vector3(0, -1, 0), -70, 10, new Vector3(1, 0, 0), 50);
        GameObject armLL = AddPart("ArmLL", capsule, armUL, new Vector3(0, -1, 0), new Vector3(1, 0, 0), -90, 0, new Vector3(0,-1, 0), 0);
        GameObject armUR = AddPart("ArmUR", capsule, body, new Vector3(0.5f, 0, 0), new Vector3(0, -1, 0), -70, 10, new Vector3(1, 0, 0), 50);
        GameObject armLR = AddPart("ArmLR", capsule, armUR, new Vector3(0, -1, 0), new Vector3(1, 0, 0), -90, 0, new Vector3(0, -1, 0), 0);
        GameObject legUL = AddPart("LegUL", capsule, pelvis, new Vector3(-0.5f, -1, 0), new Vector3(0, 0, 1), -20, 70, new Vector3(1, 0, 0), 30);
        GameObject legLL = AddPart("LegLL", capsule, legUL, new Vector3(0, -1, 0), new Vector3(0, 0, 1), -90, 0, new Vector3(1, 0, 0), 0);
        GameObject legUR = AddPart("LegUR", capsule, pelvis, new Vector3(0.5f, -1, 0), new Vector3(0, 0, 1), -20, 70, new Vector3(1, 0, 0), 30);
        GameObject legLR = AddPart("LegLR", capsule, legUR, new Vector3(0, -1, 0), new Vector3(0, 0, 1), -90, 0, new Vector3(1, 0, 0), 0);

        /* GameObject legUL = AddPart("LegUL", capsule, pelvis, new Vector3(-0.5f, -1, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));
         GameObject legLL = AddPart("LegLL", capsule, legUL, new Vector3(0, -1, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));
         GameObject legUR = AddPart("LegUR", capsule, pelvis, new Vector3(0.5f, -1, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));
         GameObject legLR = AddPart("LegLR", capsule, legUR, new Vector3(0, -1, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));*/
    }

    // Update is called once per frame
    void Update () {
        

	}

    GameObject AddPart(string name, GameObject template, GameObject parent, Vector3 pos, Vector3 axis, float lmin, float lmax, Vector3 swingAxis, float range)
    {
        GameObject part = Instantiate(template);
        part.transform.position = parent.transform.position + pos;
        part.transform.parent = parent.transform;
        part.name = name;

        CharacterJoint joint = part.AddComponent<CharacterJoint>();
        joint.connectedBody = parent.GetComponent<Rigidbody>();
        joint.axis = axis;
        SoftJointLimit sjl;
        sjl = joint.lowTwistLimit; sjl.limit = lmin; joint.lowTwistLimit = sjl;
        sjl = joint.highTwistLimit; sjl.limit = lmax; joint.highTwistLimit = sjl;
        joint.swingAxis = swingAxis;
        sjl = joint.swing1Limit; sjl.limit = range; joint.swing1Limit = sjl;

        return part;
    }
}
