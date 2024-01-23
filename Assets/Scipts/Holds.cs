using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Holds : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has a FixedJoint component
        FixedJoint fixedJoint = collision.collider.GetComponent<FixedJoint>();

        //if there is a fixed joint
        if (fixedJoint != null)
        {
            // Attach the character to the object with the FixedJoint
            AttachToFixedJoint(fixedJoint);
        }
    }
    private void AttachToFixedJoint(FixedJoint fixedJoint)
    {
        // Get the Rigidbody of the character
        Rigidbody characterRigidbody = GetComponent<Rigidbody>();

        if (characterRigidbody != null)
        {
            fixedJoint.connectedBody = characterRigidbody;
        }

    }
}
