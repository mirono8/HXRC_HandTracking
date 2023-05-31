using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SessionStart : MonoBehaviour
{
    public GetSetupData setupData;

    string size;
    string type;

    public List<GameObject> interactablePrefabs;

    GameObject grid;
    private void OnEnable()
    {
        size = setupData.ReturnSize();
        type = setupData.ReturnType();

    }

    private void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Grid");

        //size ei tee mitää jostai syyst lol

        var temp = interactablePrefabs.Find(x => x.name.ToString().ToLower().Contains(type));
        Debug.Log(temp);
        InteractableActivityManager.InteractableSize sizeAsEnum = (InteractableActivityManager.InteractableSize) Enum.Parse(typeof(InteractableActivityManager.InteractableSize), size, true);
        Debug.Log(sizeAsEnum);
        if (temp != null)
        {
            for (int i = 0; i < 10; i++)
            {
                if (i != 0)
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
                }
            }
        }
    }
}
