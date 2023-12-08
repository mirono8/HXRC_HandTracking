using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensor : MonoBehaviour
{
    [SerializeField]
    bool sensorTripped;
    AudioFeedback audioFeedback;

    public bool enableSensor;

    [SerializeField]
    List<Collider> collidingWith;

    SessionManager sessionManager;
    private void Start()
    {
        audioFeedback = GetComponentInParent<AudioFeedback>();
        sessionManager = GameObject.FindGameObjectWithTag("SessionManager").GetComponent<SessionManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enableSensor)
        {
            if (other != null)
            {
                if (other.gameObject.activeSelf)
                {
                    if (other.CompareTag("FingerCollider") )
                    {
                        if (collidingWith.Count == 0)
                            audioFeedback.PlaySoundClip(0);

                        if (!collidingWith.Contains(other))
                            collidingWith.Add(other);

                      /*  Debug.Log("sensor tripped");
                        sensorTripped = true;*/

                        
                    }
                }
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (enableSensor)
        {
            if (other != null)
            {
                if (other.gameObject.activeSelf)
                {
                    if (other.CompareTag("FingerCollider"))
                    {
                        if (collidingWith.Contains(other))
                            collidingWith.Remove(other);

                        Debug.Log("sensor no longer tripped");

                        if (collidingWith.Count == 0)
                        {
                           // sensorTripped = false;
                            audioFeedback.PlaySoundClip(1);
                        }

                    }
                }
            }
        }
    }
}
