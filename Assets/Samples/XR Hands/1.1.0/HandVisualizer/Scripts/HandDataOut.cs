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

        public Transform originTransform;

        public XRHand handLeftXR;
        public XRHand handRightXR;

        public SaveManager saveManager;

        [Serializable]
        public class Hand
        {
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

            public List<TrackColliders.FingertipsColliders> fingertips;

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
                    public Vector3 fingerPosition;
                }
            }
        }

        public Hand leftHand;
        public Hand rightHand;

        private void Awake()
        {
            saveManager = GetComponentInChildren<SaveManager>();
            leftHand.myHandedness = Hand.MyHandedness.left;
            rightHand.myHandedness = Hand.MyHandedness.right;


            Debug.Log("pop");
            leftHand.fingers.trackColliders.PopulateColliderList(leftHand.fingertips);

            rightHand.fingers.trackColliders.PopulateColliderList(rightHand.fingertips);
            //InitHands();
        }

        public void TrackLeftHandGeneral(XRHand hand)
        {
            if (hand.isTracked && track)
            {
                handLeftXR = hand;
                leftHand.handPosition = handLeftXR.rootPose.position;
                leftHand.forward = handLeftXR.rootPose.forward;

                leftHand.fingers.index.isClosed = IndexClosed(hand);
                leftHand.fingers.middle.isClosed = MiddleClosed(hand);
                leftHand.fingers.ring.isClosed = RingClosed(hand);
                leftHand.fingers.little.isClosed = LittleClosed(hand);

                saveManager.SaveHandLocation(leftHand);

                

                if (track)
                    UpdateFingerPositions(hand);
            }
        }

        public void TrackRightHandGeneral(XRHand hand)
        {
            if (hand.isTracked && track)
            {
                handRightXR = hand;
                rightHand.handPosition = handRightXR.rootPose.position;
                rightHand.forward = handRightXR.rootPose.forward;

                rightHand.fingers.index.isClosed = IndexClosed(hand);
                rightHand.fingers.middle.isClosed = MiddleClosed(hand);
                rightHand.fingers.ring.isClosed = RingClosed(hand);
                rightHand.fingers.little.isClosed = LittleClosed(hand);

                saveManager.SaveHandLocation(rightHand);

                if (track)
                {
                    UpdateFingerPositions(hand);

                }
            }
        }


        public void UpdateFingerPositions(XRHand hand)
        {
            Pose originPose = new Pose(originTransform.position, originTransform.rotation);

            hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var indexTipPose);
            hand.GetJoint(XRHandJointID.MiddleTip).TryGetPose(out var middleTipPose);
            hand.GetJoint(XRHandJointID.RingTip).TryGetPose(out var ringTipPose);
            hand.GetJoint(XRHandJointID.LittleTip).TryGetPose(out var littleTipPose);

            hand.GetJoint(XRHandJointID.IndexDistal).TryGetPose(out var indexMiddlePose);
            hand.GetJoint(XRHandJointID.MiddleDistal).TryGetPose(out var middleMiddlePose);
            hand.GetJoint(XRHandJointID.RingDistal).TryGetPose(out var ringMiddlePose);
            hand.GetJoint(XRHandJointID.LittleDistal).TryGetPose(out var littleMiddlePose);

            if (hand.handedness == Handedness.Left)
            {

                leftHand.fingertips[0].fingerTipPosition = indexTipPose.GetTransformedBy(originPose).position;
                leftHand.fingertips[1].fingerTipPosition = middleTipPose.GetTransformedBy(originPose).position;
                leftHand.fingertips[2].fingerTipPosition = ringTipPose.GetTransformedBy(originPose).position;
                leftHand.fingertips[3].fingerTipPosition = littleTipPose.GetTransformedBy(originPose).position;

                leftHand.fingers.index.fingerPosition = indexMiddlePose.GetTransformedBy(originPose).position;
                leftHand.fingers.middle.fingerPosition = middleMiddlePose.GetTransformedBy(originPose).position;
                leftHand.fingers.ring.fingerPosition = ringMiddlePose.GetTransformedBy(originPose).position;
                leftHand.fingers.little.fingerPosition = littleMiddlePose.GetTransformedBy(originPose).position;

                leftHand.fingers.trackColliders.ShowColliderPosition(leftHand);
            }

            if (hand.handedness == Handedness.Right)
            {

                rightHand.fingertips[0].fingerTipPosition = indexTipPose.GetTransformedBy(originPose).position;
                rightHand.fingertips[1].fingerTipPosition = middleTipPose.GetTransformedBy(originPose).position;
                rightHand.fingertips[2].fingerTipPosition = ringTipPose.GetTransformedBy(originPose).position;
                rightHand.fingertips[3].fingerTipPosition = littleTipPose.GetTransformedBy(originPose).position;

                rightHand.fingers.index.fingerPosition = indexMiddlePose.GetTransformedBy(originPose).position;
                rightHand.fingers.middle.fingerPosition = middleMiddlePose.GetTransformedBy(originPose).position;
                rightHand.fingers.ring.fingerPosition = ringMiddlePose.GetTransformedBy(originPose).position;
                rightHand.fingers.little.fingerPosition = littleMiddlePose.GetTransformedBy(originPose).position;

                rightHand.fingers.trackColliders.ShowColliderPosition(rightHand);
            }
        }


        public List<TrackColliders.FingertipsColliders> GetFingertips(Hand.MyHandedness handedness)
        {
            if (handedness == leftHand.myHandedness)
            {
                return leftHand.fingertips;
            }
            else if (handedness == rightHand.myHandedness)
            {
                return rightHand.fingertips;
            }

            return null;
        }

        //tähän timestamp ku alkaa ja tallentuu vasta ku loppuu, nyt tallentuu joka frame ja tankkaa

        public void SendCollisionData(Hand hand, TrackColliders.CollisionEvent collision)
        {
            if (hand.myHandedness == Hand.MyHandedness.left)
            {
                /* foreach (var fingertips in leftHand.fingertips)
                 {
                     if (fingertips.colliding)
                     {
                         saveManager.SaveFingerCollision(leftHand, fingertips.finger);
                     }
                 }*/

                saveManager.SaveFingerCollision(leftHand,  collision);
            }

            if (hand.myHandedness == Hand.MyHandedness.right)
            {
                /*foreach (var fingertips in rightHand.fingertips)
                {
                    if (fingertips.colliding)
                    {
                        saveManager.SaveFingerCollision(rightHand, rightHand.fingertips[fingerTipIndex].finger);
                    }
                }*/

                saveManager.SaveFingerCollision(rightHand, collision);
            }
        }

        public string GetDate()
        {
            string date = DateTime.Now.ToString("dd-MM-yyyy");
            string time = DateTime.Now.ToString("HH-mm-ss");

            string dateTime = date + "-" + time;

            return dateTime;
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
