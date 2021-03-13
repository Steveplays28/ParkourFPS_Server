using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Hovercraft : MonoBehaviour
{
    private Rigidbody rb;

    public Transform[] hoverPoints;
    public float hoverHeight;
    public float hoverForce;
    public float errorCorrectionForce;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        foreach (Transform hoverPoint in hoverPoints)
        {
            if (Physics.Raycast(hoverPoint.position, -hoverPoint.up, out RaycastHit hit, hoverHeight))
            {
                Debug.DrawLine(hoverPoint.position, hit.point, Color.green, Time.deltaTime);

                if (hoverPoint.position.y < transform.position.y)
                {
                    rb.AddForceAtPosition(Vector3.up * errorCorrectionForce, hoverPoint.position);
                }
                else
                {
                    rb.AddForceAtPosition(hoverPoint.up * hoverForce/* * (1.0f - (hit.distance / hoverHeight))*/, hoverPoint.position);
                }
            }
            else
            {
                Debug.DrawLine(hoverPoint.position, -hoverPoint.up * 1000, Color.red, Time.deltaTime);
                if (hoverPoint.position.y < transform.position.y)
                {
                    rb.AddForceAtPosition(Vector3.up * errorCorrectionForce, hoverPoint.position);
                }
            }
        }
    }
}
