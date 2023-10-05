using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandData;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;

public class InteractableActivityManager : MonoBehaviour
{
    public enum InteractableType // chosen type
    {
        Button, Lever, Switch
    }

    public enum InteractableSize // chosen size
    {
        Small, Medium, Large, Random
    }

    public enum Interactor // which hand interacted with this
    {
        Left, Right
    }

    public enum SessionMode // the session mode this interactanble operates in
    {
        All, OneByOne, Matrix
    }

    public InteractableType type;
    public InteractableSize size;
    public Interactor interactor;
    public SessionMode sessionMode;

    HandDataOut handData;
    SaveManager saveManager;

    [SerializeField]
    SaveManager.InteractableEvent interactableEvent;

    List<string> leftHandPos;
    List<string> rightHandPos;

    CollidableObjects collidables;

    [SerializeField]
    RandomButtons randomButtons;

    ButtonMatrix matrix;

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

    public bool tooClose;

    public bool rayCasting = true;

    public bool boundsCollide;

    public Transform RaycastStartPos;

    public Transform rayStartLeft;
    public Transform rayStartRight;
    public Transform rayStartUp;
    public Transform rayStartDown;

    public Transform rayStartUpLeft;
    public Transform rayStartUpRight;
    public Transform rayStartDownRight;
    public Transform rayStartDownLeft;

    public List<Collider> colliders;

    public SessionManager sessionManager;



    LayerMask ignoreInRaycast;
    private void Awake()
    {
        if (sessionMode != SessionMode.Matrix)
        {
            randomButtons = transform.parent.GetComponentInParent<RandomButtons>();
        }
        else
        {
            matrix = transform.parent.GetComponentInParent<ButtonMatrix>();
            rayCasting = false;
        }
    }
    
    void Start()
    {
        GetMyColliders();

        originalMaterial = rendererToChange.material;
        ignoreInRaycast = LayerMask.GetMask("BlockHands");
        leftHandPos = new();
        rightHandPos = new();

        sessionManager = GameObject.FindGameObjectWithTag("SessionManager").GetComponent<SessionManager>();

        if (type == InteractableType.Button)
        {
            upPosition = 0;
            downPosition = -0.003f;
        }

        if (type == InteractableType.Switch)
        {
            upPosition = -90f;
            downPosition = 300f;
        }

        myRigidbody = GetComponentInChildren<Rigidbody>();
        collidables = GetComponentInParent<CollidableObjects>();


        myOrderIndex = collidables.objects.FindIndex(x => x == gameObject);


        if (randomButtons != null)
        {
            if (myOrderIndex == 0 && randomButtons.oneByOne)
            {
                rendererToChange.material = highlightMaterial;
                gameObject.SetActive(true);
            }
            else if (myOrderIndex == 0 && !randomButtons.oneByOne)
            {
                rendererToChange.material = highlightMaterial;
                StartCoroutine(StartInteractionEvent());
                gameObject.SetActive(true);
            }

        }
        else
        {
            if (myOrderIndex == 0)
            {
                rendererToChange.material = highlightMaterial;
                gameObject.SetActive(true);
            }
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

    
    public void ToggleKinematic(bool toggleOn) // at set start, to prevent interactables from colliding during the initial position set
    {
        if (myRigidbody != null)
        {
            Debug.Log("toggling kinematics");
            if (toggleOn) {

                myRigidbody.Sleep();
                myRigidbody.isKinematic = true;
            }
            else if (!toggleOn)
            {
                myRigidbody.WakeUp();
                myRigidbody.isKinematic = false;
            }
                
        }
    }
    void GetMyColliders() // finds colliders of this interactable, to ignore some in other scripts
    {
        var result = transform.GetComponents<Collider>();
        Debug.Log(result.Length + "found colliders length");
        if (result != null)
        {
            foreach (var c in result)
            {
                colliders.Add(c);
            }
        }
        result = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            
            result = transform.GetChild(i).GetComponents<Collider>();
            if (result != null)
            {
                foreach (var c in result)
                {
                    colliders.Add(c);
                }
            }
            result = null;
        }
    }
    public void SetMySize() // sets interactable size based on the JSON
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
    public void ButtonPressed() // continuous check if this interactable, as a button type, has been pressed
    {

        if (myRigidbody.gameObject.transform.localPosition.y <= downPosition)
        {
            // myInteractableCollider.gameObject.GetComponent<Animator>().SetFloat("force", force);
            if (sessionMode != SessionMode.Matrix)
            {
                if (!randomButtons.oneByOne)
                {
                    if (!highlighted)
                    {
                        ResetPosition();
                    }
                    else
                        interactSuccess = true;
                }
                else
                {
                    interactSuccess = true;
                }
            }
            else
            {
                if (!highlighted)
                {
                    ResetPosition();
                }
                else
                    interactSuccess = true;
            }
        }
    }

    public void SwitchPressed() // same as above for switches
    {

        if (myRigidbody.gameObject.transform.localEulerAngles.x >= downPosition)
        {
            if (sessionMode != SessionMode.Matrix)
            {
                if (!randomButtons.oneByOne)
                {
                    if (!highlighted)
                    {
                        ResetPosition();
                    }
                    else
                        interactSuccess = true;
                }
                else
                {
                    interactSuccess = true;
                }
            }
            else
            {
                if (!highlighted)
                {
                    ResetPosition();
                }
                else
                    interactSuccess = true;
            }

            
        }

    }

    public void LeverPulled() // currently redundant, check for levers
    {
        interactSuccess = true;
    }
    #endregion

    #region TransformConstraintsByType
    public void ButtonContraints() // constraints for button type interactable's rigidbody movement
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


        if ((myRigidbody.gameObject.transform.localPosition.y > upPosition) && !interactSuccess)
        {
            myRigidbody.gameObject.transform.localPosition = new Vector3(myRigidbody.gameObject.transform.localPosition.x, upPosition, myRigidbody.gameObject.transform.localPosition.z);
        }
    }

    public void SwitchContraints() // same as above for switches
    {
        myRigidbody.gameObject.transform.localPosition = new Vector3(0, 0.007f, 0);

        if(!interactSuccess)
            myRigidbody.gameObject.transform.localEulerAngles = new Vector3(myRigidbody.gameObject.transform.localEulerAngles.x, 180f, 180f);
        else
            myRigidbody.gameObject.transform.localEulerAngles = new Vector3(downPosition, 180f, 180f);

    }

    public void ResetPosition() // resets interactable's rigidbody values in case of being interacted with out of order
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
    public IEnumerator StartInteractionEvent()  // Aloittaa tämän interactablen datan seurannan JSONia varten  // <- this but in english
    {
        Debug.Log("start interaction event");

        if (randomButtons != null)
        {
            if (randomButtons.oneByOne && myOrderIndex == 0)
            {
                yield return new WaitForSeconds(1);
            }
        }

        if (!randomButtons)
        {
            yield return new WaitUntil(matrix.PassFaderStatus);
        }
        else
        {
            yield return new WaitUntil(randomButtons.PassFaderStatus);
        }

        Debug.Log("interaction starting, fade over");
        myDuration = 0;

        interactableEvent = new SaveManager.InteractableEvent();
        interactableEvent.trajectory = new();

        interactableEvent.interactableSize = size.ToString().Substring(0,1).ToLower();
        interactableEvent.interactableType = type.ToString().Substring(0,1).ToLower();


    }

    public void EndInteractionEvent() // ends the tracking of the interaction event for this interactable, saves the data to JSON
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

    public static Vector3 StringToVector3(string sVector) // converts gathered position data from string to Vector3, not my code but modified
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
        if (randomButtons != null && randomButtons.IsOneByOne())
        {
            Debug.Log("onebyone");
            StartCoroutine(StartInteractionEvent());

            rendererToChange.material = highlightMaterial;
        }
        
    }


    private void OnDisable()
    {
        if (randomButtons != null && randomButtons.IsOneByOne())
        {
            EndInteractionEvent();

            rendererToChange.material = originalMaterial;
        }
        
    }
    
    public void OneByOneSuccesscheck() // continuous check for interaction success, for sets where interactables show up one by one
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

    public void AllAtOnceSuccessCheck() // same as above but for sets where all interactables show up at once
    {
        if (highlighted)
        {
            if (myOrderIndex != collidables.objects.Count - 1)  //if i am not the last
            {
                rendererToChange.material = originalMaterial;

                var nearestNeighbor = collidables.GetNearestNeighbor(myOrderIndex);
                if (nearestNeighbor != collidables.objects[myOrderIndex + 1]) // if my nearest neighbor is not next in order
                {
                    collidables.objects[myOrderIndex + 1].GetComponent<InteractableActivityManager>().rendererToChange.material = highlightMaterial;
                    StartCoroutine(collidables.objects[myOrderIndex + 1].GetComponent<InteractableActivityManager>().StartInteractionEvent());

                }
                else
                {
                    Debug.Log("next was too close");
                    if(nearestNeighbor.GetComponent<InteractableActivityManager>().myOrderIndex == collidables.objects.Count - 1)
                    {
                        Debug.Log("it's fine its the last one");

                        nearestNeighbor.GetComponent<InteractableActivityManager>().rendererToChange.material = highlightMaterial;
                        StartCoroutine(nearestNeighbor.GetComponent<InteractableActivityManager>().StartInteractionEvent());

                    }
                    else if (collidables.objects[myOrderIndex + 2] != null)
                    {
                        collidables.objects[myOrderIndex + 2].GetComponent<InteractableActivityManager>().rendererToChange.material = highlightMaterial;
                        StartCoroutine(collidables.objects[myOrderIndex + 2].GetComponent<InteractableActivityManager>().StartInteractionEvent());
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

    
    void SetMoveOn(bool b) // set is done
    {
       moveOn = b;
    }

    bool MoveOn() // set is done
    {
        return moveOn;
    }

    public bool CheckMyRays() // raycasts in the cardinal directions relative to this interactable to prevent positions from overlapping or being too close
    {

       

        if (highlighted)
        {
           /* Debug.DrawRay(RaycastStartPos.position, transform.TransformDirection(Vector3.left), Color.blue);
            Debug.DrawRay(RaycastStartPos.position, transform.TransformDirection(Vector3.forward), Color.blue);
            Debug.DrawRay(RaycastStartPos.position, transform.TransformDirection(Vector3.right), Color.blue);
            Debug.DrawRay(RaycastStartPos.position, transform.TransformDirection(-1 * Vector3.forward), Color.blue);
           */

            //CARDINALS
            Debug.DrawRay(rayStartLeft.position, transform.TransformDirection(Vector3.right), Color.white);  
            Debug.DrawRay(rayStartUp.position, transform.TransformDirection(Vector3.forward*-1), Color.blue);
            Debug.DrawRay(rayStartRight.position, transform.TransformDirection(Vector3.left), Color.red);
            Debug.DrawRay(rayStartDown.position, transform.TransformDirection(Vector3.forward), Color.black);

           
            //ORDINALS
            Debug.DrawRay(rayStartUpLeft.position, transform.TransformDirection(1,0,-1), Color.yellow);
            Debug.DrawRay(rayStartUpRight.position, transform.TransformDirection(-1,0,-1), Color.yellow);
            Debug.DrawRay(rayStartDownRight.position, transform.TransformDirection(-1,0,1), Color.yellow);
            Debug.DrawRay(rayStartDownLeft.position, transform.TransformDirection(1,0,1), Color.yellow);


        }
        //   RAYCAST, TRYING RaycastAll
        /* RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(rayStartLeft.position, transform.TransformDirection(Vector3.right), out hit, 0.1f))
        {
           if (hit.collider != null && !colliders.Contains(hit.collider))
           {
               tooClose = true;
               return tooClose;
           }
        }
        else if (Physics.Raycast(rayStartUp.position, transform.TransformDirection(Vector3.forward * -1), 0.1f))
        {
           if (hit.collider != null && !colliders.Contains(hit.collider))
           {
               tooClose = true;
               return tooClose;
           }
       }
        else if (Physics.Raycast(rayStartRight.position, transform.TransformDirection(Vector3.left), 0.1f))
        {
           if (hit.collider != null && !colliders.Contains(hit.collider))
           {
               tooClose = true;
               return tooClose;
           }
       }
        else if (Physics.Raycast(rayStartRight.position, transform.TransformDirection(Vector3.left), 0.1f))
        {
           if (hit.collider != null && !colliders.Contains(hit.collider))
           {
               tooClose = true;
               return tooClose;
           };
        }
        else
        {
            tooClose = false;
            return tooClose;
        }*/
        if (rayCasting)
        {
            RaycastHit[] hitsLeft = Physics.RaycastAll(rayStartLeft.position, transform.TransformDirection(Vector3.right), 0.1f);
            RaycastHit[] hitsUp = Physics.RaycastAll(rayStartUp.position, transform.TransformDirection(Vector3.forward * -1), 0.1f);
            RaycastHit[] hitsRight = Physics.RaycastAll(rayStartRight.position, transform.TransformDirection(Vector3.left), 0.1f);
            RaycastHit[] hitsDown = Physics.RaycastAll(rayStartDown.position, transform.TransformDirection(Vector3.forward), 0.1f);

            RaycastHit[] hitsUpLeft = Physics.RaycastAll(rayStartUpLeft.position, transform.TransformDirection(1, 0, -1), 0.125f);
            RaycastHit[] hitsUpRight = Physics.RaycastAll(rayStartUpRight.position, transform.TransformDirection(-1, 0, -1), 0.125f);
            RaycastHit[] hitsDownRight = Physics.RaycastAll(rayStartDownRight.position, transform.TransformDirection(-1, 0, 1), 0.125f);
            RaycastHit[] hitsDownLeft = Physics.RaycastAll(rayStartDownLeft.position, transform.TransformDirection(1, 0, 1), 0.125f);

            if (hitsLeft != null)
            {
                foreach (RaycastHit hit in hitsLeft)
                {
                    if (!colliders.Contains(hit.collider))
                    {
                        tooClose = true;
                        return true;
                    }
                }
            }

            if (hitsUp != null)
            {
                foreach (RaycastHit hit in hitsUp)
                {
                    if (!colliders.Contains(hit.collider))
                    {
                        tooClose = true;
                        return true;
                    }
                }
            }
            
            if (hitsRight != null)
            {
                foreach (RaycastHit hit in hitsRight)
                {
                    if (!colliders.Contains(hit.collider))
                    {
                        tooClose = true;
                        return true;
                    }
                }
            }

            if (hitsDown != null)
            {
                foreach (RaycastHit hit in hitsDown)
                {
                    if (!colliders.Contains(hit.collider))
                    {
                        tooClose = true;
                        return true;
                    }
                }
            }

            if (hitsUpLeft != null)
            {
                foreach (RaycastHit hit in hitsUpLeft)
                {
                    if (!colliders.Contains(hit.collider))
                    {
                        tooClose = true;
                        return true;
                    }
                }
            }

            if (hitsUpRight != null)
            {
                foreach (RaycastHit hit in hitsUpRight)
                {
                    if (!colliders.Contains(hit.collider))
                    {
                        tooClose = true;
                        return true;
                    }
                }
            }

            if (hitsDownRight != null)
            {
                foreach (RaycastHit hit in hitsDownRight)
                {
                    if (!colliders.Contains(hit.collider))
                    {
                        tooClose = true;
                        return true;
                    }
                }
            }

            if (hitsDownLeft != null)
            {
                foreach (RaycastHit hit in hitsDownLeft)
                {
                    if (!colliders.Contains(hit.collider))
                    {
                        tooClose = true;
                        return true;
                    }
                }
            }

        }
        /*  if (rayCasting)
          {
              // Debug.Log("CASTING");


              RaycastHit[] hitsLeft;

              //hitsLeft = Physics.RaycastAll(RaycastStartPos.position, transform.TransformDirection(Vector3.left), 0.05f);
              hitsLeft = Physics.Raycast(rayStartLeft.position, transform.TransformDirection(Vector3.right), 0.1f);

              RaycastHit[] hitsRight;
             // hitsRight = Physics.RaycastAll(RaycastStartPos.position, transform.TransformDirection(Vector3.right), 0.05f);

              hitsRight= Physics.RaycastAll(rayStartRight.position, transform.TransformDirection(Vector3.left), 0.1f);

              RaycastHit[] hitsUp;
              //hitsUp = Physics.RaycastAll(RaycastStartPos.position, transform.TransformDirection(Vector3.forward), 0.05f);
              hitsUp = Physics.RaycastAll(rayStartUp.position, transform.TransformDirection(Vector3.forward *-1), 0.1f);

              RaycastHit[] hitsDown;
             // hitsDown = Physics.RaycastAll(RaycastStartPos.position, transform.TransformDirection(Vector3.forward * -1), 0.05f);
              hitsDown = Physics.RaycastAll(rayStartDown.position, transform.TransformDirection(Vector3.forward), 0.1f);


              if (hitsLeft.Length > 0) 
              {
                  tooClose = true;
                  return tooClose;
              }
              else if(hitsRight.Length > 0)
              {
                  tooClose = true;
                  return tooClose;
              }
              else if (hitsUp.Length > 0)
              {
                  tooClose = true;
                  return tooClose;
              }
              else if(hitsDown.Length > 0)
              {
                  tooClose = true;
                  return tooClose;
              }

        
          }*/
        tooClose = false;
        return false;
    }

    private void FixedUpdate()
    {
        CheckMyRays();   
    }
    private void Update()  // all checks and other things are being run in here (yrjistä)
    {
        if (!interactSuccess)
            myDuration += Time.deltaTime;

        if (rendererToChange.sharedMaterial == highlightMaterial)
        {
            highlighted = true;
        }
        else
        {
            highlighted = false;
        }

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

        if (handData != null && handData.leftHandTracking)
            leftHandPos.Add(handData.leftHand.handPosition.ToString());

        if (handData != null && handData.rightHandTracking)
            rightHandPos.Add(handData.rightHand.handPosition.ToString());


        if (sessionMode != SessionMode.Matrix)
        {
            if (randomButtons.oneByOne)
            {
                if (interactSuccess)
                    OneByOneSuccesscheck();
            }
            else
            {
                if (interactSuccess)
                    AllAtOnceSuccessCheck();
            }
        }
        else
        {
            if (interactSuccess)
            {
                AllAtOnceSuccessCheck();
            }
        }
        
        

        myRot = myRigidbody.transform.localEulerAngles; // käytä tätä switch interactionsuccess checkis

        if (Input.GetButtonDown("InteractSuccess")) // for debugging purposes
        {
            if (sessionMode != SessionMode.Matrix)
            {
                if (randomButtons.oneByOne)
                {
                    interactSuccess = true;
                }
                else
                {
                    if (highlighted)
                        interactSuccess = true;
                }
            }
            else
            {
                if (highlighted)
                    interactSuccess = true;
            }
        }
    }
}
