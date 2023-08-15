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

    [SerializeField]
    GameObject grid;

    private void OnEnable()
    {
        size = setupData.ReturnSize(0);
        type = setupData.ReturnType(0);
        mode = setupData.ReturnMode(0);

    }

    private void Start()
    {
        grid = Instantiate(gridPrefab, gameObject.transform);
        setGrid.Add(grid);
    }

    public void SetupInteractables()
    {
        var temp = interactablePrefabs.Find(x => x.name.ToString().ToLower().Contains(type));
        Debug.Log(temp);
        InteractableActivityManager.InteractableSize sizeAsEnum = (InteractableActivityManager.InteractableSize)Enum.Parse(typeof(InteractableActivityManager.InteractableSize), size, true);
        Debug.Log(sizeAsEnum);

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
    }

    public void ClearCurrentSet()
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

    public void GameObjectsToTrack()
    {
        if (!grid.GetComponent<CollidableObjects>().objects.Any())  //second grid if there are more
        {
            for (int i = 0; i < 10; i++)
            {
                grid.GetComponent<CollidableObjects>().objects.Add(currentSetGameObjs[i]);
            }
        }
    }

    public void RunSet()
    {
        if (mode != "all")
        {
            grid.GetComponent<RandomButtons>().oneByOne = true;
        }else
        { grid.GetComponent<RandomButtons>().oneByOne=false; }

        StartCoroutine(grid.GetComponent<RandomButtons>().ReadyForSetup());
    }

    public void AssignSetParams(int round, bool reusePanel = false)
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