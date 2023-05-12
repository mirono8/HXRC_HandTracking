using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandData;
using System;

public class FingerCollisionDetector : MonoBehaviour
{
    
    HandDataOut handData;
    public TrackColliders colliders;
    public TrackColliders.CollisionEvent collisionEvent = new TrackColliders.CollisionEvent();
    // TrackColliders.FingertipsColliders fingerTipColliderTip;
    int fingertipListIndex;

    private enum ThisHand
    {
        left,right
    }

    ThisHand thisHand;
    private void Start()
    {
        handData = GameObject.FindGameObjectWithTag("HandData").GetComponent<HandDataOut>();
        colliders = GetComponentInParent<TrackColliders>();
        
    }
    private void OnTriggerEnter(Collider other)
    {
        

        if (other.gameObject.GetComponentInParent<CollidableObjects>())
        {
            
            if (colliders.handedness == TrackColliders.Hand.left)
            {
                thisHand = ThisHand.left;
                var collision = colliders.sphereColliders.Find(x => gameObject.GetComponent<SphereCollider>() == x);

                if (collision != null)
                {

                    for (int i = 0; i < colliders.sphereColliders.Count; i++)
                    {

                        if (handData.leftHand.fingers.trackColliders.sphereColliders[i].Equals(collision))
                        {
                           
                            //handData.leftHand.fingers.trackColliders.tipColliders[i].colliding = true;
                            handData.leftHand.fingertips[i].colliding = true;
                            //fingerTipColliderTip = colliders.fingertips[i];
                            fingertipListIndex = i;
                            // Debug.Log(fingerTipColliderTip.finger);
                            collisionEvent.startTime = DateTime.Now.ToString("HH-mm-ss");
                            collisionEvent.collidingFinger = handData.leftHand.fingertips[i].fingerName;
                            collisionEvent.otherCollider = other.gameObject.name;
                        }
                    }
                }
            }

            if (colliders.handedness == TrackColliders.Hand.right)
            {
                thisHand = ThisHand.right;
                var collision = colliders.sphereColliders.Find(x => gameObject.GetComponent<SphereCollider>() == x);

                if (collision != null)
                {

                    for (int i = 0; i < colliders.sphereColliders.Count; i++)
                    {

                        if (handData.rightHand.fingers.trackColliders.sphereColliders[i].Equals(collision))
                        {
                           
                            //handData.leftHand.fingers.trackColliders.tipColliders[i].colliding = true;
                            handData.rightHand.fingertips[i].colliding = true;
                            //fingerTipColliderTip = colliders.fingertips[i];
                            fingertipListIndex = i;
                            // Debug.Log(fingerTipColliderTip.finger);
                            collisionEvent.startTime = DateTime.Now.ToString("HH-mm-ss");
                            collisionEvent.collidingFinger = handData.leftHand.fingertips[i].fingerName;
                            collisionEvent.otherCollider = other.gameObject.name;
                        }
                    }
                }
            }

            /*var interactableByType = other.gameObject.GetComponentInParent<InteractableActivityManager>();


            if (interactableByType != null)
            {
                if (interactableByType.type == InteractableActivityManager.InteractableType.Button)
                {
                    Debug.Log("button pressed by " + gameObject.name);
                    interactableByType.ButtonPressed(gameObject);
                }
            }*/

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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.GetComponentInParent<CollidableObjects>())
        {
            // colliders.tipColliders[fingertipListIndex].colliding = false;
            if ((int)thisHand == (int)handData.leftHand.myHandedness)
            {
                handData.leftHand.fingertips[fingertipListIndex].colliding = false;
                handData.SendCollisionData(handData.leftHand,  collisionEvent);
                CleanCollisionEvent();
            }
            if ((int)thisHand == (int)handData.rightHand.myHandedness)
            {
                handData.rightHand.fingertips[fingertipListIndex].colliding = false;
                handData.SendCollisionData(handData.rightHand, collisionEvent);
                CleanCollisionEvent();
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
