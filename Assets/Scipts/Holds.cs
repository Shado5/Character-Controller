using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holds : MonoBehaviour
{
    private FixedJoint currentFixedJoint;

    public FixedJoint CurrentFixedJoint
    {
        get { return currentFixedJoint; }
    }

    public void SetCurrentFixedJoint(FixedJoint fixedJoint)
    {
        currentFixedJoint = fixedJoint;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("holds"))
        {
            // Check if the collided object has a Rigidbody component
            Rigidbody collidedRigidbody = collision.collider.GetComponent<Rigidbody>();

            // If there is a Rigidbody
            if (collidedRigidbody != null)
            {
                // Attach the limb to the hold with a FixedJoint
                AttachLimbToHold(collidedRigidbody);
            }
        }

    }

    private void AttachLimbToHold(Rigidbody limbRigidbody)
    {
        if (limbRigidbody != null && currentFixedJoint == null)
        {
            // Create a FixedJoint and connect the limb to the hold
            FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = limbRigidbody;
            SetCurrentFixedJoint(fixedJoint);
        }
    }
}