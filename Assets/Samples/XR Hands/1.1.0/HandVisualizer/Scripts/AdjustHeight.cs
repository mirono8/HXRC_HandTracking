using HandData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class AdjustHeight : MonoBehaviour
{
    bool colliding;
    bool clamped;
   // public Slider slider;

    public float sliderValue;

    [SerializeField]
    Transform target;

    [SerializeField]
    Vector3 adjustedPosition;

    float constraints = 0.25f;

    [SerializeField]
    GameObject handle;

    [SerializeField]
    LobbyStart lobby;
    private void Start()
    {
        //slider = GetComponent<Slider>();

      //  slider.onValueChanged.AddListener(delegate { SetFloat(slider.value); });
        
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
                    Debug.Log("collided with handle");

                }
                else if ((int)hand.rightHand.myHandedness == (int)finger.GetMyHandedness(finger))
                {
                    hand.rightHand.isGrabbing = true;
                    colliding = true;

                    Debug.Log("collided with handle");
                }
            }
        }
    }

    private void Update()
    {
        if (handle.transform.localPosition.y >= constraints)
        {
            float clamp = Mathf.Clamp(handle.transform.localPosition.y, (constraints * -1), constraints);
            handle.transform.localPosition = new Vector3(0, clamp, -0.017f);
            clamped = true;

        }
        else
         clamped = false;

        if (handle.transform.localPosition.y <= (constraints * -1))
        {
            float clamp = Mathf.Clamp(handle.transform.localPosition.y, (constraints * -1), constraints);
            handle.transform.localPosition = new Vector3(0, clamp, -0.017f);
            clamped = true;
        }
        else
            clamped = false;

        target.transform.position = adjustedPosition;
        lobby.SetPanelHeight(adjustedPosition.y);
    }

    private void OnTriggerExit(Collider other)
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
              

            }
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("FingerCollider"))
        {
            handle.transform.localPosition = new Vector3(0, other.transform.position.y, -0.017f);

            if(!clamped)
                adjustedPosition = new Vector3(target.transform.position.x, handle.transform.localPosition.y, target.transform.position.z);
        }
    }

    public void SetFloat(float value)
    {
        sliderValue = value;
    }

    
}
