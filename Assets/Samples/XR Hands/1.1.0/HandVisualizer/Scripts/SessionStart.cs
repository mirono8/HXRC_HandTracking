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
        if (temp != null)
        {
            temp.GetComponent<InteractableActivityManager>().size = sizeAsEnum;

            for (int i = 0; i < 10; i++)
            {
                if (i != 0)
                {
                    Instantiate(temp, grid.transform).SetActive(false);

                }
                else
                {
                    Instantiate(temp, grid.transform);
                }
            }
        }
    }
}
