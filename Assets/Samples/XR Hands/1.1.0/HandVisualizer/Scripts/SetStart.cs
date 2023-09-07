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
    string mode;

    public GameObject gridPrefab;

    public List<GameObject> interactablePrefabs;

    public List<GameObject> currentSetGameObjs;

    public List<GameObject> setGrid;

    public List<Transform> warpPoints;

    [SerializeField]
    GameObject grid;

    public FadeIn fadeIn;

    [SerializeField]
    GameObject rig;

    int currentSetNumber;


    private void OnEnable()
    {
        size = setupData.ReturnSize(0);
        type = setupData.ReturnType(0);
        mode = setupData.ReturnMode(0);

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

        if (mode != "all")
        {
            grid.GetComponent<RandomButtons>().oneByOne = true;
        }

        if (temp != null)
        {

            for (int i = 0; i < 10; i++)
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
                x.GetComponent<InteractableActivityManager>().size = sizeAsEnum;
                x.GetComponent<InteractableActivityManager>().SetMySize();

                currentSetGameObjs.Add(x);
            }
            
            
        }
        StartCoroutine(WaitForFade());
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
            }
            currentSetGameObjs.Clear();
            grid.GetComponent<CollidableObjects>().objects.Clear(); //first grid
        }
    }

    public void GameObjectsToTrack() // starts tracking gameobjects of the current set
    {
        if (!grid.GetComponent<CollidableObjects>().objects.Any())  //second grid if there are more
        {
            for (int i = 0; i < 10; i++)
            {
                grid.GetComponent<CollidableObjects>().objects.Add(currentSetGameObjs[i]);
            }
        }
    }

    public void RunSet() // gives the go signal for the grid
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

    public IEnumerator WaitForFade() // suspends operation until set has loaded and calls the method to possibly adjust camera position
    {
        fadeIn.FadeBack();
        Debug.Log("waiting");
        rig.GetComponent<VrCamStartPos>().WarpToNextPanel(warpPoints[currentSetNumber]);
        yield return new WaitUntil(grid.GetComponent<RandomButtons>().GetSetStatus);
        Debug.Log("i can wait no longer");
        fadeIn.StartFade();
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
        Debug.Log("new params " + size + type);

        if (!reusePanel) {
            grid = setGrid[round];
        }
        else
        {
            GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>().ReusePanel();
            Debug.Log("reusing panel " + grid.transform.parent.gameObject.name);
        }
    }
}