using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandData;
using System;
using static UnityEngine.GraphicsBuffer;

public class FingerCollisionDetector : MonoBehaviour
{
    
    HandDataOut handData;
    public TrackColliders colliders;
    public TrackColliders.CollisionEvent collisionEvent = new TrackColliders.CollisionEvent();
    // TrackColliders.FingertipsColliders fingerTipColliderTip;
    int fingertipListIndex;

    //Track fader status and change colliders to triggers accordingly

    SessionManager sessionManager;
    private enum ThisHand
    {
        left,right
    }

    ThisHand thisHand;

    [SerializeField]
    Vector3 collisionOffset;

    [SerializeField]
    bool activeVisualCollision;

    Collider currentlyCollidingWith;

    [SerializeField]
    bool staying;

    [SerializeField]
    Vector3 othersVector;
    private void Start()
    {
        handData = GameObject.FindGameObjectWithTag("HandData").GetComponent<HandDataOut>();
        colliders = GetComponentInParent<TrackColliders>();


        if (GameObject.FindGameObjectWithTag("SessionManager"))
        {
            if (GameObject.FindGameObjectWithTag("SessionManager").TryGetComponent<SessionManager>(out SessionManager sessionManagerSuccess))
            {
                if (sessionManagerSuccess)
                {
                    sessionManager = sessionManagerSuccess;
                }
            }
        }


    }

    public Vector3 GetCollisionOffset()
    {
        return collisionOffset;
    }

    public bool IsActiveVisualCollision()
    {
        return activeVisualCollision;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (sessionManager != null)
        {
            if (!other.CompareTag("IgnoreCollisionSaves"))
            {
                if (other.gameObject.GetComponentInParent<CollidableObjects>() && sessionManager.CurrentState() == States.State.Active && !staying)
                {

                    if (colliders.handedness == TrackColliders.Hand.left)
                    {
                        thisHand = ThisHand.left;
                        var collision = colliders.tipSphereColliders.Find(x => gameObject.GetComponent<SphereCollider>() == x);

                        if (collision != null)
                        {

                            for (int i = 0; i < colliders.tipSphereColliders.Count; i++)
                            {

                                if (handData.leftHand.fingers.trackColliders.tipSphereColliders[i].Equals(collision))
                                {
                                    activeVisualCollision = true;
                                    collisionOffset = transform.position + other.transform.position;
                                    //handData.leftHand.fingers.trackColliders.tipColliders[i].colliding = true;
                                    handData.leftHand.fingerColliders[i].colliding = true;
                                    //fingerTipColliderTip = colliders.fingertips[i];
                                    fingertipListIndex = i;
                                    // Debug.Log(fingerTipColliderTip.finger);
                                    currentlyCollidingWith = other;
                                    collisionEvent = new()
                                    {
                                        startTime = DateTime.Now.ToString("HH-mm-ss"),
                                        collidingFinger = handData.leftHand.fingerColliders[i].fingerName,
                                        otherCollider = other.gameObject.name
                                    };
                                }
                            }
                        }
                    }

                    if (colliders.handedness == TrackColliders.Hand.right)
                    {
                        thisHand = ThisHand.right;
                        var collision = colliders.tipSphereColliders.Find(x => gameObject.GetComponent<SphereCollider>() == x);

                        if (collision != null)
                        {

                            for (int i = 0; i < colliders.tipSphereColliders.Count; i++)
                            {

                                if (handData.rightHand.fingers.trackColliders.tipSphereColliders[i].Equals(collision))
                                {
                                    Vector3 tip = handData.rightHand.fingerColliders[i].fingerTipPosition;
                                    activeVisualCollision = true;
                                    float distance = Vector3.Distance(tip, other.transform.position);
                                    float floatX = tip.x - other.transform.position.x;
                                    float floatY = tip.y - other.transform.position.y;
                                    float floatZ = tip.z - other.transform.position.z;

                                    //var y = Vector3.MoveTowards(tip, other.transform.position, 1);
                                    //var x = handData.rightHand.fingers.trackColliders.tipSphereColliders[i].radius;
                                    collisionOffset = new Vector3(floatX, floatY, floatZ);

                                    
                                    Debug.Log("collision offset " + collisionOffset);
                                   // Debug.Log(tip + " is the tip position");
                                    //Debug.Log(floatX + ", " + floatY + ", " + floatZ  + " is the distance between tip and other collider");
                                    //collisionOffset = new Vector3(tip.x + distance, tip.y + distance, tip.z + distance);
                                    //handData.leftHand.fingers.trackColliders.tipColliders[i].colliding = true;
                                    handData.rightHand.fingerColliders[i].colliding = true;
                                    //fingerTipColliderTip = colliders.fingertips[i];
                                    fingertipListIndex = i;
                                    currentlyCollidingWith = other;
                                    // Debug.Log(fingerTipColliderTip.finger);
                                    Debug.Log("SAVING NEW COLLISION EVENT TO JSON");
                                    collisionEvent = new()
                                    {
                                        startTime = DateTime.Now.ToString("HH-mm-ss"),
                                        collidingFinger = handData.rightHand.fingerColliders[i].fingerName,
                                        otherCollider = other.gameObject.name
                                    };
                                }
                            }
                        }
                    }

                    var interactableByType = other.gameObject.GetComponentInParent<InteractableActivityManager>();


                    if (interactableByType != null)
                    {
                        if (interactableByType.type == InteractableActivityManager.InteractableType.Button)
                        {
                            Debug.Log("button pressed by " + gameObject.name);
                            interactableByType.interactor = (InteractableActivityManager.Interactor)thisHand;
                        }
                    }

                    /*  if (colliders.handedness == TrackColliders.Hand.right)   ----------------OLD----------------------------
                      {
                          var collision = handData.rightHand.fingers.trackColliders.tipCollidersLeft.Find(x => gameObject.GetComponent<SphereCollider>() == x);

                          if (collision != null)
                          {


                              for (int i = 0; i < colliders.tipCollidersLeft.Count; i++)
                              {

                                  if (colliders.tipCollidersLeft.Equals(collision))
                                  {

                                      handData.rightHand.fingers.trackColliders.tipCollidersLeft[i].colliding = true;
                                      //fingerTipColliderTip = colliders.fingertips[i];
                                      fingertipListIndex = i;
                                     // Debug.Log(fingerTipColliderTip.finger);

                                  }
                              }
                          }
                      }*/
                }

            }
        }
    }

    

    private void OnTriggerExit(Collider other)
    {
       
        if (sessionManager != null)
        {

            if (!other.CompareTag("IgnoreCollisionSaves"))
            {
                if (other.gameObject.transform.GetComponentInParent<CollidableObjects>() && sessionManager.CurrentState() == States.State.Active)
                {
                    staying = false;
                    activeVisualCollision = false;
                    collisionOffset = Vector3.zero;
                    // colliders.tipColliders[fingertipListIndex].colliding = false;
                    if ((int)thisHand == (int)handData.leftHand.myHandedness)
                    {
                        handData.leftHand.fingerColliders[fingertipListIndex].colliding = false;
                        handData.SendCollisionData(handData.leftHand, collisionEvent);
                        //CleanCollisionEvent();
                    }
                    if ((int)thisHand == (int)handData.rightHand.myHandedness)
                    {
                        handData.rightHand.fingerColliders[fingertipListIndex].colliding = false;
                        handData.SendCollisionData(handData.rightHand, collisionEvent);
                        //  CleanCollisionEvent();
                    }
                }
            }
        }
    }

    private void CleanCollisionEvent()
    {
        if (collisionEvent != null) 
        {
            collisionEvent.startTime = null;
            collisionEvent.collidingFinger = null;
            collisionEvent.otherCollider = null;
        }
    }


}
