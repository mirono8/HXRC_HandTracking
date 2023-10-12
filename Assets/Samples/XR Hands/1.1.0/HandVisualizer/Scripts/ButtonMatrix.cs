using HandData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMatrix : MonoBehaviour
{
    CollidableObjects collidables;

    GridToPanel grid;

    FadeIn fader;

    SetStart setData;
    //int socketCount = 5;

    int columnCount; //t‰‰ setupdatast!!!!   also cap matrix to 9x9!!!
 
    [SerializeField]
    int currentRow = 0;   //bugaa uudelleenk‰ytt‰ess‰ paneelia!

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

    [SerializeField]
    int interactionsGoal = 10;

    private void Start()
    {
        fader = GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeIn>();
        setData = GameObject.FindGameObjectWithTag("SessionManager").GetComponentInChildren<SetStart>();
    }
    public IEnumerator ReadyForSetup()
    {

        collidables = GetComponent<CollidableObjects>();
        grid = GetComponent<GridToPanel>();
        

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

        collidables.RandomizeOrderIndex();

        yield return new WaitUntil(collidables.RandomOrderReady);
        ArrangeInteractables();


        readyToTrack = true;
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
                Debug.Log("modulo" + (i % 2));
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
            Debug.Log("iteration number " + i + ", row " + currentRow);
            if (i != 0)
            {
                //sockets[i] = new Vector3((-pWidth / 2) + (socketInterval * (i+1)) /2, 0, 0);

                var origin = rows[0];
                Debug.Log("modulo" + (i % 2));
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
        for (int i = 0; i < collidables.objects.Count; i++)
        {
            var r = collidables.GetRandomOrder(i);

            collidables.objects[r].transform.localPosition = new Vector3(rows[rowNum], columns[columnNum], 0.005f);

            RotateByType(collidables.objects[r]);

            rowNum++;

            if (rowNum == objsPerColumn)
            {
                rowNum = 0;
                columnNum++;
            }

            collidables.objects[r].SetActive(true);
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
        if (collidables.objects.Count < 10)
        {
            interactionsGoal = collidables.objects.Count;    //t‰s kohtaa yks skipataa highlightaamisesta koska en tii‰, syy allaatonce interaciton checkis
        }
        else
        {
            interactionsGoal = 10;
        }

        interactions = 0;
        for (int i = 0; i < collidables.objects.Count; i++)
        {
            if (collidables.objects[i].GetComponent<InteractableActivityManager>().interactSuccess)
            {
                interactions++;
            }
        }
    }

    public bool IsSetDone()
    {

        return interactions == interactionsGoal;
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
        return fader.FaderStatus();
    }

    private void Update()
    {
        if (readyToTrack)
            MatrixInteractionCheck();
    }
}
