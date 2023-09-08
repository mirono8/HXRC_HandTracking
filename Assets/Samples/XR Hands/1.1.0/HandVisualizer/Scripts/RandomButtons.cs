using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomButtons : MonoBehaviour
{
    public bool setAgain;
    CollidableObjects collidables;

    public List<Vector3> originalPositions = new List<Vector3>();

    public List<int> intersecting = new List<int>();

    public Vector3 panelScale;

    public bool oneByOne;

    PanelManager panelManager;

    [SerializeField]
    bool setReady;

    bool loopOngoing;

    bool checkingBounds;

    FadeIn fader;

    public IEnumerator ReadyForSetup() // initial call during scene start, finds stuff for the script and starts randomizing positions for the first sets
    {

        collidables = GetComponent<CollidableObjects>();

        foreach (GameObject x in collidables.objects)
        {
            originalPositions.Add(x.transform.localPosition);
        }
        panelScale = GetComponent<GridToPanel>().panelScale;
        panelManager = GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>();

        StartCoroutine(SetRandomPositions());

        yield return new WaitWhile(LoopStatus);
        if (!oneByOne)
        {
            
            StartCoroutine(LoopingIntersectSetter());
        }
        else
        {
            setReady = true;
        }
    }

    public IEnumerator SetRandomPositions() // sets random positions for the interactables of current set based on panel size and mode
    {
        loopOngoing = true;
        for (int i = 0; i < collidables.objects.Count; i++)
        {
            // var deviation = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.2f, 0.2f));
            var deviationInPanel = new Vector3(UnityEngine.Random.Range(panelScale.x * -0.4f, panelScale.x * 0.4f),
                UnityEngine.Random.Range(panelScale.y * -0.4f, panelScale.y * 0.4f));

            collidables.objects[i].transform.localPosition = new Vector3(collidables.objects[i].transform.localPosition.x + deviationInPanel.x,
                collidables.objects[i].transform.localPosition.y + deviationInPanel.y, 0f);

            yield return new WaitForEndOfFrame();

            if (oneByOne)
            {
                collidables.objects[i].transform.eulerAngles = new Vector3(-90f, collidables.objects[i].transform.eulerAngles.y,   
                    collidables.objects[i].transform.eulerAngles.z);
            }

            if (!oneByOne)
            {
                Debug.Log("setting" + i + " indexed active");
                collidables.objects[i].SetActive(true);

                
                StartCoroutine(CheckBoundIntersection(i));
                yield return new WaitWhile(CheckingBounds);
            }
            else if (i == 0)
                collidables.objects[i].SetActive(true);
        }
        
        yield return new WaitForEndOfFrame();
        loopOngoing = false;

    }


    public IEnumerator CheckBoundIntersection(int index) // works together with raycasting from the interactable proper, also old code with bounds.intersects, might re-implement it later as a fallback
    {
        checkingBounds = true;
        for (int i = 0; i < collidables.objects.Count; i++)
        {

           /* if (i != index && collidables.objects[index].GetComponent<InteractableActivityManager>().intersectionCollider.bounds.Intersects(collidables.objects[i].GetComponent<InteractableActivityManager>().intersectionCollider.bounds)) // i != index && collidables.objects[index].GetComponentInChildren<Collider>().bounds.Intersects(collidables.objects[i].GetComponentInChildren<Collider>().bounds))
            {
                collidables.objects[index].GetComponent<InteractableActivityManager>().boundsCollide = true;
                if (!intersecting.Contains(index))
                {
                    //intersecting.Add(index);
                }
             //   return true;
            }
            else
                collidables.objects[index].GetComponent<InteractableActivityManager>().boundsCollide = false;
            //OLD CODE WITH BOUNDS, WORKING TO REPLACE WITH RAYS */

            //yield return new WaitForEndOfFrame();
            // NEW CODE WITH RAYS

            if (i != index && collidables.objects[index].GetComponent<InteractableActivityManager>().tooClose)
            {

                collidables.objects[index].GetComponent<InteractableActivityManager>().rayCasting = true;
                if (!intersecting.Contains(index))
                {
                    Debug.Log(index + " added to intersecting");
                    intersecting.Add(index);

                }
                //yield return true;
            }
            else if (index == collidables.objects.Count - 1)
            {
                if (collidables.objects[index].GetComponent<InteractableActivityManager>().tooClose)
                {
                    if (!intersecting.Contains(index))
                    {
                        Debug.Log(index + " added to intersecting");
                        intersecting.Add(index);

                    }
                    //yield return true;
                }
                else
                {
                    if (intersecting.Contains(index))
                    {
                        Debug.Log("removing " + index);
                        intersecting.Remove(index);
                    }
                }
            }
            else
            {

                if (intersecting.Contains(index))
                {
                    Debug.Log("removing " + index);
                    intersecting.Remove(index);
                }
            }
            
        }


        //  collidables.objects[index].GetComponent<InteractableActivityManager>().rayCasting = false;

        collidables.objects[index].transform.localPosition = new Vector3(collidables.objects[index].transform.localPosition.x,
                collidables.objects[index].transform.localPosition.y, 0f);

        collidables.objects[index].transform.eulerAngles = new Vector3(-90f, collidables.objects[index].transform.eulerAngles.y,   //TÄÄ OLI SE ISO ISSUE MOLEMMISSA TARKISTUSTAVOISSA, PITÄÄ HETI KÄÄNTÄÄ OIKEIN PÄIN, EI VIEL PERFECT MUT JATKA
                    collidables.objects[index].transform.eulerAngles.z);

        yield return false;
        checkingBounds = false;
    }

    public bool LoopStatus() // is the randomizer still ongoing, this is for script sequencing purposes
    {
        return loopOngoing;
    }

    public bool CheckingBounds() // same as above, during bounds collision check
    {
        return checkingBounds;
    }

    public bool GetSetStatus() // same as above, when everything is ready
    {
        return setReady;
    }

    public bool PassFaderStatus() // passes fader status from the fader script, interactables need to wait for this to start tracking time for the interaction event
    {
        return fader.FaderStatus();
    }

    public IEnumerator LoopingIntersectSetter() // iterates through interactables that intersect each other
    {
        setReady = false;
        Debug.Log("Starting loop");

        while (intersecting.Count > 0)
        {
           // yield return new WaitForEndOfFrame();
            Debug.Log("while loop!");
            yield return null;
            if (intersecting.Count > 0)
            {
                    SetIntersectingPositions(0);
                    StartCoroutine(CheckBoundIntersection(intersecting[0]));
                    yield return new WaitWhile(CheckingBounds);
                
            }
        }

        yield return new WaitForEndOfFrame();
        for (int i = 0; i < collidables.objects.Count; i++)
        {
            StartCoroutine(CheckBoundIntersection(i));
            yield return new WaitWhile(CheckingBounds);
        }

        yield return null;
        
        if (intersecting.Count > 0)
        {
            Debug.Log("Second loop in looping setter");
            StartCoroutine(LoopingIntersectSetter());
        }
        else
        {
            Debug.Log("loop end in grid with children count of " + originalPositions.Count);
            for (int i = 0; i < collidables.objects.Count; i++)
            {
                collidables.objects[i].transform.localPosition = new Vector3(collidables.objects[i].transform.localPosition.x,
                collidables.objects[i].transform.localPosition.y, 0f);

                collidables.objects[i].transform.eulerAngles = new Vector3(-90f, collidables.objects[i].transform.eulerAngles.y,   //muuta hardcoded rot pois!½!!
                    collidables.objects[i].transform.eulerAngles.z);// panelManager.GetPanelBackward(GetComponent<GridToPanel>().SendPanel());

                collidables.objects[i].GetComponent<InteractableActivityManager>().ToggleKinematic(false);
                setReady = true;
            }
        }

    }

    public void SetIntersectingPositions(int index) // used by the recursive loop, sets the positions of intersecting objects again randomly
    {

        Debug.Log("intersecting set");
        var deviationAgain = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.2f, 0.2f));
        collidables.objects[intersecting[index]].transform.localPosition = originalPositions[intersecting[index]] + deviationAgain;

        collidables.objects[index].transform.localPosition = new Vector3(collidables.objects[index].transform.localPosition.x,
                collidables.objects[index].transform.localPosition.y, 0f);

        //intersectaa viel välil joskus lul
    }
    public bool IsOneByOne() // get mode
    {
        return oneByOne;
    }

    void DoubleCheck() // redundant, debugging method for bounds intersection
    {
        Debug.Log("double check");
        var j = 0;
        for (int i = 0; i < collidables.objects.Count; i++)
        {
            if (i != j && collidables.objects[j].GetComponent<InteractableActivityManager>().intersectionCollider.bounds.Intersects(collidables.objects[i].
                GetComponent<InteractableActivityManager>().intersectionCollider.bounds)) // i != index && collidables.objects[index].GetComponentInChildren<Collider>().bounds.Intersects(collidables.objects[i].GetComponentInChildren<Collider>().bounds))
            {
                Debug.Log("double double check");
                if (!intersecting.Contains(j))
                {
                    Debug.Log("added " + j);
                    intersecting.Add(j);
                }
            }
        }
    }

    private void Start()
    {
        fader = GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeIn>();
    }

    private void Update()
    {

        if (Input.GetButtonDown("FunnyTestKey")) // for debugging purposes
            DoubleCheck();
    }

   
}
