using HandData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickGrabbing : MonoBehaviour
{

    Transform grabbedBody;

    Vector3 initialPos;

    public Transform followChild;

    bool beingTouched;

    public List<Collider> touchers;

    public InteractableActivityManager interactableActivityManager;

    public BoxCollider target;

    private void Start()
    {
        initialPos = followChild.transform.position;
        grabbedBody = GetComponentInChildren<Rigidbody>().transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other == target)
        {
            Debug.Log("we're done here");
            interactableActivityManager.LeverPulled();
        }

        if (other.CompareTag("FingerCollider") && !other.isTrigger)
        {
            beingTouched = true;
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
        //grabbedBody.transform.position = followChild.transform.position;

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
