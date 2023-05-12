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

    Rigidbody myRigidbody;

    float upPosition;
    float downPosition;

    float restrictedValue;
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

    public void ButtonPressed(GameObject whoDunnit)
    {
        if (randomButtons.oneByOne)
        {
            if (whoDunnit.CompareTag("FingerCollider"))
            {
                Debug.Log("we in there");
                if (whoDunnit.name == "indexL" || whoDunnit.name == "indexR")
                {
                    if (myRigidbody.gameObject.transform.localPosition.y <= -0.00223f)
                    {
                        Debug.Log("interact success");
                        // myInteractableCollider.gameObject.GetComponent<Animator>().SetFloat("force", force);
                        interactSuccess = true;
                    }
                }
            }
        }
    }
    private void Update()
    {
        restrictedValue = Mathf.Clamp(myRigidbody.gameObject.transform.localPosition.y, upPosition, downPosition);

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
