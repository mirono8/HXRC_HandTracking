using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SetStart : MonoBehaviour
{
    public GetSetupData setupData;

    string size;
    string type;
    [SerializeField]
    string mode;

    [SerializeField]
    int interactableCount; //lis�� netti sivulle!!

    public int columnCount; //t�� kabns!!!

    public GameObject gridPrefab;

    public List<GameObject> interactablePrefabs;

    public List<GameObject> currentSetGameObjs;

    public List<GameObject> setGrid;

    public List<GridToPanel.WarpPoint> warpPoints;

    [SerializeField]
    GameObject grid;

    public FadeIn fadeIn;

    [SerializeField]
    GameObject rig;

    int currentSetNumber;

    bool readyToRun;

    private void OnEnable()
    {
        size = setupData.ReturnSize(0);
        type = setupData.ReturnType(0);
        mode = setupData.ReturnMode(0);
        //interactableCount = setupData.ReturnInteractableCount(0); lis�� ku on lis�tty json
        interactableCount = setupData.ReturnInteractableCount(0);
        columnCount = interactableCount; //ehk� kaks eri arvoo?
        rig = GameObject.FindGameObjectWithTag("Player");

    }

    private void Start()
    {
        grid = Instantiate(gridPrefab, gameObject.transform);
        setGrid.Add(grid);
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
            grid.AddComponent<RandomButtons>();

            if (mode != "all")
            {
                grid.GetComponent<RandomButtons>().oneByOne = true;
            }
        }
        else
        {
            //matrix stuff!!!!
            // grid.GetComponent<ButtonMatrix>().enabled = true;
            //grid.GetComponent<RandomButtons>().enabled = false;
            grid.AddComponent<ButtonMatrix>();
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

                    var x = Instantiate(temp, grid.transform.GetChild(0));
                    x.GetComponent<InteractableActivityManager>().sessionMode = modeEnum;
                    x.GetComponent<InteractableActivityManager>().size = sizeAsEnum;
                    x.GetComponent<InteractableActivityManager>().SetMySize();

                    currentSetGameObjs.Add(x);
                }
            }
            else
            {
                for (int j = 0; j < columnCount; j++)
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

                        var x = Instantiate(temp, grid.transform.GetChild(0));
                        x.GetComponent<InteractableActivityManager>().sessionMode = modeEnum;
                        x.GetComponent<InteractableActivityManager>().size = sizeAsEnum;
                        x.GetComponent<InteractableActivityManager>().SetMySize();

                        currentSetGameObjs.Add(x);
                    }
                }
            }

            StartCoroutine(WaitForFade());
        }
    }

    public void ClearComponents()
    {
        if (grid.GetComponent<RandomButtons>() != null)
        {
            Debug.Log("destroy randombuttons");
            Destroy(grid.GetComponent<RandomButtons>());
        }
        else
        {
            Debug.Log("destroy matrix");
            Destroy(grid.GetComponent<ButtonMatrix>());
        }

        grid.GetComponent<CollidableObjects>().ResetRandomOrder();
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

                currentSetGameObjs[i].transform.parent = GameObject.FindGameObjectWithTag("Garbage").transform;
            }
            currentSetGameObjs.Clear();
            grid.GetComponent<CollidableObjects>().objects.Clear(); //first grid

            
        }
    }

    public void GameObjectsToTrack() // starts tracking gameobjects of the current set
    {
        var gridActual = grid.transform.GetChild(0);
        Debug.Log("oppa");
        if (!grid.GetComponent<CollidableObjects>().objects.Any())  //second grid if there are more
        {
            for (int i = 0; i < gridActual.childCount; i++)
            {
                grid.GetComponent<CollidableObjects>().objects.Add(currentSetGameObjs[i]);
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
                grid.GetComponent<RandomButtons>().oneByOne = true;
            }
            else
            {
                grid.GetComponent<RandomButtons>().oneByOne = false;
            }

            StartCoroutine(grid.GetComponent<RandomButtons>().ReadyForSetup());

        }
        else
        {
            //matrix stuff reloaded!
            Debug.Log("matrix go!!");
            StartCoroutine(grid.GetComponent<ButtonMatrix>().ReadyForSetup());

        }
    }


    public IEnumerator WaitForFade() // suspends operation until set has loaded and calls the method to possibly adjust camera position
    {
        StartCoroutine(fadeIn.FadeCanvasIn());
        Debug.Log("waiting");
        yield return new WaitWhile(fadeIn.FaderStatus);

        if (grid.GetComponent<RandomButtons>())
        {
            yield return new WaitUntil(grid.GetComponent<RandomButtons>().GetSetStatus);
        }
        else
        {
            //matrix stuff!!
            yield return new WaitUntil(grid.GetComponent<ButtonMatrix>().AllocateRows);
        }

        rig.GetComponent<VrCamStartPos>().RotateWhileTrue(true);

        rig.GetComponent<VrCamStartPos>().WarpToNextPanel(warpPoints[currentSetNumber]);

        Debug.Log("i can wait no longer");
        rig.GetComponent<VrCamStartPos>().RotateWhileTrue(false);
        StartCoroutine(fadeIn.FadeCanvasOut());

        Debug.Log("wait for fade over");
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
    public void AssignSetParams(int round, bool reusePanel = false) // assigns current sets enumerables and finds the next free panel to use / reuses a panel if there are no unused panels
    {
        size = setupData.ReturnSize(round);
        type = setupData.ReturnType(round);
        mode = setupData.ReturnMode(round);
        interactableCount = setupData.ReturnInteractableCount(round);
        columnCount = interactableCount;
        Debug.Log("new params " + size + type);

        if (!reusePanel)
        {
            grid = setGrid[round];
        }
        else
        {
            GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>().ReusePanel();
            Debug.Log("reusing panel " + grid.transform.parent.gameObject.name);
        }
    }
}
