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

    public enum InteractableSize
    {
        Small, Medium, Large, Random
    }

    public InteractableType type;
    public InteractableSize size;

    CollidableObjects collidables;

    RandomButtons randomButtons;

    public int myOrderIndex;

    public bool interactSuccess;

    Rigidbody myRigidbody;

    float upPosition;
    float downPosition;

    

    void Start()
    {
        if (type == InteractableType.Button)
        {
            upPosition = 0;
            downPosition = -0.003f;
        }

        myRigidbody = GetComponentInChildren<Rigidbody>();
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

    public void SetMySize()
    {
        switch (size)
        {
            case InteractableSize.Small: gameObject.transform.localScale.Set(2, 2, 2); break;
            case InteractableSize.Medium: gameObject.transform.localScale.Set(3, 3, 3); break;
            case InteractableSize.Large: gameObject.transform.localScale.Set(4, 4, 4); break;
            case InteractableSize.Random: int r = Random.Range(0, 3); size = (InteractableSize)r; SetMySize(); break;
        }
    }




    public void ButtonPressed()
    {
        if (randomButtons.oneByOne)
        {
            if (myRigidbody.gameObject.transform.localPosition.y <= downPosition)
            {
                Debug.Log("interact success");
                // myInteractableCollider.gameObject.GetComponent<Animator>().SetFloat("force", force);
                interactSuccess = true;
            }
        }
    }


    private void Update()
    {
        ButtonPressed();

        if (myRigidbody.gameObject.transform.localPosition.y > upPosition)
        {
            myRigidbody.gameObject.transform.localPosition = new Vector3(myRigidbody.gameObject.transform.localPosition.x, upPosition , myRigidbody.gameObject.transform.localPosition.z);
        }

        if (myRigidbody.gameObject.transform.localPosition.y < downPosition)
        {
            myRigidbody.gameObject.transform.localPosition = new Vector3(myRigidbody.gameObject.transform.localPosition.x, downPosition, myRigidbody.gameObject.transform.localPosition.z);
        }

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
