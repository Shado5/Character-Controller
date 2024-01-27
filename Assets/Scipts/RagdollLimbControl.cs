using UnityEngine;

public class RagdollLimbControl : MonoBehaviour
{
    private Transform selectedLimb;
    private bool isMovingLimb;

    public float moveSpeed = 5f;

    void Update()
    {
        CheckInput();

        if (isMovingLimb && selectedLimb != null)
        {
            MoveLimbInteractively(selectedLimb, moveSpeed);
        }
    }

    void CheckInput()
    {
        // Handle input for limb selection and deselection
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
        }
    }

    void DeselectLimb()
    {
        isMovingLimb = false;
        selectedLimb = null;
    }

    void MoveLimbInteractively(Transform limb, float moveSpeed)
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
