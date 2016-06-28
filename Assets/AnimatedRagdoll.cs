using UnityEngine;
using System.Collections;

public class AnimatedRagdoll : MonoBehaviour {

    Rigidbody[] bodies;

	// Use this for initialization
    void Start () {
        bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in bodies)
        {
            AnimatedRagdollPart part = body.gameObject.AddComponent<AnimatedRagdollPart>();
            part.ragdoll = this;
        }

        MakeKinematic(true);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.P))
            MakeKinematic(false);
	}

    public void MakeKinematic(bool kin)
    {
        foreach (Rigidbody body in bodies)
        {
            body.isKinematic = kin;
            Collider _collider = body.gameObject.GetComponent<Collider>();
            if (_collider)
                _collider.isTrigger = kin;
        }
        Animator anim = GetComponent<Animator>();
        if (anim)
            anim.enabled = kin;
    }


}
