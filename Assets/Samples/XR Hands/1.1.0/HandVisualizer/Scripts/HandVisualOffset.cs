using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandVisualOffset : MonoBehaviour
{
    public enum Hand
    {
        Left, Right
    }

    public Hand thisHand;

    TrackColliders[] sortThroughColliders;

    [SerializeField]
    TrackColliders trackColliders;

    [SerializeField]
    GameObject handVisualizerGameObject;

    [SerializeField]
    Transform wristRoot;
    [SerializeField]
    List<FingerCollisionDetector> collisionDetectors;
    [SerializeField]
    List<FingerCollisionDetector> activeCollisions;
    void Start()
    {
       /* sortThroughColliders = FindObjectsOfType<TrackColliders>();

        foreach (var obj in sortThroughColliders)
        {
            if (gameObject.name == "L_Wrist")  //this is the left hand's root
            {
                if (obj.handedness == TrackColliders.Hand.left)
                {
                    trackColliders = obj;
                }
            }
            else if (gameObject.name == "R_Wrist") //this is the right hand's root
            {
                if(obj.handedness == TrackColliders.Hand.right)
                {
                    trackColliders = obj;
                }
            }
        }

        if (trackColliders == null)
        {
            Debug.Log("Couldn't find TrackColliders component");
        }*/
        trackColliders = GetComponentInChildren<TrackColliders>(); //fingercollisiondeterctorii get collision offset ja tänne updatee!

        for (int i = 0; i < 4; i++)
        {
            collisionDetectors.Add(trackColliders.physicsColliders[i].GetComponent<FingerCollisionDetector>());
        }
    }

    private void OnEnable()
    {

        if (wristRoot == null) {

            for (int i = 0; i < handVisualizerGameObject.transform.childCount; i++)
            {
                if (thisHand == Hand.Left)
                {
                    if (handVisualizerGameObject.transform.GetChild(i).name == "L_Wrist")
                    {
                        wristRoot = handVisualizerGameObject.transform.GetChild(i);
                        break;
                    }
                    else
                    {
                        for (int j = 0; j < handVisualizerGameObject.transform.GetChild(i).childCount; j++)
                        {
                            if (handVisualizerGameObject.transform.GetChild(i).GetChild(j).name == "L_Wrist")
                            {
                                wristRoot = handVisualizerGameObject.transform.GetChild(i).GetChild(j);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (handVisualizerGameObject.transform.GetChild(i).name == "R_Wrist")
                    {
                        wristRoot = handVisualizerGameObject.transform.GetChild(i);
                        break;
                    }
                    for (int j = 0; j < handVisualizerGameObject.transform.GetChild(i).childCount; j++)
                    {
                        if (handVisualizerGameObject.transform.GetChild(i).GetChild(j).name == "R_Wrist")
                        {
                            wristRoot = handVisualizerGameObject.transform.GetChild(i).GetChild(j);
                            break;
                        }
                    }

                }
            
            }
        }
    }

    public void CheckActiveCollisions()
    {

        FingerCollisionDetector active = collisionDetectors.Find(x => x.IsActiveVisualCollision() == true);
        if (active != null)
        {
            
                if (!activeCollisions.Contains(active))
                {
                    activeCollisions.Add(active);
                }
         
        }
    }

    public void ClearInactiveCollisions()
    {
        FingerCollisionDetector inactive = activeCollisions.Find(x => x.IsActiveVisualCollision() == false && activeCollisions.Contains(x));
        if (inactive != null)
        {
            if (activeCollisions.Any())
            {

                if (activeCollisions.Contains(inactive))
                {
                    activeCollisions.Remove(inactive);
                }
            }
        }
    }
    void Update()
    {
        CheckActiveCollisions();
        if (activeCollisions.Any())
        {
            
            
        
            wristRoot.position += activeCollisions[0].GetCollisionOffset();
            ClearInactiveCollisions();
        }
      //  wristRoot.position =
    }
}
