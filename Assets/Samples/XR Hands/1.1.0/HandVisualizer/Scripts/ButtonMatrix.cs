using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMatrix : MonoBehaviour
{
     CollidableObjects collidables;

     GridToPanel grid;

    private bool loopStatus;
    public IEnumerator ReadyForSetup()
    {
        collidables = GetComponent<CollidableObjects>();
        grid = GetComponent<GridToPanel>();

        Debug.Log(collidables.objects[0].transform.lossyScale);

        yield return new WaitUntil(LoopStatus);

        //panelScale.x to get width and expand rows if the size is too big

        //get center of gridActual and place interactables in a row
    }

    public bool LoopStatus()
    {
        return loopStatus;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
