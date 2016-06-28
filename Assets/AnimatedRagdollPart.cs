using UnityEngine;
using System.Collections;

public class AnimatedRagdollPart : MonoBehaviour
{
    public AnimatedRagdoll ragdoll;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Missile")
        {
            ragdoll.MakeKinematic(false);
            Rigidbody missile = collider.gameObject.GetComponent<Rigidbody>();
            Rigidbody body = gameObject.GetComponent<Rigidbody>();
            if (missile && body)
                body.AddForce(missile.mass * missile.velocity);
        }
    }
}
