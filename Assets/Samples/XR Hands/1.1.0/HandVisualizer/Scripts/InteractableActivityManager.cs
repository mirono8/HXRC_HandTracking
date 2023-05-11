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

    public Collider myInteractableCollider;

    float force;

    float upPosition;
    float downPosition;

    void Start()
    {
        if (type == InteractableType.Button)
        {
            upPosition = 0;
            downPosition = -0.003f;
        }

        collidables = GetComponentInParent<CollidableObjects>();
        randomButtons = GetComponentInParent<RandomButtons>();

        myOrderIndex = collidables.objects.FindIndex(x => x == gameObject);

        if (myOrderIndex == 0) 
        {
            gameObject.SetActive(true);
        }
    }

   /* private void OnTriggerEnter(Collider other)
    {
        if (randomButtons.oneByOne)
        {
            if (other.gameObject == GameObject.FindGameObjectWithTag("FingerCollider"))
            {
                Debug.Log("we in there");
                if (other == other.GetComponent<HandDataOut>().leftHand.fingers.trackColliders.sphereColliders[0] ||
                    other.GetComponent<HandDataOut>().rightHand.fingers.trackColliders.sphereColliders[0])
                {
                    Debug.Log("interact success");
                    interactSuccess = true;
                }
            }
        }
    }*/

    public void ButtonPressed(GameObject whoDunnit)
    {
        if (randomButtons.oneByOne)
        {
            if (whoDunnit.CompareTag("FingerCollider"))
            {
                Debug.Log("we in there");
                if (whoDunnit.name == "indexL" || whoDunnit.name == "indexR")
                {
                    //NEED LOGIC BUTTON PRESSING I.E. PASSING THE FORCE TO BLENDTREE
                    Debug.Log("interact success");
                    myInteractableCollider.gameObject.GetComponent<Animator>().SetFloat("force", force);
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
                collidables.objects[myOrderIndex + 1].SetActive(true);
            }
            else
                Debug.Log("All buttons pressed");

            this.gameObject.SetActive(false);
        }
    }
}
