using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandData;

public class InteractableActivityManager : MonoBehaviour
{
    public enum InteractableType
    {
        Button, Lever, Switch
    }

    public InteractableType type;

    CollidableObjects collidables;

    RandomButtons randomButtons;

    public int myOrderIndex;

    public bool interactSuccess;

    void Start()
    {
        collidables = GetComponentInParent<CollidableObjects>();
        randomButtons = GetComponentInParent<RandomButtons>();

        myOrderIndex = collidables.objects.FindIndex(x => x == gameObject);

        if (myOrderIndex == 0) 
        {
            gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (randomButtons.oneByOne)
        {
            if (other == GameObject.FindGameObjectWithTag("FingerCollider"))
            {
                if (other == other.GetComponent<HandDataOut>().leftHand.fingers.trackColliders.sphereColliders[0] ||
                    other.GetComponent<HandDataOut>().rightHand.fingers.trackColliders.sphereColliders[0])
                {
                    interactSuccess = true;
                }
            }
        }
    }

    private void Update()
    {
        if (interactSuccess)
        {
            if (myOrderIndex != collidables.objects.Count - 1)
            {
                var next = collidables.objects[myOrderIndex + 1];
                next.gameObject.SetActive(true);
            }
            else
                Debug.Log("All buttons pressed");

            this.gameObject.SetActive(false);
        }
    }
}
