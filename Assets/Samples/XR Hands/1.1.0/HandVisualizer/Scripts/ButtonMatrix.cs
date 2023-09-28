using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMatrix : MonoBehaviour
{
    CollidableObjects collidables;

    GridToPanel grid;

    //int socketCount = 5;

    int columnCount = 3; //t‰‰ setupdatast!!!!

    [SerializeField]
    int currentRow = 0;

    int objsPerColumn;

    [SerializeField]
    List<float> rows = new();

    [SerializeField]
    List<float> columns = new();

    bool rowsSet;

    bool columnsSet;

    float rowInterval;

    float columnInterval;
    public IEnumerator ReadyForSetup()
    {

        collidables = GetComponent<CollidableObjects>();
        grid = GetComponent<GridToPanel>();



        for (int i = 0; i < columnCount; i++)
        {
            columns.Add(0f);
        }

        objsPerColumn = collidables.objects.Count / columnCount;

        for (int i = 0; i < objsPerColumn; i++)
        {
            rows.Add(0f);
        }
        // Debug.Log(collidables.objects[0].transform.lossyScale);

        SetUpColumns();

        yield return new WaitUntil(AllocateColumns);

        SetUpRows();

        yield return new WaitUntil(AllocateRows);
        ArrangeInteractables();


        //SOCKET PLACEMENTS DONE, NEXT ROWS AND THEN BUTTON LOGIC
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
        rowsSet = true;
    }

    void ArrangeInteractables()
    {
        Debug.Log("allonsy");
        var columnNum = 0;
        var rowNum = 0;
        for (int i = 0; i < collidables.objects.Count; i++)
        {

            collidables.objects[i].transform.localPosition = new Vector3(rows[rowNum], columns[columnNum], 0);
            collidables.objects[i].transform.eulerAngles = new Vector3(-90f, collidables.objects[i].transform.eulerAngles.y,  
                    collidables.objects[i].transform.eulerAngles.z);

            rowNum++;
            if (rowNum == objsPerColumn - 1)
            {
                rowNum = 0;
                columnNum++;
            }
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

}
