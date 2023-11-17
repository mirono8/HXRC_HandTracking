using HandData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustHeight : MonoBehaviour
{
    bool colliding;
    private void OnTriggerEnter(Collider other)
    {
        if (!colliding)
        {
            if (other.CompareTag("FingerCollider"))
            {


                var finger = other.gameObject.GetComponentInParent<TrackColliders>();

                var hand = FindObjectOfType<HandDataOut>();

                if ((int)hand.leftHand.myHandedness == (int)finger.GetMyHandedness(finger))
                {
                    hand.leftHand.isGrabbing = true;
                    colliding = true;

                }
                else if ((int)hand.rightHand.myHandedness == (int)finger.GetMyHandedness(finger))
                {
                    hand.rightHand.isGrabbing = true;
                    colliding = true;


                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FingerCollider"))
        {
            var finger = other.gameObject.GetComponentInParent<TrackColliders>();

            var hand = FindObjectOfType<HandDataOut>();

            if ((int)hand.leftHand.myHandedness == (int)finger.GetMyHandedness(finger))
            {
                hand.leftHand.isGrabbing = false;
                colliding = false;


            }
            else if ((int)hand.rightHand.myHandedness == (int)finger.GetMyHandedness(finger))
            {
                hand.rightHand.isGrabbing = false;
                colliding = false;

            }
        }
    }
}
