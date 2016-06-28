using UnityEngine;
using System.Collections;

public class PuppetHandle : MonoBehaviour {

    public KeyCode left;
    public KeyCode right;
    public float speed = 0.02f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        if (Input.GetKey(left))
            pos.x -= speed;
        if (Input.GetKey(right))
            pos.x += speed;
        transform.position = pos;
    }
}
