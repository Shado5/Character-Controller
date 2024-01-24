using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollLimbControl : MonoBehaviour
{
    [Header("Left Arm")]
    public Transform leftArm;
    public float leftArmMoveSpeed = 5f;

    [Header("Right Arm")]
    public Transform rightArm;
    public float rightArmMoveSpeed = 5f;

    [Header("Left Leg")]
    public Transform leftLeg;
    public float leftLegMoveSpeed = 5f;

    [Header("Right Leg")]
    public Transform rightLeg;
    public float rightLegMoveSpeed = 5f;

    private bool movingLeftArm;
    private bool movingRightArm;
    private bool movingLeftLeg;
    private bool movingRightLeg;

    void Update()
    {
        CheckInput();

        MoveLimbInteractively(leftArm, movingLeftArm, leftArmMoveSpeed);
        MoveLimbInteractively(rightArm, movingRightArm, rightArmMoveSpeed);
        MoveLimbInteractively(leftLeg, movingLeftLeg, leftLegMoveSpeed);
        MoveLimbInteractively(rightLeg, movingRightLeg, rightLegMoveSpeed);
    }

    void CheckInput()
    {
        // Handle input for limb movement
        HandleLimbInput(KeyCode.Alpha1, ref movingLeftArm);
        HandleLimbInput(KeyCode.Alpha2, ref movingRightArm);
        HandleLimbInput(KeyCode.Alpha3, ref movingLeftLeg);
        HandleLimbInput(KeyCode.Alpha4, ref movingRightLeg);
    }

    void HandleLimbInput(KeyCode key, ref bool movingFlag)
    {
        if (Input.GetKeyDown(key))
        {
            movingFlag = true;
        }

        if (Input.GetKeyUp(key))
        {
            movingFlag = false;
            AttachLimbToHold(key);
        }
    }

    void MoveLimbInteractively(Transform limb, bool isMoving, float moveSpeed)
    {
        Holds currentHold = limb.GetComponentInChildren<Holds>();

        if (isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 targetPosition = hit.point;
                Vector3 direction = (targetPosition - limb.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

                // Check if the limb is connected to a hold
                if (currentHold != null && currentHold.CurrentFixedJoint != null)
                {
                    // Disconnect the limb from the current hold
                    DisconnectLimbFromHold(limb, currentHold);
                }

                // Move the limb
                limb.position = Vector3.MoveTowards(limb.position, targetPosition, moveSpeed * Time.deltaTime);
                limb.rotation = Quaternion.RotateTowards(limb.rotation, targetRotation, moveSpeed * Time.deltaTime);
            }
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

    void AttachLimbToHold(KeyCode key)
    {
        Transform limb = GetLimbByKey(key);
        Holds hold = limb.GetComponentInChildren<Holds>();

        if (hold != null)
        {
            // Check if the hold has the correct tag
            if (hold.gameObject.CompareTag("holds"))
            {
                // Create a FixedJoint if it doesn't exist
                if (hold.CurrentFixedJoint == null)
                {
                    FixedJoint fixedJoint = hold.gameObject.AddComponent<FixedJoint>();
                    hold.SetCurrentFixedJoint(fixedJoint);
                }

                // Connect the limb to the hold with the existing FixedJoint
                hold.CurrentFixedJoint.connectedBody = limb.GetComponent<Rigidbody>();
            }
            else
            {
                Debug.LogWarning("Attempted to attach limb to an object without the 'holds' tag.");
            }
        }
    }

    Transform GetLimbByKey(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Alpha1:
                return leftArm;
            case KeyCode.Alpha2:
                return rightArm;
            case KeyCode.Alpha3:
                return leftLeg;
            case KeyCode.Alpha4:
                return rightLeg;
            default:
                return null;
        }
    }
}
