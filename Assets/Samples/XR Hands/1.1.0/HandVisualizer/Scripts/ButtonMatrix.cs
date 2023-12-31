using HandData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonMatrix : MonoBehaviour
{
    CollidableObjects collidables;

    GridToPanel grid;

    FadeIn fader;

    SetStart setData;
    //int socketCount = 5;

    int columnCount; //t�� setupdatast!!!!   also cap matrix to 9x9!!!
 
    [SerializeField]
    int currentRow = 0;   

    [SerializeField]
    int objsPerColumn;

    [SerializeField]
    List<float> rows = new();

    [SerializeField]
    List<float> columns = new();

    bool rowsSet;

    bool columnsSet;

    float rowInterval;

    float columnInterval;

    [SerializeField]
    int interactions = 0;

    bool readyToTrack;

    public int interactionsGoal = 10;

    [SerializeField]  // if you want more interactions than the amount of available buttons
    bool rollover = true;

    public List<GameObject> extraObjs = new();

    public List<Vector3> previousPositions = new();

    public List<Group> groupList = new();

    [Serializable]
    public class Group
    {
        /*public GameObject activeObject;
        public GameObject inactiveObject;*/

        public List<GameObject> groupedObjs = new();
        public bool processed = false;
    }

    private void Start()
    {
        fader = GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeIn>();
        setData = GameObject.FindGameObjectWithTag("SessionManager").GetComponentInChildren<SetStart>();
        collidables = GetComponent<CollidableObjects>();
    }
    public IEnumerator ReadyForSetup()
    {

        
        grid = GetComponent<GridToPanel>();
        collidables.ToggleColliders(true);

        objsPerColumn = setData.columnCount;
        columnCount = setData.columnCount;

        for (int i = 0; i < objsPerColumn; i++)
        {
            columns.Add(0f);
        }

        for (int i = 0; i < objsPerColumn; i++)
        {
            rows.Add(0f);
        }
        // Debug.Log(collidables.objects[0].transform.lossyScale);

        SetUpColumns();

        yield return new WaitUntil(AllocateColumns);

        SetUpRows();

        yield return new WaitUntil(AllocateRows);

        collidables.RandomizeOrderIndex(extraObjs.Count);

        yield return new WaitUntil(collidables.RandomOrderReady);
        ArrangeInteractables();


        readyToTrack = true;


        MatrixInteractionCheck();

    }

    void SetUpColumns()
    {
        columnsSet = false;

        var pHeight = grid.panelScale.y;

        columnInterval = pHeight / columnCount;

        var j = 0;

        for (int i = 0; i < columnCount; i++)
        {

            if (i != 0)
            {
                var origin = columns[0];
              //  Debug.Log("modulo" + (i % 2));
                if (i % 2 == 0)
                {
                    if (j > 0)
                    {
                        columns[i] = (origin - columnInterval * (j + 1)); ////sockets[4]
                    }
                    else
                    {
                        columns[i] = (origin - columnInterval);//sockets[2]
                    }

                    j++;
                }
                else
                {
                    if (j > 0)
                    {
                        columns[i] = origin + columnInterval * (j + 1); //sockets[3,5]
                    }
                    else
                    {
                        columns[i] = origin + columnInterval; //sockets[1]
                    }
                }
            }
            else
            {
                columns[i] = 0f;
            }
        }


        columnsSet = true;
    }

    void SetUpRows()
    {
        rowsSet = false;

        var pWidth = grid.panelScale.x;

        rowInterval = pWidth / objsPerColumn;

        var j = 0;

        /* for (int iteration = 0; iteration < rowCount; iteration++)
         {*/
        for (int i = 0; i < objsPerColumn; i++)
        {
            // var socketByIteration = i + (iteration * objsPerRow);
           // Debug.Log("iteration number " + i + ", row " + currentRow);
            if (i != 0)
            {
                //sockets[i] = new Vector3((-pWidth / 2) + (socketInterval * (i+1)) /2, 0, 0);

                var origin = rows[0];
               // Debug.Log("modulo" + (i % 2));
                if (i % 2 == 0)
                {
                    //even
                    if (j > 0)
                    {
                        rows[i] = origin - rowInterval * (j + 1); ////sockets[4]
                    }
                    else
                    {
                        rows[i] = origin - rowInterval;//sockets[2]
                    }

                    j++;
                }
                else
                {
                    //odd
                    if (j > 0)
                    {
                        rows[i] = origin + rowInterval * (j + 1); //sockets[3,5]
                    }
                    else
                    {
                        rows[i] = origin + rowInterval; //sockets[1]
                    }
                }
            }
            else
            {
                rows[i] = 0;
            }

        }
        /* currentRow++;

     }*/

        if (objsPerColumn % 2 == 0) //if even, bump everything towards the middle
        {
            Debug.Log("bumba");
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i] = rows[i] - rowInterval / 2;
            }

            for (int i = 0; i < columns.Count; i++)
            {
                columns[i] = columns[i] - columnInterval / 2;
            }
        }

        rowsSet = true;
    }

    void ArrangeInteractables()  //shuffle interactables in the grid
    {
        Debug.Log("allonsy");
        var columnNum = 0;
        var rowNum = 0;
        for (int i = 0; i < objsPerColumn * columnCount; i++)
        {
            if (!extraObjs.Contains(collidables.objects[i]))
            {
                var r = collidables.GetRandomOrder(i);

                collidables.objects[r].transform.localPosition = new Vector3(rows[rowNum], columns[columnNum], 0.005f);

                RotateByType(collidables.objects[r]);

                if (i == (objsPerColumn * columnCount) - 1) //if last, put in previous
                {
                    previousPositions.Add(collidables.objects[i].transform.localPosition);
                }

                rowNum++;

                if (rowNum == objsPerColumn)
                {
                    rowNum = 0;
                    columnNum++;
                }

                collidables.objects[r].SetActive(true);

                
            }

            
        }

        if (extraObjs.Count > 0)
        {
            for (int i = 0; i < extraObjs.Count; i++)
            {
                ArrangeExtras(i);
            }
        }

        AssignOverlappingPairs();

        foreach (GameObject x in collidables.objects)
        {
            x.GetComponent<InteractableActivityManager>().proximityCollider.enabled = true;
        }
    }

    public void AssignOverlappingPairs()
    {
        for (int i = 0; i < collidables.objects.Count - extraObjs.Count; i++)
        {
            groupList.Add(new Group());
            groupList[i].groupedObjs.Add(collidables.objects[i]);
            
        }

        for (int i = 0; i < collidables.objects.Count - extraObjs.Count; i++)
        {

            List<GameObject> overlapping = extraObjs.FindAll(x => x.transform.localPosition == collidables.objects[i].transform.localPosition);

            foreach (GameObject obj in overlapping)
            {
                if (obj != null)
                {
                    groupList[i].groupedObjs.Add(obj);
                }
            }

          /*  Group existingInactive = objectGroups.Find(existing => existing.inactiveObject == overlapping);

            Debug.Log("existing inactive " + (existingInactive == null));
            if (overlapping != null && existingInactive == null)
            {

                groupList.Add(new Group(collidables.objects[i], overlapping));
            }
            else
            {
                if (i != collidables.objects.Count - 1)
                {
                    if (collidables.objects[i + 1].activeSelf == false)
                    {
                        Group existingAnyInactive = groupList.Find(existing => existing.inactiveObject == collidables.objects[i]);

                        Debug.Log("existing active " + (existingAnyInactive == null));
                        if (existingAnyInactive == null)
                        {
                            groupList.Add(new Group(collidables.objects[i], collidables.objects[i + 1]));
                        }
                    }
                }
            }*/
        }
    }

    public void ShowExtraButtons(GameObject activeInteractable)   
    {
        Group referenced = null;

        for (int i = 0; i < groupList.Count; i++)
        {
            if (groupList[i].groupedObjs.Contains(activeInteractable))
            {
                referenced = groupList[i];
                break;
            }
        }

        if (referenced != null) 
        {
            int index = referenced.groupedObjs.IndexOf(activeInteractable);

            if (index != referenced.groupedObjs.Count - 1)
            {
                activeInteractable.SetActive(false);
                referenced.groupedObjs[index + 1].SetActive(true);
            }            
        }
      /*  Group x = groupList.Find(y => y.activeObject == g);

        if (x != null && x.processed == false)
        {
            Debug.Log("processing " + x);
            x.activeObject.SetActive(false);

            x.inactiveObject.SetActive(true);

            x.processed = true;
        }
        else
        {
            Debug.Log(" xnull");
            /*var a = pairedObjects.Find(y => y.inactiveObject = g);
            Debug.Log(a.inactiveObject);
            if (a != null && a.processed == false)
            {
                Debug.Log("R���R��");
                a.activeObject.SetActive(false);

                a.inactiveObject.SetActive(true);

                a.processed = true;
            }
        }
    */
        /* if (extraObjs.Any() && rollover)  // all this fuckery for one extra button
         {
             for (int i = 0; i < extraObjs.Count; i++)
             {
                 var x = collidables.objects.Find(x => x.activeSelf && x.transform.localPosition == extraObjs[i].transform.localPosition);

                 if (x != null && x.GetComponent<InteractableActivityManager>().interactSuccess)
                 {
                     if (!extraObjs[i].activeSelf && x.GetComponent<InteractableActivityManager>().moveOn)
                     {
                         x.SetActive(false);
                         extraObjs[i].SetActive(true);

                         if (i == 0)
                         {
                             if (!extraObjs[i].GetComponent<InteractableActivityManager>().interactionEventStarted && collidables.GetInteractSuccessCount() >= collidables.objects.Count - extraObjs.Count)
                             {
                                 extraObjs[i].GetComponent<InteractableActivityManager>().rendererToChange.material = extraObjs[i].GetComponent<InteractableActivityManager>().highlightMaterial;
                                 StartCoroutine(extraObjs[i].GetComponent<InteractableActivityManager>().StartInteractionEvent());
                             }
                         }
                     }
                 }
             }
         }*/
    }

    public void ArrangeExtras(int index)
    {
        
        Debug.Log("last normal interactable index " + (objsPerColumn * columnCount + index));
        Vector3 previousPos = collidables.objects[(objsPerColumn * columnCount) + index].gameObject.transform.localPosition;

        if (previousPositions.Any())
        {
            previousPos = previousPositions[^1];
        }
        var randomRow = UnityEngine.Random.Range(0, rows.Count);
        var randomColumn = UnityEngine.Random.Range(0, columns.Count);

        Debug.Log(randomRow);
        Debug.Log(randomColumn);

        Vector3 extraPos = new Vector3(rows[randomRow], columns[randomColumn], 0.005f);

        Debug.Log(extraPos + " pos of extraobj " + index + ", previous pos was " + previousPos);

        if (extraPos != previousPos && previousPos != Vector3.zero)
        {
            extraObjs[index].transform.localPosition = extraPos;
            RotateByType(extraObjs[index]);
            previousPositions.Add(extraPos);
        }
        else if (previousPos == Vector3.zero)
        {
            Debug.Log("recursion since previous was extra, ei pit�s en�� tulla t�nne");
           // previousVectors.Add(previousPos);
            //ArrangeExtras(index, previousVectors);
        }
        else
        {
            Debug.Log("same position as previous, calling again");

           ArrangeExtras(index);
        }
    }

    public void RotateByType(GameObject g)
    {
        var type = g.GetComponent<InteractableActivityManager>().type;
        switch (type)
        {
            case InteractableActivityManager.InteractableType.Button:

                g.transform.eulerAngles = new Vector3(90f, g.transform.eulerAngles.y,
                   g.transform.eulerAngles.z);

                break;

            case InteractableActivityManager.InteractableType.Switch:

                g.transform.localEulerAngles = new Vector3(270, 180, 0);

                break;

        }
    }

    public void MatrixInteractionCheck()
    {
        if (collidables.objects.Count < 10 && !rollover)
        {
            interactionsGoal = collidables.objects.Count;    //asettaa gridin alle 10 obj kokoisen gridin interaction goalin obj m��r�ksi
        }
        else
        {
            interactionsGoal = 10;
        }

    }



    public bool IsSetDone()
    {
        if (collidables.objects.Any())
        {
            return collidables.GetInteractSuccessCount() == interactionsGoal;
        }
        else
        {
            return false;
        }
    }

    public bool AllocateRows()
    {
        return rowsSet;
    }

    public bool AllocateColumns()
    {
        return columnsSet;
    }

    public bool PassFaderStatus() // passes fader status from the fader script, interactables need to wait for this to start tracking time for the interaction event
    {
        return fader.FaderisFadedOut();
    }
}
