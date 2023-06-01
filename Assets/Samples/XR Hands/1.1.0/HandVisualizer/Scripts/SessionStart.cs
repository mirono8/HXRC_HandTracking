using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SessionStart : MonoBehaviour
{
    public GetSetupData setupData;

    string size;
    string type;

    public GameObject gridPrefab;

    public List<GameObject> interactablePrefabs;

    public List<GameObject> currentSetGameObjs;

    public List<GameObject> setGrid;

    GameObject grid;
    private void OnEnable()
    {
        size = setupData.ReturnSize(0);
        type = setupData.ReturnType(0);

    }

    private void Start()
    {
        grid = Instantiate(gridPrefab, gameObject.transform);

        //size ei tee mitää jostai syyst lol
        RunSet();
    }

    public void RunSet()
    {
        var temp = interactablePrefabs.Find(x => x.name.ToString().ToLower().Contains(type));
        Debug.Log(temp);
        InteractableActivityManager.InteractableSize sizeAsEnum = (InteractableActivityManager.InteractableSize)Enum.Parse(typeof(InteractableActivityManager.InteractableSize), size, true);
        Debug.Log(sizeAsEnum);


        if (currentSetGameObjs.Count > 0)
        {
            for (int i = 0; i < 10; i++)
            {
                currentSetGameObjs[i].SetActive(false);
            }
            currentSetGameObjs.Clear();
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

                var x = Instantiate(temp, grid.transform);
                x.GetComponent<InteractableActivityManager>().size = sizeAsEnum;
                x.GetComponent<InteractableActivityManager>().SetMySize();

                currentSetGameObjs.Add(x);
            }
        }
    }

    public void AssignSetParams(int round)
    {
        size = setupData.ReturnSize(round);
        type = setupData.ReturnType(round);
        grid = setGrid[round];
    }

}

