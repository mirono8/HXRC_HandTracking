using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensor : MonoBehaviour
{
    [SerializeField]
    bool sensorTripped;
    AudioFeedback audioFeedback;

    private void Start()
    {
     audioFeedback = GetComponentInParent<AudioFeedback>();   
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.activeSelf)
            {
                if (other.CompareTag("FingerCollider") && !sensorTripped)
                {
                    Debug.Log("sensor tripped");
                    sensorTripped = true;

                    audioFeedback.PlaySoundClip(0);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.activeSelf)
            {
                if (other.CompareTag("FingerCollider") && sensorTripped)
                {
                    Debug.Log("sensor no longer tripped");
                    sensorTripped = false;

                    audioFeedback.PlaySoundClip(1);
                }
            }
        }
    }
}
