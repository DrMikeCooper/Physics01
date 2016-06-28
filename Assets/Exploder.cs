using UnityEngine;
using System.Collections;

public class Exploder : MonoBehaviour {

    public float force = 100.0f;
    public float speed = 0.1f;

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = transform.position +
            new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed;

        // do the explodo!
        if (Input.GetKeyDown(KeyCode.X))
        {
            Rigidbody[] bodies = GameObject.FindObjectsOfType<Rigidbody>();
            foreach(Rigidbody body in bodies)
            {
                Vector3 dx = body.position - transform.position;
                body.AddForce(dx * force/dx.sqrMagnitude);
            }
       }

       /* if (Input.GetKeyDown(KeyCode.Space))
        {
            Rigidbody body = GetComponent<Rigidbody>();
            if (body)
            {
                body.isKinematic = false;
                body.AddForce(new Vector3(0, 0, body.mass*1000));
            }
        }*/
	}
}
