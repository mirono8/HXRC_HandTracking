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
    public List<SphereCollider> sphereColliders;

    [HideInInspector]
    public enum Hand
    {
        left, right
    }

    public Hand handedness;

    [Serializable]
    public class FingertipsColliders
    {
        [HideInInspector]
        public string fingerName;

        public Vector3 fingerTipPosition;

        public bool colliding;

        //public List<FingerCollisionDetector> collisionDetector;

        public FingertipsColliders(string n)
        {
            this.fingerName = n;
        }
    }

    public class CollisionEvent
    {
        public string startTime;
        public string collidingFinger;
        public string otherCollider;
    }

    HandDataOut handDataOut;

   // public List<FingertipsColliders> tipColliders;
   // public List<FingertipsColliders> tipCollidersRight;

    private void Start()
    {
        //PopulateColliderList(tipColliders);
       // PopulateColliderList(tipCollidersRight);

        handDataOut = GameObject.FindGameObjectWithTag("HandData").GetComponent<HandDataOut>();

      //  tipColliders = handDataOut.GetFingertips(HandDataOut.Hand.MyHandedness.left);

       // tipCollidersRight = handDataOut.GetFingertips(HandDataOut.Hand.MyHandedness.right);
    }

    public void PopulateColliderList(List<FingertipsColliders> list)
    {
        list.Add(new FingertipsColliders(new string("Index Finger")));
        list.Add(new FingertipsColliders(new string("Middle Finger")));
        list.Add(new FingertipsColliders(new string("Ring Finger")));
        list.Add(new FingertipsColliders(new string("Little Finger")));
    }

    public void ShowColliderPosition(HandDataOut.Hand hand)
    {
        for (int i = 0; i < hand.fingertips.Count; i++)
        {
            sphereColliders[i].transform.position = hand.fingertips[i].fingerTipPosition;

        }
    }

    public Hand GetMyHandedness(TrackColliders finger)
    {
        return finger.handedness;
    }
}
