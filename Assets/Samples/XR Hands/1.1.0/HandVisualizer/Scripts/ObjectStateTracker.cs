using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandData;
using System.Diagnostics;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.Hands;

public class ObjectStateTracker : MonoBehaviour
{
    Vector3 initialPosition;
    Quaternion initialRotation;

    Vector3 previousPosition;

   [HideInInspector]
    public float timer;

    Transform handTransform;

    public bool joystick;

    private bool grabbed;
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (transform.position != initialPosition && timer > 5  && !joystick)
            ReturnHome();
    }

    private void LateUpdate()
    {
        if (Time.frameCount % 2 == 0)
        {
            previousPosition = transform.position;
        }

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

            if (transform.position != previousPosition)
            {
                var finger = other.gameObject.GetComponentInParent<TrackColliders>();

                var hand = FindObjectOfType<HandDataOut>();

                if ((int)hand.leftHand.myHandedness == (int)finger.GetMyHandedness(finger))
                {
                    hand.leftHand.isGrabbing = true;
                    grabbed = true;
                    
                }
                else if((int)hand.rightHand.myHandedness == (int)finger.GetMyHandedness(finger))
                {
                    hand.rightHand.isGrabbing = true;
                    grabbed = true;
                    
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
                grabbed = false;
                

            }
            else if ((int)hand.rightHand.myHandedness == (int)finger.GetMyHandedness(finger))
            {
                hand.rightHand.isGrabbing = false;
                grabbed = false;
               
            }


        }
    }
}

