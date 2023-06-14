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
    public void ReadyForSetup()
    {
        collidables = GetComponent<CollidableObjects>();

        foreach (GameObject x in collidables.objects) 
        {
            originalPositions.Add(x.transform.localPosition);
        }

        panelScale = GetComponent<GridToPanel>().panelScale;
        panelManager = GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>();
        
        SetRandomPositions();

        if (!oneByOne)
        {
            StartCoroutine(LoopingIntersectSetter());
        }
    }

    public void SetRandomPositions()
    {
        for (int i = 0; i < collidables.objects.Count; i++)
        {
            // var deviation = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.2f, 0.2f));
            var deviationInPanel = new Vector3(UnityEngine.Random.Range(panelScale.x * -0.4f, panelScale.x * 0.4f),
                UnityEngine.Random.Range(panelScale.y * -0.4f, panelScale.y * 0.4f));

            collidables.objects[i].transform.localPosition = new Vector3(collidables.objects[i].transform.localPosition.x + deviationInPanel.x,
                collidables.objects[i].transform.localPosition.y + deviationInPanel.y, 0f);



            if (!oneByOne)
             {
                 Debug.Log("setting" + i + " indexed active");
                 collidables.objects[i].SetActive(true);

                 CheckBoundIntersection(i);
             }
             else if (i == 0)
                 collidables.objects[i].SetActive(true);
        }
    }

    public bool CheckBoundIntersection(int index)
    {
        for (int i = 0; i < collidables.objects.Count; i++)
        {
            if (i != index && collidables.objects[index].GetComponent<InteractableActivityManager>().intersectionCollider.bounds.Intersects(collidables.objects[i].GetComponent<InteractableActivityManager>().intersectionCollider.bounds)) // i != index && collidables.objects[index].GetComponentInChildren<Collider>().bounds.Intersects(collidables.objects[i].GetComponentInChildren<Collider>().bounds))
            {
                if (!intersecting.Contains(index))
                {
                    intersecting.Add(index);
                }
                return true;
            }
        }
        intersecting.Remove(index);

        collidables.objects[index].transform.localPosition = new Vector3(collidables.objects[index].transform.localPosition.x,
                collidables.objects[index].transform.localPosition.y, 0f);

        return false;
    }

    public IEnumerator LoopingIntersectSetter()
    {

        while (intersecting.Count > 0)
        {
            yield return null;
            if (CheckBoundIntersection(intersecting[0]))
                SetIntersectingPositions(0);
        }
        
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < collidables.objects.Count; i++)
        {
            CheckBoundIntersection(i);
            
        }
        yield return null;
        if (intersecting.Count > 0)
        {
            StartCoroutine(LoopingIntersectSetter());
        }
        else
        {
            Debug.Log("loop end");
           for (int i = 0;i < collidables.objects.Count; i++)
            {
                collidables.objects[i].transform.localPosition = new Vector3(collidables.objects[i].transform.localPosition.x,
                collidables.objects[i].transform.localPosition.y, 0f);

                collidables.objects[i].transform.eulerAngles = new Vector3(-90f, collidables.objects[i].transform.eulerAngles.y,
                    collidables.objects[i].transform.eulerAngles.z);// panelManager.GetPanelBackward(GetComponent<GridToPanel>().SendPanel());


            }
        }
    }

    public void SetIntersectingPositions(int index)
    {
        Debug.Log("intersecting set");
        var deviationAgain = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.2f, 0.2f));
        collidables.objects[intersecting[index]].transform.localPosition = originalPositions[intersecting[index]]+ deviationAgain;

        collidables.objects[index].transform.localPosition = new Vector3(collidables.objects[index].transform.localPosition.x,
                collidables.objects[index].transform.localPosition.y, 0f);


    }
    //intersectaa viel v‰lil joskus lul

    public bool IsOneByOne()
    {
        return oneByOne;
    }

    void DoubleCheck()
    {
        Debug.Log("double check");
        var j = 0;
        for (int i = 0; i < collidables.objects.Count; i++)
        {
            if (i != j && collidables.objects[j].GetComponent<InteractableActivityManager>().intersectionCollider.bounds.Intersects(collidables.objects[i].
                GetComponent<InteractableActivityManager>().intersectionCollider.bounds)) // i != index && collidables.objects[index].GetComponentInChildren<Collider>().bounds.Intersects(collidables.objects[i].GetComponentInChildren<Collider>().bounds))
            {
                Debug.Log("m‰‰ly");
                if (!intersecting.Contains(j))
                {
                    intersecting.Add(j);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("FunnyTestKey"))
            DoubleCheck();
    }
}
