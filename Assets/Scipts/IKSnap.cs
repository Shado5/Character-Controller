using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSnap : MonoBehaviour
{
    #region Public Variables

    [Header("IK Settings")]
    public bool useIk;

    [Header("IK Targets")]
    public bool leftHandIK;
    public bool rightHandIK;
    public bool leftFootIK;
    public bool rightFootIK;

    [Header("IK Positions")]
    public Vector3 leftHandPos;
    public Vector3 rightHandPos;
    public Vector3 leftFootPos;
    public Vector3 rightFootPos;

    [Header("IK Offsets")]
    public Vector3 leftHandOffset;
    public Vector3 rightHandOffset;
    public Vector3 leftFootOffset;
    public Vector3 rightFootOffset;

    [Header("IK Rotations")]
    public Quaternion leftHandRot;
    public Quaternion rightHandRot;

    [Header("Animation")]
    public Animator anim;

    [Header("Sphere Cast Settings")]
    [Tooltip("Adjust the sphere radius to control the area of detection.")]
    public float sphereRadius = 0.1f;

    [Header("Interactive Limb Movement")]
    public bool interactiveMoveEnabled = true;
    public KeyCode moveLeftLimbKey = KeyCode.L;
    public KeyCode moveRightLimbKey = KeyCode.R;

    private bool movingLeftHand;
    private bool movingRightHand;

    // New variables to store the currently targeted hold for each hand
    private GameObject leftHandTargetHold;
    private GameObject rightHandTargetHold;

    #endregion

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        CheckLeftHandIK();
        CheckRightHandIK();
        CheckLeftFootIK();
        CheckRightFootIK();
    }

    void Update()
    {
        if (interactiveMoveEnabled)
        {
            CheckLimbMovementInput();
        }

        if (movingLeftHand)
        {
            MoveLimbInteractively(AvatarIKGoal.LeftHand);
        }

        if (movingRightHand)
        {
            MoveLimbInteractively(AvatarIKGoal.RightHand);
        }
    }

    void OnAnimatorIK()
    {
        if (useIk)
        {
            ApplyIK(AvatarIKGoal.LeftHand, leftHandIK, leftHandPos, leftHandRot);
            ApplyIK(AvatarIKGoal.RightHand, rightHandIK, rightHandPos, rightHandRot);
            ApplyIK(AvatarIKGoal.LeftFoot, leftFootIK, leftFootPos, Quaternion.identity);
            ApplyIK(AvatarIKGoal.RightFoot, rightFootIK, rightFootPos, Quaternion.identity);
        }
    }

    #region Helper Methods

    void CheckLeftHandIK()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + new Vector3(0.0f, 2.0f, 0.5f), sphereRadius, -transform.up + new Vector3(-0.5f, -0.0f, 0.0f), out hit, 1f))
        {
            if (hit.transform.gameObject.CompareTag("holds"))
            {
                leftHandIK = true;
                leftHandPos = hit.point - leftHandOffset;
                leftHandRot = Quaternion.FromToRotation(-Vector3.up, hit.normal);

                leftHandTargetHold = hit.transform.gameObject;
            }
        }
        else
        {
            leftHandIK = false;
            leftHandTargetHold = null;
        }
    }

    void CheckRightHandIK()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + new Vector3(0.0f, 2.0f, 0.5f), sphereRadius, -transform.up + new Vector3(0.5f, -0.0f, 0.0f), out hit, 1f))
        {
            if (hit.transform.gameObject.CompareTag("holds"))
            {
                rightHandIK = true;
                rightHandPos = hit.point - rightHandOffset;
                rightHandRot = Quaternion.FromToRotation(-Vector3.up, hit.normal);

                rightHandTargetHold = hit.transform.gameObject;
            }
        }
        else
        {
            rightHandIK = false;
            rightHandTargetHold = null;
        }
    }

    void CheckLeftFootIK()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + new Vector3(-0.1f, 0.5f, 0.0f), sphereRadius, transform.forward, out hit, 0.5f))
        {
            leftFootPos = hit.point - leftFootOffset;
            leftFootIK = true;
        }
        else
        {
            leftFootIK = false;
        }
    }

    void CheckRightFootIK()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + new Vector3(0.1f, 0.5f, 0.0f), sphereRadius, transform.forward, out hit, 0.5f))
        {
            rightFootPos = hit.point - rightFootOffset;
            rightFootIK = true;
        }
        else
        {
            rightFootIK = false;
        }
    }

    void OnDrawGizmos()
    {
        DrawSphereCastVisualization(transform.position + new Vector3(0.0f, 2.0f, 0.5f), -transform.up + new Vector3(-0.7f, -0.0f, 0.0f), Color.green);
        DrawSphereCastVisualization(transform.position + new Vector3(0.0f, 2.0f, 0.5f), -transform.up + new Vector3(0.7f, -0.0f, 0.0f), Color.red);
        DrawSphereCastVisualization(transform.position + new Vector3(-0.1f, 0.5f, 0.0f), transform.forward, Color.blue);
        DrawSphereCastVisualization(transform.position + new Vector3(0.1f, 0.5f, 0.0f), transform.forward, Color.cyan);
    }

    void DrawSphereCastVisualization(Vector3 origin, Vector3 direction, Color color)
    {
        RaycastHit hit;
        if (Physics.SphereCast(origin, sphereRadius, direction, out hit, 1f))
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(origin + direction * hit.distance, sphereRadius);
        }
    }

    void ApplyIK(AvatarIKGoal goal, bool isIKActive, Vector3 position, Quaternion rotation)
    {
        if (isIKActive)
        {
            anim.SetIKPositionWeight(goal, 1f);
            anim.SetIKPosition(goal, position);

            anim.SetIKRotationWeight(goal, 1f);
            anim.SetIKRotation(goal, rotation);
        }
    }

    void CheckLimbMovementInput()
    {
        if (Input.GetKeyDown(moveLeftLimbKey))
        {
            movingLeftHand = !movingLeftHand;
            if (movingLeftHand)
            {
                movingRightHand = false;
            }
        }

        if (Input.GetKeyDown(moveRightLimbKey))
        {
            movingRightHand = !movingRightHand;
            if (movingRightHand)
            {
                movingLeftHand = false;
            }
        }
    }

    void MoveLimbInteractively(AvatarIKGoal goal)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (goal == AvatarIKGoal.LeftHand)
            {
                leftHandPos = hit.point - leftHandOffset;
                leftHandRot = Quaternion.FromToRotation(-Vector3.up, hit.normal);
            }
            else if (goal == AvatarIKGoal.RightHand)
            {
                rightHandPos = hit.point - rightHandOffset;
                rightHandRot = Quaternion.FromToRotation(-Vector3.up, hit.normal);
            }

            if (Input.GetMouseButton(0)) // Check if the left mouse button is held down
            {
                // Continue updating limb position while dragging
                SnapToClosestHold(goal, hit);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // Snap to closest hold when the mouse button is released
                SnapToClosestHold(goal, hit);
                // Set the limb movement boolean to false
                if (goal == AvatarIKGoal.LeftHand)
                {
                    movingLeftHand = false;
                }
                else if (goal == AvatarIKGoal.RightHand)
                {
                    movingRightHand = false;
                }
            }
        }
    }

    void SnapToClosestHold(AvatarIKGoal goal, RaycastHit hit)
    {
        GameObject targetHold = goal == AvatarIKGoal.LeftHand ? leftHandTargetHold : rightHandTargetHold;

        if (targetHold != null && targetHold.CompareTag("holds"))
        {
            Vector3 limbPosition = goal == AvatarIKGoal.LeftHand ? leftHandPos : rightHandPos;
            Vector3 limbOffset = goal == AvatarIKGoal.LeftHand ? leftHandOffset : rightHandOffset;

            RaycastHit[] hits = Physics.SphereCastAll(limbPosition + limbOffset, sphereRadius, -transform.up, 1f);

            if (hits.Length > 0)
            {
                float closestDistance = Mathf.Infinity;
                Vector3 snapPosition = Vector3.zero;

                foreach (RaycastHit holdHit in hits)
                {
                    if (holdHit.transform.gameObject.CompareTag("holds"))
                    {
                        float distance = Vector3.Distance(limbPosition, holdHit.point);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            snapPosition = holdHit.point - limbOffset;
                        }
                    }
                }

                if (closestDistance < Mathf.Infinity)
                {
                    if (goal == AvatarIKGoal.LeftHand)
                    {
                        leftHandPos = snapPosition;
                        leftHandRot = Quaternion.FromToRotation(-Vector3.up, hit.normal);
                    }
                    else if (goal == AvatarIKGoal.RightHand)
                    {
                        rightHandPos = snapPosition;
                        rightHandRot = Quaternion.FromToRotation(-Vector3.up, hit.normal);
                    }
                }
            }
        }
    }

    #endregion
}
