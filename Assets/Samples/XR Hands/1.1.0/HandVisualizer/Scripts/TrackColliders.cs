using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;
using UnityEngine;
using HandData;
using UnityEngine.XR.Hands;

public class TrackColliders : MonoBehaviour
{
    public List<SphereCollider> tipSphereColliders;

    public List<SphereCollider> intermediateSphereColliders; //THESE ONLY
                                                             //USED IN
    public List<SphereCollider> proximalSphereColliders;     //LEVER GRABBING

    public List<SphereCollider> physicsColliders = new();

    [HideInInspector]
    public enum Hand
    {
        left, right
    }

    public Hand handedness;

    [Serializable]
    public class FingerColliders
    {
        [HideInInspector]
        public string fingerName;

        public Vector3 fingerTipPosition;
        public Vector3 fingerIntermediatePosition;
        public Vector3 fingerProximalPosition;

        public bool colliding;

        //public List<FingerCollisionDetector> collisionDetector;

        public FingerColliders(string n)
        {
            this.fingerName = n;
        }
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var spheres = transform.GetChild(i).GetComponents<SphereCollider>();

            physicsColliders.Add(spheres[1]);
            
        }
    }
    public class CollisionEvent
    {
        public string startTime;
        public string collidingFinger;
        public string otherCollider;
    }

    public void PopulateColliderList(List<FingerColliders> list)
    {
        list.Add(new FingerColliders(new string("Index Finger")));
        list.Add(new FingerColliders(new string("Middle Finger")));
        list.Add(new FingerColliders(new string("Ring Finger")));
        list.Add(new FingerColliders(new string("Little Finger")));
    }

    public void ShowColliderPosition(HandDataOut.Hand hand)
    {
        for (int i = 0; i < hand.fingerColliders.Count; i++)
        {
            tipSphereColliders[i].transform.position = hand.fingerColliders[i].fingerTipPosition;
            intermediateSphereColliders[i].transform.position = hand.fingerColliders[i].fingerIntermediatePosition;
            proximalSphereColliders[i].transform.position = hand.fingerColliders[i].fingerProximalPosition;
        }
    }

    public Hand GetMyHandedness(TrackColliders finger)
    {
        return finger.handedness;
    }

    public void ToggleTrigger(bool b)
    {
        for (int i = 0; i < physicsColliders.Count; i++)
        {
            physicsColliders[i].isTrigger = b;

        }
    }
}
