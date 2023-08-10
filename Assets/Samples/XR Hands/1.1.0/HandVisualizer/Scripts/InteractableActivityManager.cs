using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandData;
using UnityEngine.UIElements;

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

    public enum Interactor
    {
        Left, Right
    }

    public InteractableType type;
    public InteractableSize size;
    public Interactor interactor;

    HandDataOut handData;
    SaveManager saveManager;

    [SerializeField]
    SaveManager.InteractableEvent interactableEvent;

    List<string> leftHandPos;
    List<string> rightHandPos;

    CollidableObjects collidables;

    [SerializeField]
    RandomButtons randomButtons;

    public int myOrderIndex;

    public bool interactSuccess;

    Rigidbody myRigidbody;

    float upPosition;
    float downPosition;

    float myDuration;

    public Vector3 myRot;

    Material originalMaterial;

    public Material highlightMaterial;

    public bool highlighted;

    public Renderer rendererToChange;

    public Collider intersectionCollider;

    private bool moveOn;

    private void Awake()
    {
        randomButtons = transform.parent.GetComponentInParent<RandomButtons>();
    }
    
    void Start()
    {
        originalMaterial = rendererToChange.material;

        leftHandPos = new();
        rightHandPos = new();

        if (type == InteractableType.Button)
        {
            upPosition = 0;
            downPosition = -0.003f;
        }
        if(type == InteractableType.Switch)
        {
            upPosition = -90f;
            downPosition = 300f;
        }

        myRigidbody = GetComponentInChildren<Rigidbody>();
        collidables = GetComponentInParent<CollidableObjects>();

        myOrderIndex = collidables.objects.FindIndex(x => x == gameObject);

        if (myOrderIndex == 0 && randomButtons.oneByOne) 
        {
            gameObject.SetActive(true);
        }
        else if (myOrderIndex == 0 && !randomButtons.oneByOne)
        {
            rendererToChange.material = highlightMaterial;
            StartInteractionEvent();
            gameObject.SetActive(true);
        }

        handData = GameObject.FindGameObjectWithTag("HandData").GetComponent<HandDataOut>();
        saveManager = FindObjectOfType<SaveManager>();

        SetMySize();

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
            case InteractableSize.Small: gameObject.transform.localScale = new Vector3(2f, 2f, 2f); break;
            case InteractableSize.Medium: gameObject.transform.localScale = new Vector3(3f, 3f, 3f); break;
            case InteractableSize.Large: gameObject.transform.localScale = new Vector3(4f, 4f, 4f); break;
            case InteractableSize.Random: int r = Random.Range(0, 3); size = (InteractableSize)r; SetMySize(); break;
            default: Debug.Log("cant make it"); break;
        }
    }

    #region InteractionCheckByType
    public void ButtonPressed()
    {

        if (myRigidbody.gameObject.transform.localPosition.y <= downPosition)
        {
            // myInteractableCollider.gameObject.GetComponent<Animator>().SetFloat("force", force);
            if (!highlighted)
            {
                ResetPosition();
            }
            else
                interactSuccess = true;
        }
    }

    public void SwitchPressed()
    {


        if (myRigidbody.gameObject.transform.localEulerAngles.x >= downPosition)
        {

            interactSuccess = true;
        }

    }

    public void LeverPulled()
    {
        interactSuccess = true;
    }
    #endregion

    #region TransformConstraintsByType
    public void ButtonContraints()
    {
        if (myRigidbody.gameObject.transform.localPosition.y > upPosition)
        {
            myRigidbody.gameObject.transform.localPosition = new Vector3(myRigidbody.gameObject.transform.localPosition.x, upPosition, myRigidbody.gameObject.transform.localPosition.z);
        }

        if (myRigidbody.gameObject.transform.localPosition.y < downPosition)
        {
            myRigidbody.gameObject.transform.localPosition = new Vector3(myRigidbody.gameObject.transform.localPosition.x, downPosition, myRigidbody.gameObject.transform.localPosition.z);
        }

        if (myRigidbody.gameObject.transform.localPosition.x > 0 || myRigidbody.gameObject.transform.localPosition.x < 0)
        {
            myRigidbody.gameObject.transform.localPosition = new Vector3(0, myRigidbody.gameObject.transform.localPosition.y, myRigidbody.gameObject.transform.localPosition.z);
        }

        if (myRigidbody.gameObject.transform.localPosition.z < 0 || myRigidbody.gameObject.transform.localPosition.z > 0)
        {
            myRigidbody.gameObject.transform.localPosition = new Vector3(myRigidbody.gameObject.transform.localPosition.x, myRigidbody.gameObject.transform.localPosition.y, 0);
        }


       /* if ((myRigidbody.gameObject.transform.localPosition.y > upPosition) && !interactSuccess)
        {
            myRigidbody.gameObject.transform.localPosition = new Vector3(myRigidbody.gameObject.transform.localPosition.x, upPosition, myRigidbody.gameObject.transform.localPosition.z);
        }*/
    }

    public void SwitchContraints()
    {
        myRigidbody.gameObject.transform.localPosition = new Vector3(0, 0.007f, 0);

        if(!interactSuccess)
            myRigidbody.gameObject.transform.localEulerAngles = new Vector3(myRigidbody.gameObject.transform.localEulerAngles.x, 180f, 180f);
        else
            myRigidbody.gameObject.transform.localEulerAngles = new Vector3(downPosition, 180f, 180f);

    }

    public void ResetPosition()
    {
        if (!MoveOn())
        {

            if (type == InteractableType.Button)
                myRigidbody.gameObject.transform.localPosition = new Vector3(myRigidbody.gameObject.transform.localPosition.x, upPosition, myRigidbody.gameObject.transform.localPosition.z);

            if (type == InteractableType.Switch)
                myRigidbody.gameObject.transform.localEulerAngles = new Vector3(upPosition, 180f, 180f);

            interactSuccess = false;
        }
    }

    #endregion

    #region JSONDataCollection
    public void StartInteractionEvent()  // Aloittaa tämän interactablen datan seurannan JSONia varten
    {
        Debug.Log("start interaction event");

        myDuration = 0;

        interactableEvent = new SaveManager.InteractableEvent();
        interactableEvent.trajectory = new();

        interactableEvent.interactableSize = size.ToString().Substring(0,1).ToLower();
        interactableEvent.interactableType = type.ToString().Substring(0,1).ToLower();


    }
    public void EndInteractionEvent()
    {
        interactableEvent.duration = myDuration.ToString();

        Debug.Log("end interaction event");
        if (interactor == Interactor.Left)
        {
            if (leftHandPos.Count > 0 && handData.leftHandTracking)
            {
                interactableEvent.startPoint = leftHandPos[0];

                for (int i = 0; i < leftHandPos.Count; i++)
                {
                    interactableEvent.trajectory.Add(leftHandPos[i]);
                }

                interactableEvent.distance = Vector3.Distance(StringToVector3(leftHandPos[0]), gameObject.transform.position).ToString();
                interactableEvent.endPoint = leftHandPos[^1];

            }
        }
        else if (interactor == Interactor.Right && handData.rightHandTracking)
        {
            if (rightHandPos.Count > 0)
            {
                interactableEvent.startPoint = rightHandPos[0];
                for (int i = 0; i < rightHandPos.Count; i++)
                {
                    interactableEvent.trajectory.Add(rightHandPos[i]);
                }

                interactableEvent.distance = Vector3.Distance(StringToVector3(rightHandPos[0]), gameObject.transform.position).ToString();
                interactableEvent.endPoint = rightHandPos[^1];
            }
        }
        Debug.Log(interactableEvent.interactableSize + interactableEvent.interactableType + interactableEvent.duration);

        saveManager.combinedData.interactableEvents.events.Add(interactableEvent); //välil (aina) nullreference, vaikuttaa janssoniin vaa, fix
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
#endregion


    private void OnEnable()
    {
        if (randomButtons.IsOneByOne())
        {
            Debug.Log("onebyone");
            StartInteractionEvent();
        }
        
    }


    private void OnDisable()
    {
        if (randomButtons.IsOneByOne())
        {
            EndInteractionEvent();
        }
        
    }
    
    public void OneByOneSuccesscheck()
    {
        if (interactSuccess)
        {
            if (myOrderIndex != collidables.objects.Count - 1)
            {
                collidables.objects[myOrderIndex + 1].SetActive(true);
            }
            else
            {

              //  GameObject.FindGameObjectWithTag("SessionManager").GetComponent<SessionManager>().TryStartNextSet();
            }

            if (gameObject.activeSelf == true)
                gameObject.SetActive(false);
        }
    }

    public void AllAtOnceSuccessCheck()
    {
        if (highlighted)
        {
            if (myOrderIndex != collidables.objects.Count - 1)
            {
                rendererToChange.material = originalMaterial;
                if (collidables.GetNearestNeighbor(myOrderIndex) != collidables.objects[myOrderIndex + 1])
                {
                    collidables.objects[myOrderIndex + 1].GetComponent<InteractableActivityManager>().rendererToChange.material = highlightMaterial;
                    collidables.objects[myOrderIndex + 1].GetComponent<InteractableActivityManager>().StartInteractionEvent();
                }
                else
                {
                    Debug.Log("next was too close");
                    if (collidables.objects[myOrderIndex + 2] != null)
                    {
                        collidables.objects[myOrderIndex + 2].GetComponent<InteractableActivityManager>().rendererToChange.material = highlightMaterial;
                        collidables.objects[myOrderIndex + 2].GetComponent<InteractableActivityManager>().StartInteractionEvent();
                    }
                    else
                        Debug.Log("shouldnt't be here");
                }

                SetMoveOn(true);
                EndInteractionEvent();
            }
            else
            {

                //GameObject.FindGameObjectWithTag("SessionManager").GetComponent<SessionManager>().TryStartNextSet();
            }
        }
        else
        {
            if (!MoveOn())
            {
                interactSuccess = false;
                Debug.Log("order LUL " + " from orderindex of " + myOrderIndex);
                //ResetPosition();
            }
        }
    }

    
    void SetMoveOn(bool b)
    {
       moveOn = b;
    }

    bool MoveOn()
    {
        return moveOn;
    }

    private void Update()
    {
        myDuration += Time.deltaTime;

        if (rendererToChange.sharedMaterial == highlightMaterial)
        {
            highlighted = true;
        }
        else { highlighted = false; }

        if (type == InteractableType.Button)
        {
            ButtonPressed();
            ButtonContraints();
        }
        if (type == InteractableType.Switch)
        {
            SwitchPressed();
            SwitchContraints();
        }

        if (handData.leftHandTracking)
            leftHandPos.Add(handData.leftHand.handPosition.ToString());

        if(handData.rightHandTracking)
            rightHandPos.Add(handData.rightHand.handPosition.ToString());

        if (randomButtons.oneByOne)
        {
            if(interactSuccess)
                OneByOneSuccesscheck();
        } 
        else
        {
            if(interactSuccess)
                AllAtOnceSuccessCheck();
        }

        myRot = myRigidbody.transform.localEulerAngles; // käytä tätä switch interactionsuccess checkis

        if (Input.GetButtonDown("InteractSuccess"))
            interactSuccess = true;
    }
}
