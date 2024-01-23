using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{

    public float sphereRadius = 0.1f;
    void update()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + new Vector3(0.0f, 2.0f, 0.5f), sphereRadius, -transform.up + new Vector3(-0.5f, -0.0f, 0.0f), out hit, 1f))
        {
            DrawSphereCastVisualization(transform.position + new Vector3(0.0f, 2.0f, 0.5f), -transform.up + new Vector3(-0.5f, -0.0f, 0.0f), Color.green);
        }
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

}
