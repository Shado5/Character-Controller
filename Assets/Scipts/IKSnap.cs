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

    #endregion

    void Start()
    {
        // Get the Animator component at the start
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // Perform sphere casts to detect holds and update IK targets
        CheckLeftHandIK();
        CheckRightHandIK();
        CheckLeftFootIK();
        CheckRightFootIK();
    }

    void Update()
    {
        // No longer visualizing sphere casts in Update
    }

    void OnAnimatorIK()
    {
        // Apply IK positions and rotations if enabled
        if (useIk)
        {
            ApplyIK(AvatarIKGoal.LeftHand, leftHandIK, leftHandPos, leftHandRot);
            ApplyIK(AvatarIKGoal.RightHand, rightHandIK, rightHandPos, rightHandRot);
            ApplyIK(AvatarIKGoal.LeftFoot, leftFootIK, leftFootPos, Quaternion.identity);
            ApplyIK(AvatarIKGoal.RightFoot, rightFootIK, rightFootPos, Quaternion.identity);
        }
    }

    #region Helper Methods

    // Sphere cast and update left hand IK data
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
            }
        }
        else
        {
            leftHandIK = false;
        }
    }

    // Sphere cast and update right hand IK data
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
            }
        }
        else
        {
            rightHandIK = false;
        }
    }

    // Sphere cast and update left foot IK data
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

    // Sphere cast and update right foot IK data
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

    // Visualize sphere casts for debugging
    void OnDrawGizmos()
    {
        // Visualize sphere casts for each IK target
        DrawSphereCastVisualization(transform.position + new Vector3(0.0f, 2.0f, 0.5f), -transform.up + new Vector3(-0.5f, -0.0f, 0.0f), Color.green);
        DrawSphereCastVisualization(transform.position + new Vector3(0.0f, 2.0f, 0.5f), -transform.up + new Vector3(0.5f, -0.0f, 0.0f), Color.red);
        DrawSphereCastVisualization(transform.position + new Vector3(-0.1f, 0.5f, 0.0f), transform.forward, Color.blue);
        DrawSphereCastVisualization(transform.position + new Vector3(0.1f, 0.5f, 0.0f), transform.forward, Color.cyan);
    }

    // Visualize a sphere cast and draw a wireframe sphere if hit
    void DrawSphereCastVisualization(Vector3 origin, Vector3 direction, Color color)
    {
        RaycastHit hit;
        if (Physics.SphereCast(origin, sphereRadius, direction, out hit, 1f))
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(origin + direction * hit.distance, sphereRadius);
        }
    }

    // Apply IK positions and rotations to the animator
    void ApplyIK(AvatarIKGoal goal, bool isIKActive, Vector3 position, Quaternion rotation)
    {
        // Apply IK if the target is active
        if (isIKActive)
        {
            anim.SetIKPositionWeight(goal, 1f);
            anim.SetIKPosition(goal, position);

            anim.SetIKRotationWeight(goal, 1f);
            anim.SetIKRotation(goal, rotation);
        }
    }

    #endregion
}
