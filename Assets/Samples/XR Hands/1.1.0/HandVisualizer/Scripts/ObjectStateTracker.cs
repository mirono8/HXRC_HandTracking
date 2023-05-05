using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandData;
using System.Diagnostics;

public class ObjectStateTracker : MonoBehaviour
{
    Vector3 initialPosition;
    Quaternion initialRotation;

    [HideInInspector]
    public float timer;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (transform.position != initialPosition && timer > 5)
            ReturnHome();
    }

    public void ReturnHome()
    {
        timer = 0;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FingerCollider"))
        {
            timer = 0;
            if(transform.position != initialPosition)
            {
                other.gameObject.GetComponentInParent<HandDataOut.Hand>().isGrabbing = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FingerCollider"))
        {
            other.gameObject.GetComponentInParent<HandDataOut.Hand>().isGrabbing = false;

        }
    }
}

