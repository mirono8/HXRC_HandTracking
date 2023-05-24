using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FROM AUTOHAND
/// </summary>
public class JoystickObjectMover : JoystickPhysics
{
    public Transform move;
    public float speed = 2;

    ObjectStateTracker stateTracker;

  /*  private void Awake()
    { 
        
        stateTracker = GetComponent<ObjectStateTracker>();
        
    }
    void Update()
    {
        if (stateTracker.IsGrabbed())
        {
            move = stateTracker.GrabbedBy();

            var axis = GetValue();
            var moveAxis = new Vector3(axis.x * Time.deltaTime * speed, 0, axis.y * Time.deltaTime * speed);
            move.transform.localPosition += moveAxis;
            //moveAxis =+ stateTracker.GrabbedBy().handPosition;
        }
    }*/
}

