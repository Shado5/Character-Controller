using UnityEngine;

public class RagdollLimbControl : MonoBehaviour
{
    // Variables to store the selected limb and track whether a limb is being moved
    private Transform selectedLimb;
    private bool isMovingLimb;

    // Speed at which the limb moves interactively
    public float moveSpeed = 5f;

    public float slingshotSpeed = 1f;

    //variable to control the maximum slingshot force
    public float maxSlingshotForce = 10f;

    // Update is called once per frame
    void Update()
    {
        // Check for user input and move the limb if it's selected
        CheckInput();

        if (isMovingLimb && selectedLimb != null)
        {
            MoveLimbInteractively(selectedLimb, moveSpeed);
        }
    }

    // Method to check user input for limb selection and deselection
    void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isMovingLimb)
            {
                // Select limb when left mouse button is clicked
                SelectLimb();
            }
            else
            {
                // Deselect limb when left mouse button is clicked again
                DeselectLimb();
            }
        }
    }

    // Method to select a limb based on a raycast from the mouse position
    void SelectLimb()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("limbs"))
            {
                // Check if the hit object has a limb
                Transform limb = hit.collider.GetComponent<Transform>();

                if (limb != null)
                {
                    // Set the selected limb
                    selectedLimb = limb;
                    isMovingLimb = true;

                    // Detach the limb from the hold if it's connected
                    Holds currentHold = limb.GetComponentInChildren<Holds>();
                    if (currentHold != null && currentHold.CurrentFixedJoint != null)
                    {
                        DisconnectLimbFromHold(limb, currentHold);
                    }
                }
            }
            if (hit.collider.CompareTag("Spine"))
            {
                Debug.Log("hitSpine");

                // Check if the hit object has a limb
                Transform limb = hit.collider.GetComponent<Transform>();
                if (limb != null)
                {
                    // Set the selected limb
                    selectedLimb = limb;
                    isMovingLimb = true;
                }
            }
        }
    }

    // Method to deselect the currently selected limb
    void DeselectLimb()
    {
        isMovingLimb = false;
        selectedLimb = null;
    }

    // Method to move the selected limb interactively towards the mouse position
    void MoveLimbInteractively(Transform limb, float slingshotSpeed)
    {
        // Check if the selected limb is the spine
        if (limb.CompareTag("Spine"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 targetPosition = hit.point;
                Vector3 direction = (targetPosition - limb.position).normalized;

                // Calculate force based on the direction and slingshotSpeed
                Vector3 force = direction * Mathf.Clamp(slingshotSpeed, 0f, maxSlingshotForce);

                // Apply force to the limb's rigidbody
                Rigidbody limbRigidbody = limb.GetComponent<Rigidbody>();
                if (limbRigidbody != null)
                {
                    // Clear existing forces before applying the new force
                    limbRigidbody.velocity = Vector3.zero;
                    limbRigidbody.angularVelocity = Vector3.zero;

                    limbRigidbody.AddForce(force, ForceMode.Impulse);
                }
            }
        }
        else
        {
            // Move the limb using MoveTowards for non-spine limbs
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 targetPosition = hit.point;
                Vector3 direction = (targetPosition - limb.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

                // Move the limb
                limb.position = Vector3.MoveTowards(limb.position, targetPosition, moveSpeed * Time.deltaTime);
                limb.rotation = Quaternion.RotateTowards(limb.rotation, targetRotation, moveSpeed * Time.deltaTime);
            }
        }
    }

    // Method to disconnect the limb from the hold it may be attached to
    void DisconnectLimbFromHold(Transform limb, Holds currentHold)
    {
        FixedJoint fixedJoint = currentHold.CurrentFixedJoint;

        if (fixedJoint != null)
        {
            // Detach the limb from the hold by setting the connected body to null
            fixedJoint.connectedBody = null;

            // Set the currentFixedJoint to null
            currentHold.SetCurrentFixedJoint(null);

            // Destroy the FixedJoint component after a delay to allow physics to update
            Destroy(fixedJoint, 0.1f);
        }
    }
}

