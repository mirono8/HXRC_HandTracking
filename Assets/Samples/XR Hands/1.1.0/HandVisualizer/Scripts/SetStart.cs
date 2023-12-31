using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SetStart : MonoBehaviour  // GAMEOBJECT HAS TO BE UNENABLED ON START
{
    public GetSetupData setupData;

    SessionManager sessionManager;

    string size;
    string type;
    [SerializeField]
    string mode;

    [SerializeField]
    int rounds;   //after all panels are used once

    [SerializeField]
    int interactableCount; //lis�� netti sivulle!!

    public int columnCount; //t�� kabns!!!

    public GameObject gridPrefab;

    public List<GameObject> interactablePrefabs;

    public List<GameObject> currentSetGameObjs;

    public List<GameObject> setGrid;

    public List<GridToPanel.WarpPoint> warpPoints;

    [SerializeField]
    public GameObject currentGrid;

    public FadeIn fadeIn;

    [SerializeField]
    GameObject rig;

    int currentSetNumber;

    bool once;

    public bool automaticFade;

    public bool collidablesReady;

    public TrackColliders leftHandColliders;
    public TrackColliders rightHandColliders;

    bool colliderActiveStatus;

    bool modeChanged = true;
    //GATHER AUDIOSOURCES AND WAIT UNTIL NONE IS PLAYING

    private void OnEnable()
    {
        size = setupData.ReturnSize(0);
        type = setupData.ReturnType(0);
        mode = setupData.ReturnMode(0);
        //interactableCount = setupData.ReturnInteractableCount(0); lis�� ku on lis�tty json
        interactableCount = setupData.ReturnInteractableCount(0);
        columnCount = interactableCount; //ehk� kaks eri arvoo?
        rig = GameObject.FindGameObjectWithTag("Player");
        sessionManager = GameObject.FindGameObjectWithTag("SessionManager").GetComponent<SessionManager>();
        automaticFade = true;
    }

    private void Start()
    { 
        currentGrid = Instantiate(gridPrefab, gameObject.transform);
        setGrid.Add(currentGrid);
    }

    private void Update()
    {
        DisableCollision();
    }
    public void ModeHasChanged(bool isTrue)
    {
        modeChanged = isTrue;
    }

    public void SetupInteractables() // sets the interactables for current set active and adjusts them based on loaded values
    {
        
        var temp = interactablePrefabs.Find(x => x.name.ToString().ToLower().Contains(type));
        Debug.Log(temp);
        InteractableActivityManager.InteractableSize sizeAsEnum = (InteractableActivityManager.InteractableSize)Enum.Parse(typeof(InteractableActivityManager.InteractableSize), size, true);
        Debug.Log(sizeAsEnum);

        InteractableActivityManager.SessionMode modeEnum = (InteractableActivityManager.SessionMode)Enum.Parse(typeof(InteractableActivityManager.SessionMode), mode, true);

        if (mode != "matrix")
        {
            //grid.GetComponent<RandomButtons>().enabled = true;
            currentGrid.AddComponent<RandomButtons>();

            if(size == "large")
                currentGrid.GetComponent<RandomButtons>().largeSet = true;

            if (mode != "all")
            {
                currentGrid.GetComponent<RandomButtons>().oneByOne = true;
            }
        }
        else
        {
            //matrix stuff!!!!
            // grid.GetComponent<ButtonMatrix>().enabled = true;
            //grid.GetComponent<RandomButtons>().enabled = false;
            currentGrid.AddComponent<ButtonMatrix>();
        }

        if (temp != null)
        {
            if (mode != "matrix")
            {
                for (int i = 0; i < interactableCount; i++)
                {

                    /*if (i != 0)
                    {
                        var x = Instantiate(temp, grid.transform);
                        //x.SetActive(false);
                        x.GetComponent<InteractableActivityManager>().size = sizeAsEnum;
                        x.GetComponent<InteractableActivityManager>().SetMySize();


                    }
                    else
                    {
                        var x = Instantiate(temp, grid.transform);
                        //x.SetActive(true);
                        x.GetComponent<InteractableActivityManager>().size = sizeAsEnum;
                        x.GetComponent<InteractableActivityManager>().SetMySize();
                    }*/

                    var x = Instantiate(temp, currentGrid.transform.GetChild(0));
                    x.name = x.GetComponent<InteractableActivityManager>().type.ToString() + i;
                    x.GetComponent<InteractableActivityManager>().sessionMode = modeEnum;
                    x.GetComponent<InteractableActivityManager>().size = sizeAsEnum;
                    x.GetComponent<InteractableActivityManager>().SetMySize();

                    currentSetGameObjs.Add(x);
                    
                }
            }
            else
            {
                int nameIteration = 0;
                for (int j = 0; j < columnCount; j++)
                {

                    for (int i = 0; i < interactableCount; i++)
                    {


                        var x = Instantiate(temp, currentGrid.transform.GetChild(0));
                        x.name = x.GetComponent<InteractableActivityManager>().type.ToString() + nameIteration;
                        x.GetComponent<InteractableActivityManager>().sessionMode = modeEnum;
                        x.GetComponent<InteractableActivityManager>().size = sizeAsEnum;
                        x.GetComponent<InteractableActivityManager>().SetMySize();

                        currentSetGameObjs.Add(x);
                        nameIteration++;
                    }
                    nameIteration++;
                }

                if (currentSetGameObjs.Count < currentGrid.GetComponent<ButtonMatrix>().interactionsGoal)  //if grid is smaller than 10 objects total, add hidden objects to roll over in the matrix
                {
                    for (int i = currentSetGameObjs.Count; i < currentGrid.GetComponent<ButtonMatrix>().interactionsGoal; i++)
                    {
                        var x = Instantiate(temp, currentGrid.transform.GetChild(0));
                        x.name = x.GetComponent<InteractableActivityManager>().type.ToString() + i;
                        x.GetComponent<InteractableActivityManager>().sessionMode = modeEnum;
                        x.GetComponent<InteractableActivityManager>().size = sizeAsEnum;
                        x.GetComponent<InteractableActivityManager>().SetMySize();

                        currentGrid.GetComponent<ButtonMatrix>().extraObjs.Add(x);
                        //x.SetActive(false);

                        currentSetGameObjs.Add(x);
                    }
                }
            }

            StartCoroutine(currentGrid.GetComponent<CollidableObjects>().PopulateCollidables());



            StartCoroutine(WaitForFade());

            currentGrid.GetComponent<CollidableObjects>().ToggleColliders(false);
            collidablesReady = true;
        }
    }

    public void ClearComponents()
    {
        if (currentGrid.GetComponent<RandomButtons>() != null)
        {
            Debug.Log("destroy randombuttons");
            Destroy(currentGrid.GetComponent<RandomButtons>());
        }
        else
        {
            Debug.Log("destroy matrix");
            Destroy(currentGrid.GetComponent<ButtonMatrix>());
        }

        currentGrid.GetComponent<CollidableObjects>().ResetRandomOrder();
    }

    public void ClearCurrentSet() // clears data from the current set after completion 
    {
        if (currentSetGameObjs.Any())
        {
            for (int i = 0; i < currentSetGameObjs.Count; i++)
            {
                currentSetGameObjs[i].GetComponent<InteractableActivityManager>().intersectionCollider.enabled = false;

                if (currentSetGameObjs[i].activeSelf)
                    currentSetGameObjs[i].SetActive(false);

               // currentSetGameObjs[i].transform.parent = GameObject.FindGameObjectWithTag("Garbage").transform;
            }
            currentSetGameObjs.Clear();
            currentGrid.GetComponent<CollidableObjects>().objects.Clear(); //first grid

            collidablesReady = false;
        }
    }

    public void GameObjectsToTrack() // starts tracking gameobjects of the current set
    {
        var gridActual = currentGrid.transform.GetChild(0);
        Debug.Log("oppa");
        if (!currentGrid.GetComponent<CollidableObjects>().objects.Any())  //second grid if there are more
        {
            for (int i = 0; i < gridActual.childCount; i++)
            {
                currentGrid.GetComponent<CollidableObjects>().objects.Add(currentSetGameObjs[i]);
            }
        }
    }

    public IEnumerator RunSet() // gives the go signal for the grid
    {
        yield return new WaitForSeconds(0.3f);

        if (mode != "matrix")
        {
            if (mode != "all")
            {
                currentGrid.GetComponent<RandomButtons>().oneByOne = true;
            }
            else
            {
                currentGrid.GetComponent<RandomButtons>().oneByOne = false;
            }

           // StartCoroutine(grid.GetComponent<RandomButtons>().ReadyForSetup()); 

        }
        else
        {
            //matrix stuff reloaded!
            Debug.Log("matrix go!!");
            //StartCoroutine(grid.GetComponent<ButtonMatrix>().ReadyForSetup()); 


        }
    }


    public IEnumerator WaitForFade() // suspends operation until set has loaded and calls the method to possibly adjust camera position
    {
        fadeIn.FaderInfoText(mode);  //tarkista t��

        
        StartCoroutine(fadeIn.FadeCanvasIn());

        

        if (!automaticFade)
        {
            
            Debug.Log("wait for user in setstart");
          //  fadeIn.ChangeFaderStatus();
            StartCoroutine(fadeIn.WaitForUser(currentGrid, modeChanged));  
        }
 
        Debug.Log("waiting");
        if (!automaticFade)
        {
            yield return new WaitUntil(fadeIn.FaderisFadedOut);
        }
        else
        {
            yield return new WaitWhile(fadeIn.FaderisFadedOut);
        }

        if (currentGrid.GetComponent<RandomButtons>())
        {
            yield return new WaitUntil(currentGrid.GetComponent<RandomButtons>().GetSetStatus);
        }
        else
        {
            //matrix stuff!!
            yield return new WaitUntil(currentGrid.GetComponent<ButtonMatrix>().AllocateRows);
        }

        rig.GetComponent<VrCamStartPos>().RotateWhileTrue(true);

        if (currentSetNumber != 0)
        {
            rig.GetComponent<VrCamStartPos>().WarpToNextPanel(warpPoints[currentSetNumber]);
        }

        Debug.Log("i can wait no longer");
        rig.GetComponent<VrCamStartPos>().RotateWhileTrue(false);

        if (automaticFade)
        {
            StartCoroutine(fadeIn.FadeCanvasOut());
        }
        
        
        Debug.Log("wait for fade over");
    }

    public void DisableCollision()
    {
        if (sessionManager.CurrentState() == States.State.Paused && colliderActiveStatus == true)
        {

            currentGrid.GetComponent<CollidableObjects>().ToggleColliders(true);
            leftHandColliders.ToggleTrigger(true);
            rightHandColliders.ToggleTrigger(true);
            colliderActiveStatus = false;
        }
        else if (sessionManager.CurrentState() != States.State.Paused && colliderActiveStatus == false)
        {
            currentGrid.GetComponent<CollidableObjects>().ToggleColliders(false);
            leftHandColliders.ToggleTrigger(false);
            rightHandColliders.ToggleTrigger(false);
            colliderActiveStatus = true;
        }
    }

    public string CurrentSessionMode()
    {
        return mode;
    }

    public int CurrentInteractableCount()
    {
        return interactableCount;
    }
    public void GetCurrentSetNumber(int i)
    {
        currentSetNumber = i;
    }

    public int CurrentRound()
    {
        return rounds;
    }

    public void ResetRoundCounter()
    {
        rounds = 0;
    }

    public void AssignSetParams(int round, bool reusePanel = false) // assigns current set's enumerables and finds the next free panel to use / reuses a panel if there are no unused panels
    {
        var p = GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>();

        size = setupData.ReturnSize(round);
        type = setupData.ReturnType(round);
        mode = setupData.ReturnMode(round);
        interactableCount = setupData.ReturnInteractableCount(round);
        columnCount = interactableCount;
        Debug.Log("new params " + size + type);

       p.LastPanelUsed(currentGrid.transform.parent.gameObject);

        Destroy(currentGrid);

        if (!reusePanel)
        {
            currentGrid = setGrid[round];
        }
        else
        {
            if (!once)
            {
                for (int i = 0; i < p.panels.Count; i++)
                {
                    p.FreeThisPanel(p.panels[i].panel);
                }

                once = true;
            }

            currentGrid = setGrid[round];
            currentGrid.SetActive(true);

           // grid.transform.parent = GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>().GiveMeAPanel().transform; //GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>().ReusePanel().transform;
            Debug.Log("reusing panel");
        }

        
        rounds++;
    }
}
