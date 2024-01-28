using UnityEngine;

public class Holds : MonoBehaviour
{
    // Variable to store the current FixedJoint
    private FixedJoint currentFixedJoint;

    // Property to access the current FixedJoint from outside the class
    public FixedJoint CurrentFixedJoint
    {
        get { return currentFixedJoint; }
    }

    // Method to set the current FixedJoint
    public void SetCurrentFixedJoint(FixedJoint fixedJoint)
    {
        currentFixedJoint = fixedJoint;
    }

    // Called when the Collider enters another Collider trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entered object has the tag "holds"
        if (other.gameObject.CompareTag("holds"))
        {
            // Check if the other object has a Rigidbody component
            Rigidbody collidedRigidbody = other.GetComponent<Rigidbody>();

            // If there is a Rigidbody
            if (collidedRigidbody != null)
            {
                // Attach the limb to the hold with a FixedJoint
                AttachLimbToHold(collidedRigidbody);
            }
        }
    }

    // Method to attach the limb to the hold using a FixedJoint
    private void AttachLimbToHold(Rigidbody limbRigidbody)
    {
        // Check if the limbRigidbody is not null and there is no existing FixedJoint
        if (limbRigidbody != null && currentFixedJoint == null)
        {
            // Create a FixedJoint and connect the limb to the hold
            FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = limbRigidbody;
            SetCurrentFixedJoint(fixedJoint);

        }
    }
}