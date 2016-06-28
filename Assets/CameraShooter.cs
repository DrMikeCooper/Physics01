using UnityEngine;
using System.Collections;

public class CameraShooter : MonoBehaviour {

    public float speed = 0.1f;
    public float forceScale = 500;
    public float scale = 1;

    GameObject sphere;

	// Use this for initialization
	void Start () {
        sphere = Resources.Load("PhysicsSphere") as GameObject;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        Vector3 angles = transform.eulerAngles;

        if (Input.GetKey(KeyCode.D))
            pos += transform.right * speed;
        if (Input.GetKey(KeyCode.A))
            pos -= transform.right * speed;
        if (Input.GetKey(KeyCode.W))
            pos += transform.forward * speed;
        if (Input.GetKey(KeyCode.S))
            pos -= transform.forward * speed;
        if (Input.GetKey(KeyCode.E))
            angles.y -= 5*speed;
        if (Input.GetKey(KeyCode.Q))
            angles.y += 5*speed;

        transform.position = pos;
        transform.eulerAngles = angles;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bullet = Instantiate(sphere, transform.position, transform.rotation) as GameObject;
            bullet.tag = "Missile";
            bullet.transform.localScale = new Vector3(scale, scale, scale);
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward * forceScale);
        }
	}
}
