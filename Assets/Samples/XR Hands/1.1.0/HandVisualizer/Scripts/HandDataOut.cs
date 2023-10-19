using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands.Samples.VisualizerSample;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Hands;
using System;

namespace HandData
{
    public class HandDataOut : MonoBehaviour
    {
        public bool track = true;
        public bool leftHandTracking;
        public bool rightHandTracking;

        public Transform originTransform;

        public XRHand handLeftXR;
        public XRHand handRightXR;

        public SaveManager saveManager;

        Vector3 transformedFingerPos = new();
        [Serializable]
        public class Hand
        {
            [HideInInspector]
            public XRHand handReference;
            public enum MyHandedness
            {
                left, right
            }

            public MyHandedness myHandedness;

            [SerializeField]
            public Vector3 handPosition;
            [SerializeField]
            public Vector3 forward;

            [Space(5)]
            public bool isGrabbing;
            [Space(5)]
            [SerializeField]
            public Fingers fingers;

            public List<TrackColliders.FingerColliders> fingerColliders;



            [Serializable]
            public class Fingers {

                [SerializeField]
                public Finger index;
                [SerializeField]
                public Finger middle;
                [SerializeField]
                public Finger ring;
                [SerializeField]
                public Finger little;

                [Space(5)]
                public TrackColliders trackColliders;

                [Serializable]
                public class Finger
                {
                    public bool isClosed;
                    public Vector3 fingerTipPosition;
                    public Vector3 fingerDistalPosition;
                    public Vector3 fingerProximalPosition;
                }
            }
        }


        public Hand leftHand;
        public Hand rightHand;

        private void Awake()
        {

            leftHand.myHandedness = Hand.MyHandedness.left;
            rightHand.myHandedness = Hand.MyHandedness.right;


            Debug.Log("pop");
            leftHand.fingers.trackColliders.PopulateColliderList(leftHand.fingerColliders);

            rightHand.fingers.trackColliders.PopulateColliderList(rightHand.fingerColliders);

            //InitHands();
        }

        

        public void TrackLeftHandGeneral(XRHand hand)
        {
            if (hand.isTracked && track)
            {
                leftHand.handReference = hand;
                leftHandTracking = true;
                handLeftXR = hand;
                leftHand.handPosition = handLeftXR.rootPose.position;
                leftHand.forward = handLeftXR.rootPose.forward;

                leftHand.fingers.index.isClosed = IndexClosed(hand);
                leftHand.fingers.middle.isClosed = MiddleClosed(hand);
                leftHand.fingers.ring.isClosed = RingClosed(hand);
                leftHand.fingers.little.isClosed = LittleClosed(hand);

                if (saveManager.isActiveAndEnabled)
                    saveManager.SaveHandLocation(leftHand);

                

                if (track)
                    UpdateFingerPositions(hand);
            }
            else
                leftHandTracking = false;
        }

        public void TrackRightHandGeneral(XRHand hand)
        {
            if (hand.isTracked && track)
            {
                rightHand.handReference = hand;
                rightHandTracking = true;
                handRightXR = hand;
                rightHand.handPosition = handRightXR.rootPose.position;
                rightHand.forward = handRightXR.rootPose.forward;

                rightHand.fingers.index.isClosed = IndexClosed(hand);
                rightHand.fingers.middle.isClosed = MiddleClosed(hand);
                rightHand.fingers.ring.isClosed = RingClosed(hand);
                rightHand.fingers.little.isClosed = LittleClosed(hand);

                if (saveManager.isActiveAndEnabled)
                    saveManager.SaveHandLocation(rightHand);

                if (track)
                {
                    UpdateFingerPositions(hand);

                }
            }
            else
                rightHandTracking = false;
        }


        public void UpdateFingerPositions(XRHand hand)
        {
            Pose originPose = new Pose(originTransform.position, originTransform.rotation);

            hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var indexTipPose);
            hand.GetJoint(XRHandJointID.MiddleTip).TryGetPose(out var middleTipPose);
            hand.GetJoint(XRHandJointID.RingTip).TryGetPose(out var ringTipPose);
            hand.GetJoint(XRHandJointID.LittleTip).TryGetPose(out var littleTipPose);

            hand.GetJoint(XRHandJointID.IndexDistal).TryGetPose(out var indexMiddlePose);   //distals have no colliders, only used for finger state tracking
            hand.GetJoint(XRHandJointID.MiddleDistal).TryGetPose(out var middleMiddlePose);
            hand.GetJoint(XRHandJointID.RingDistal).TryGetPose(out var ringMiddlePose);
            hand.GetJoint(XRHandJointID.LittleDistal).TryGetPose(out var littleMiddlePose);

            hand.GetJoint(XRHandJointID.IndexIntermediate).TryGetPose(out var indexIntermediatePose);
            hand.GetJoint(XRHandJointID.MiddleIntermediate).TryGetPose(out var middleIntermediatePose);
            hand.GetJoint(XRHandJointID.RingIntermediate).TryGetPose(out var ringIntermediatePose);
            hand.GetJoint(XRHandJointID.LittleIntermediate).TryGetPose(out var littleIntermediatePose);

            hand.GetJoint(XRHandJointID.IndexProximal).TryGetPose(out var indexProximalPose);
            hand.GetJoint(XRHandJointID.MiddleProximal).TryGetPose(out var middleProximalPose);
            hand.GetJoint(XRHandJointID.RingProximal).TryGetPose(out var ringProximalPose);
            hand.GetJoint(XRHandJointID.LittleProximal).TryGetPose(out var littleProximalPose);

            

            if (hand.handedness == Handedness.Left)
            {

                leftHand.fingerColliders[0].fingerTipPosition = indexTipPose.GetTransformedBy(originPose).position;
                leftHand.fingerColliders[1].fingerTipPosition = middleTipPose.GetTransformedBy(originPose).position;
                leftHand.fingerColliders[2].fingerTipPosition = ringTipPose.GetTransformedBy(originPose).position;
                leftHand.fingerColliders[3].fingerTipPosition = littleTipPose.GetTransformedBy(originPose).position;

                leftHand.fingerColliders[0].fingerIntermediatePosition = indexIntermediatePose.GetTransformedBy(originPose).position;
                leftHand.fingerColliders[1].fingerIntermediatePosition = middleIntermediatePose.GetTransformedBy(originPose).position;
                leftHand.fingerColliders[2].fingerIntermediatePosition = ringIntermediatePose.GetTransformedBy(originPose).position;
                leftHand.fingerColliders[3].fingerIntermediatePosition = littleIntermediatePose.GetTransformedBy(originPose).position;

                leftHand.fingerColliders[0].fingerProximalPosition = indexProximalPose.GetTransformedBy(originPose).position;
                leftHand.fingerColliders[1].fingerProximalPosition = middleProximalPose.GetTransformedBy(originPose).position;
                leftHand.fingerColliders[2].fingerProximalPosition = ringProximalPose.GetTransformedBy(originPose).position;
                leftHand.fingerColliders[3].fingerProximalPosition = littleProximalPose.GetTransformedBy(originPose).position;

                leftHand.fingers.index.fingerTipPosition = indexMiddlePose.GetTransformedBy(originPose).position;
                leftHand.fingers.middle.fingerTipPosition = middleMiddlePose.GetTransformedBy(originPose).position;
                leftHand.fingers.ring.fingerTipPosition = ringMiddlePose.GetTransformedBy(originPose).position;
                leftHand.fingers.little.fingerTipPosition = littleMiddlePose.GetTransformedBy(originPose).position;

                leftHand.fingers.trackColliders.ShowColliderPosition(leftHand);
            }

            if (hand.handedness == Handedness.Right)
            {

                rightHand.fingerColliders[0].fingerTipPosition = indexTipPose.GetTransformedBy(originPose).position;
                rightHand.fingerColliders[1].fingerTipPosition = middleTipPose.GetTransformedBy(originPose).position;
                rightHand.fingerColliders[2].fingerTipPosition = ringTipPose.GetTransformedBy(originPose).position;
                rightHand.fingerColliders[3].fingerTipPosition = littleTipPose.GetTransformedBy(originPose).position;

                rightHand.fingerColliders[0].fingerIntermediatePosition = indexIntermediatePose.GetTransformedBy(originPose).position;
                rightHand.fingerColliders[1].fingerIntermediatePosition = middleIntermediatePose.GetTransformedBy(originPose).position;
                rightHand.fingerColliders[2].fingerIntermediatePosition = ringIntermediatePose.GetTransformedBy(originPose).position;
                rightHand.fingerColliders[3].fingerIntermediatePosition = littleIntermediatePose.GetTransformedBy(originPose).position;

                rightHand.fingerColliders[0].fingerProximalPosition = indexProximalPose.GetTransformedBy(originPose).position;
                rightHand.fingerColliders[1].fingerProximalPosition = middleProximalPose.GetTransformedBy(originPose).position;
                rightHand.fingerColliders[2].fingerProximalPosition = ringProximalPose.GetTransformedBy(originPose).position;
                rightHand.fingerColliders[3].fingerProximalPosition = littleProximalPose.GetTransformedBy(originPose).position;

                rightHand.fingers.index.fingerTipPosition = indexMiddlePose.GetTransformedBy(originPose).position;  //fingertip tarkoitetaan yleens‰ sormen positiota, ei p‰‰t‰
                rightHand.fingers.middle.fingerTipPosition = middleMiddlePose.GetTransformedBy(originPose).position;
                rightHand.fingers.ring.fingerTipPosition = ringMiddlePose.GetTransformedBy(originPose).position;
                rightHand.fingers.little.fingerTipPosition = littleMiddlePose.GetTransformedBy(originPose).position;

                rightHand.fingers.trackColliders.ShowColliderPosition(rightHand);
            }
        }


        public List<TrackColliders.FingerColliders> GetFingertips(Hand.MyHandedness handedness)
        {
            if (handedness == leftHand.myHandedness)
            {
                return leftHand.fingerColliders;
            }
            else if (handedness == rightHand.myHandedness)
            {
                return rightHand.fingerColliders;
            }

            return null;
        }

        public Vector3 GetTransformedColliderPosition(Collider c)
        {
            var x = leftHand.fingerColliders.Find(x => x.Equals(c));

            if (x == null)
            {
                x = rightHand.fingerColliders.Find(x => x.Equals(c));
                if (x != null)
                {
                    return transformedFingerPos = rightHand.fingerColliders.Find(x => x.Equals(c)).fingerIntermediatePosition;
                }

            }
            else
            {
                return transformedFingerPos = leftHand.fingerColliders.Find(x => x.Equals(c)).fingerIntermediatePosition;
            }

            return transformedFingerPos;
        }

        public void SendCollisionData(Hand hand, TrackColliders.CollisionEvent collision)
        {
          //  Debug.Log("Collision event happened");
            if (hand.myHandedness == Hand.MyHandedness.left)
            {
                 foreach (var fingertips in leftHand.fingerColliders)
                 {
                     if (fingertips.colliding)
                     {
                         saveManager.SaveFingerCollision(leftHand, collision);
                     }
                 }

               // saveManager.SaveFingerCollision(leftHand,  collision);
            }

            if (hand.myHandedness == Hand.MyHandedness.right)
            {
                foreach (var fingertips in rightHand.fingerColliders)
                {
                    if (fingertips.colliding)
                    {
                        saveManager.SaveFingerCollision(rightHand, collision);
                    }
                }

               // saveManager.SaveFingerCollision(rightHand, collision);
            }
        }

        public string GetDate()
        {
            string date = DateTime.Now.ToString("dd-MM-yyyy");
            string time = DateTime.Now.ToString("HH-mm-ss");

            string dateTime = date + "-" + time;

            return dateTime;

            
        }

        public Vector3 GetCurrentPosition(Hand hand)
        {
            return hand.handPosition;
        }
        //------------------------------------------- Track finger positions ------------------------------//
        public bool IndexClosed(XRHand hand)
        {
            if (!(hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose) &&
                  hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var tipPose) &&
                  hand.GetJoint(XRHandJointID.IndexIntermediate).TryGetPose(out var intermediatePose)))
            {
                return false;
            }

            var wristToTip = tipPose.position - wristPose.position;
            var wristToIntermediate = intermediatePose.position - wristPose.position;
            return wristToIntermediate.sqrMagnitude >= wristToTip.sqrMagnitude;
        }

        public bool MiddleClosed(XRHand hand)
        {
            if (!(hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose) &&
                  hand.GetJoint(XRHandJointID.MiddleTip).TryGetPose(out var tipPose) &&
                  hand.GetJoint(XRHandJointID.MiddleProximal).TryGetPose(out var proximalPose)))
            {
                return false;
            }

            var wristToTip = tipPose.position - wristPose.position;
            var wristToProximal = proximalPose.position - wristPose.position;
            return wristToProximal.sqrMagnitude >= wristToTip.sqrMagnitude;
        }

        public bool RingClosed(XRHand hand)
        {
            if (!(hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose) &&
                  hand.GetJoint(XRHandJointID.RingTip).TryGetPose(out var tipPose) &&
                  hand.GetJoint(XRHandJointID.RingProximal).TryGetPose(out var proximalPose)))
            {
                return false;
            }

            var wristToTip = tipPose.position - wristPose.position;
            var wristToProximal = proximalPose.position - wristPose.position;
            return wristToProximal.sqrMagnitude >= wristToTip.sqrMagnitude;
        }

        public bool LittleClosed(XRHand hand)
        {
            if (!(hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose) &&
                  hand.GetJoint(XRHandJointID.LittleTip).TryGetPose(out var tipPose) &&
                  hand.GetJoint(XRHandJointID.LittleProximal).TryGetPose(out var proximalPose)))
            {
                return false;
            }

            var wristToTip = tipPose.position - wristPose.position;
            var wristToProximal = proximalPose.position - wristPose.position;
            return wristToProximal.sqrMagnitude >= wristToTip.sqrMagnitude;
        }
    }
}
