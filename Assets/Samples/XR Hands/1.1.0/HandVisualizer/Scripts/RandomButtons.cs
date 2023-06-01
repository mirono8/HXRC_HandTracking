using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomButtons : MonoBehaviour
{
    public bool setAgain;
    CollidableObjects collidables;

    public List<Vector3> originalPositions = new List<Vector3>();

    public List<int> intersecting = new List<int>();

    public GameObject panel;

    public bool oneByOne;

    public void ReadyForSetup()
    {
        collidables = GetComponent<CollidableObjects>();

        foreach (GameObject x in collidables.objects) 
        {
            originalPositions.Add(x.transform.position);
        }

        panel = GetComponent<GridToPanel>().panel;

        SetRandomPositions();

        if (!oneByOne)
        {
            StartCoroutine(LoopingIntersectSetter());
        }


    }

    private void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Space Pressed");
            for (int i = 0; i < collidables.objects.Count; i++)
            {
                CheckBoundIntersection(i);
            }
        }*/
    }

    public void SetRandomPositions()
    {
        for (int i = 0; i < collidables.objects.Count; i++)
        {
            // var deviation = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.2f, 0.2f));
            var deviationInPanel = new Vector3(Random.Range(panel.transform.localScale.x * -0.4f, panel.transform.localScale.x * 0.4f),
                Random.Range(panel.transform.localScale.y * -0.4f, panel.transform.localScale.y * 0.4f));
            collidables.objects[i].transform.position = collidables.objects[i].transform.position + deviationInPanel;

            if (!oneByOne)
            {
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
            if (i != index && collidables.objects[index].GetComponentInChildren<Collider>().bounds.Intersects(collidables.objects[i].GetComponentInChildren<Collider>().bounds))
            {
                if (!intersecting.Contains(index))
                {
                    intersecting.Add(index);
                }
                return true;
            }
        }
        intersecting.Remove(index);
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
           
        }
    }


    public void SetIntersectingPositions(int index)
    {
        Debug.Log("intersecting set");
        var deviationAgain = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f));
        collidables.objects[intersecting[index]].transform.position = originalPositions[intersecting[index]]+ deviationAgain;
    }

}
