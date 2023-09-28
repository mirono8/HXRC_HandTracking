using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMatrix : MonoBehaviour
{
    CollidableObjects collidables;

    GridToPanel grid;

    int socketCount = 5;

    [SerializeField]
    List<Vector3> sockets = new();

    private bool socketsSet;

    float socketInterval;
    public IEnumerator ReadyForSetup()
    {

        collidables = GetComponent<CollidableObjects>();
        grid = GetComponent<GridToPanel>();
        


        for (int i = 0; i < socketCount; i++)
        {
            sockets.Add(new Vector3(0,0,0));
        }

        Debug.Log(collidables.objects[0].transform.lossyScale);

        SetupSocketPositions();

        ArrangeInteractables();

        yield return new WaitUntil(AllocateSockets);

        //panelScale.x to get width and expand rows if the size is too big

        //get center of gridActual and place interactables in a row
    }

    void SetupSocketPositions()
    {
        var pWidth = grid.panelScale.x;

        socketInterval = pWidth / socketCount;
        var j = 0;
        for (int i = 0; i< socketCount; i++)
        {
            
            if (i != 0)
            {
                //sockets[i] = new Vector3((-pWidth / 2) + (socketInterval * (i+1)) /2, 0, 0);

                var origin = sockets[i];
                Debug.Log("modulo" + (i % 2));
                if (i % 2 == 0)
                {
                    if (j > 0)
                    {
                        sockets[i] = new Vector3(origin.x - socketInterval * (j+1), 0, 0); ////sockets[4]
                    }
                    else
                    { 
                        sockets[i] = new Vector3(origin.x - socketInterval, 0, 0);//sockets[2]
                    }

                    j++;
                }
                else
                {
                    if (j > 0)
                    {
                        sockets[i] = new Vector3(origin.x + socketInterval * (j+1), 0, 0); //sockets[3,5]
                    }
                    else
                    {
                        sockets[i] = new Vector3(origin.x + socketInterval, 0, 0); //sockets[1]
                    }
                }
            }
            else
            {
                sockets[i] = new Vector3(0, 0, 0);
            }
            
        }

        Debug.Log(socketInterval);
        Debug.Log(-pWidth / 2);
    }

    void ArrangeInteractables()
    {
        for (int i = 0;i < socketCount; i++)
        {
            collidables.objects[i].transform.localPosition = sockets[i];
        }
    }

    public bool AllocateSockets()
    {
        return socketsSet;
    }

}
