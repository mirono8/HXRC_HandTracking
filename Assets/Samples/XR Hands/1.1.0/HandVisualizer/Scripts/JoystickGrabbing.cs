using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickGrabbing : MonoBehaviour
{
    ObjectStateTracker stateTracker;

    CapsuleCollider capsuleCollider;

    Vector3 initialPos;
    private void Start()
    {
        initialPos = transform.position;
        capsuleCollider = GetComponent<CapsuleCollider>();
        stateTracker = GetComponent<ObjectStateTracker>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        if (other.CompareTag("FingerCollider"))
        {
            if (stateTracker.GrabbedBy() != null)
            {
                Debug.Log("grabbing");
                
                capsuleCollider.enabled = false;
            }
        }
    }
    private void Update()
    {
        if (stateTracker.GrabbedBy() != null)
        {
            gameObject.transform.position = stateTracker.GrabbedBy().position;
        }
        else
        {
            gameObject.transform.position = Vector3.Slerp(transform.position, initialPos, Time.deltaTime);
            capsuleCollider.enabled=true;
        }
    }

}
