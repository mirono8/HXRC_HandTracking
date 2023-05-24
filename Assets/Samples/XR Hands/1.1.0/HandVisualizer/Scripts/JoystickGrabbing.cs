using HandData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickGrabbing : MonoBehaviour
{
    ObjectStateTracker stateTracker;

    Transform grabbedBody;

    Vector3 initialPos;

    public Transform followChild;

    bool beingTouched;

    public List<Collider> touchers;

    HandDataOut handDataOut;


    private void Start()
    {
        initialPos = followChild.transform.position;
        stateTracker = GetComponent<ObjectStateTracker>();
        grabbedBody = GetComponentInChildren<Rigidbody>().transform;
        handDataOut = GameObject.FindGameObjectWithTag("HandData").GetComponent<HandDataOut>();
    }

    private void OnTriggerEnter(Collider other)
    {

        beingTouched = true;

        if (other.CompareTag("FingerCollider") && !other.isTrigger)
        {
            if (!touchers.Contains(other) )
            {
                touchers.Add(other);
            }
        }         
    }

    private void OnTriggerExit(Collider other)
    {
        if(touchers.Count == 0)
            beingTouched=false;

        if (touchers.Contains(other))
        {
            touchers.Remove(other);
        }
    }

    private void Update()
    {
        grabbedBody.transform.position = followChild.transform.position;

        if (touchers.Count > 0 && beingTouched)
        {

            //followChild.transform.LookAt(touchers[0].transform.position);
            followChild.transform.position = touchers[0].transform.position; //Vector3.MoveTowards(followChild.transform.position, followChild.forward ,0.05f); 
        }
        else
        {
            followChild.transform.position = Vector3.Slerp(followChild.transform.position, initialPos, Time.deltaTime);
        }
        
    }
}
