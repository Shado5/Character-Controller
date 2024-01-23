using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollLimbControl : MonoBehaviour
{
    [Header("Left Arm")]
    public Transform leftArm;

    [Header("Right Arm")]
    public Transform rightArm;

    [Header("Left Leg")]
    public Transform leftLeg;

    [Header("Right Leg")]
    public Transform rightLeg;

    [Header("Limb Movement Speed")]
    public float limbMoveSpeed = 5f;

    private bool movingLeftArm;
    private bool movingRightArm;
    private bool movingLeftLeg;
    private bool movingRightLeg;

    void Start()
    {
        // Enable interpolation for all rigidbodies in the ragdoll
        EnableRigidbodyInterpolation();
    }

    void Update()
    {
        CheckMouseInput();

        if (movingLeftArm)
        {
            MoveLimbInteractively(leftArm);
        }

        if (movingRightArm)
        {
            MoveLimbInteractively(rightArm);
        }

        if (movingLeftLeg)
        {
            MoveLimbInteractively(leftLeg);
        }

        if (movingRightLeg)
        {
            MoveLimbInteractively(rightLeg);
        }
    }

    void CheckMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            movingLeftArm = true;
        }

        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            movingRightArm = true;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            movingLeftLeg = !movingLeftLeg;
            if (movingLeftLeg)
            {
                movingRightLeg = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            movingRightLeg = !movingRightLeg;
            if (movingRightLeg)
            {
                movingLeftLeg = false;
            }
        }

        if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            movingLeftArm = false;
        }

        if (Input.GetMouseButtonUp(1)) // Right mouse button released
        {
            movingRightArm = false;
        }
    }

    void MoveLimbInteractively(Transform limb)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 targetPosition = hit.point;
            Vector3 direction = (targetPosition - limb.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            limb.position = Vector3.MoveTowards(limb.position, targetPosition, limbMoveSpeed * Time.deltaTime);
            limb.rotation = Quaternion.RotateTowards(limb.rotation, targetRotation, limbMoveSpeed * Time.deltaTime);
        }
    }

    void EnableRigidbodyInterpolation()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate; // or RigidbodyInterpolation.Extrapolate
        }
    }
}
