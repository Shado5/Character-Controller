using UnityEngine;

public class RagdollLimbControl : MonoBehaviour
{
    // Variables to store the selected limb and track whether a limb is being moved
    private Transform selectedLimb;
    private bool isMovingLimb;
    private bool isSpineSelected;
    public Vector3 slingshotForce;

    // Speed at which the limb moves interactively
    public float moveSpeed = 5f;

    public float slingshotSpeed = 1f;
    public float constantShootForce = 20.0f;

    // Variable to control the maximum slingshot force
    public float maxSlingshotForce = 10f;

    public GameObject arrowPrefab;
    private GameObject arrowInstance;

    public int attachedLimbsCount;

    public float slingshotStrengthMultiplier = 2.0f;

    // Update is called once per frame
    void Update()
    {
        // Check for user input and move the limb if it's selected
        CheckInput();

        if (isMovingLimb && selectedLimb != null)
        {
            MoveLimbInteractively(selectedLimb, moveSpeed);
            UpdateArrowDirection(selectedLimb);
        }

        if (isSpineSelected && Input.GetMouseButtonUp(0))
        {
            // Release the spine and apply slingshot force
            ShootPlayer();
            isSpineSelected = false;
        }
    }

    void UpdateArrowDirection(Transform limb)
    {
        // Check if the arrow is instantiated
        if (arrowInstance != null)
        {
            // Calculate the direction from the spine to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // Reverse the direction vector
                Vector3 targetDirection = (limb.position - hit.point).normalized;

                // Calculate the rotation to point the arrow in the calculated direction
                Quaternion arrowRotation = Quaternion.LookRotation(Vector3.forward, targetDirection);

                // Set only the Z-component of the rotation
                arrowInstance.transform.rotation = Quaternion.Euler(0f, 0f, arrowRotation.eulerAngles.z);
            }
        }
    }

    // Method to shoot the player when releasing the spine
    void ShootPlayer()
    {
        // Check if the selected limb is the spine
        if (selectedLimb.CompareTag("Spine"))
        {
            // Check if there are two or more limbs attached to holds
            if (attachedLimbsCount >= 2)
            {
                Rigidbody spineRigidbody = selectedLimb.GetComponent<Rigidbody>();

                if (spineRigidbody != null)
                {
                    // Reset the slingshot force
                    slingshotForce = Vector3.zero;

                    // Apply a constant force for shooting (adjust the value as needed)
                    spineRigidbody.AddForce(Vector3.up * constantShootForce, ForceMode.Impulse);

                    // Debug log to check if the method is getting called
                    Debug.Log("Shooting Player!");

                    // Deselect the limb (release the spine)
                    DeselectLimb();

                    // Delayed detachment of limbs from holds
                    Invoke("DetachLimbsFromHolds", 0.2f);
                }
            }
            else
            {
                Debug.Log("Cannot shoot: Need at least two limbs attached to holds. Currently attached: " + attachedLimbsCount);
            }
        }
    }



    // Method to detach all limbs from holds
    void DetachLimbsFromHolds()
    {
        // Iterate through all limbs and detach them from holds
        foreach (Transform limb in GetComponentsInChildren<Transform>())
        {
            if (limb.CompareTag("limbs"))
            {
                Holds currentHold = limb.GetComponentInChildren<Holds>();
                if (currentHold != null && currentHold.CurrentFixedJoint != null)
                {
                    DisconnectLimbFromHold(limb, currentHold);
                }
            }
        }
    }

    // Method to move the selected limb interactively towards the mouse position
    void MoveLimbInteractively(Transform limb, float slingshotSpeed)
    {
        // Check if the selected limb is the spine
        if (limb.CompareTag("Spine"))
        {
            // Check if there are two or more limbs attached to holds
            if (attachedLimbsCount >= 2)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Vector3 targetPosition = hit.point;
                    Vector3 direction = (targetPosition - limb.position).normalized;

                    // Calculate force based on the direction and slingshotSpeed
                    slingshotForce = direction * Mathf.Clamp(slingshotSpeed, 0f, maxSlingshotForce);


                    // Apply force to the limb's rigidbody
                    Rigidbody limbRigidbody = limb.GetComponent<Rigidbody>();
                    if (limbRigidbody != null)
                    {
                        // Clear existing forces before applying the new force
                        limbRigidbody.velocity = Vector3.zero;
                        limbRigidbody.angularVelocity = Vector3.zero;

                        limbRigidbody.AddForce(slingshotForce, ForceMode.Impulse);
                    }
                }
            }
            // If fewer than two limbs are attached to holds, prevent spine movement
            else
            {
                Debug.Log("Cannot move spine: Need at least two limbs attached to holds. Currently attached: " + attachedLimbsCount);
            }
        }
        else
        {
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
                    isSpineSelected = true;

                    // Instantiate the arrow when the spine is selected
                    InstantiateArrow(limb);
                }
            }
        }
    }

    // Method to deselect the currently selected limb
    void DeselectLimb()
    {
        isMovingLimb = false;
        selectedLimb = null;

        // Destroy the arrow when the spine is deselected
        DestroyArrow();
    }

    // Method to instantiate the arrow when the spine is selected
    void InstantiateArrow(Transform spine)
    {
        // Instantiate the arrow prefab
        arrowInstance = Instantiate(arrowPrefab, spine.position + new Vector3(0f, 0f, -0.20f), Quaternion.identity);

        // Optionally, you can parent the arrow to the spine for easier management
        arrowInstance.transform.parent = spine;
    }

    // Method to destroy the arrow when the spine is deselected
    void DestroyArrow()
    {
        if (arrowInstance != null)
        {
            Destroy(arrowInstance);
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

            if (attachedLimbsCount >= 1)
            {
                attachedLimbsCount--;
            }
        }
    }
}
