using HandData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class AdjustHeight : MonoBehaviour
{
    bool colliding;

    public Slider slider;

    public float sliderValue;

    [SerializeField]
    Transform target;

    [SerializeField]
    Vector3 adjustedPosition;


    private void Start()
    {
        //slider = GetComponent<Slider>();

        slider.onValueChanged.AddListener(delegate { SetFloat(slider.value); });
        
    }
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
                    Debug.Log("collided with ghostwall");

                }
                else if ((int)hand.rightHand.myHandedness == (int)finger.GetMyHandedness(finger))
                {
                    hand.rightHand.isGrabbing = true;
                    colliding = true;

                    Debug.Log("collided with ghostwall");
                }
            }
        }
    }
    private void Update()
    {
        SetFloat(slider.value);
    }

    private void FixedUpdate()
    {
        target.transform.position = adjustedPosition;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("FingerCollider"))
        {
            TrackColliders finger = other.gameObject.GetComponentInParent<TrackColliders>();

            HandDataOut hand = FindObjectOfType<HandDataOut>();

            if ((int)hand.leftHand.myHandedness == (int)finger.GetMyHandedness(finger))
            {
                hand.leftHand.isGrabbing = false;
                colliding = false;


            }
            else if ((int)hand.rightHand.myHandedness == (int)finger.GetMyHandedness(finger))
            {
                hand.rightHand.isGrabbing = false;
                colliding = false;
                adjustedPosition = target.transform.position + new Vector3(target.transform.position.x, sliderValue, target.transform.position.z);

            }
        }
    }

    public void SetFloat(float value)
    {
        sliderValue = value;
    }
}
