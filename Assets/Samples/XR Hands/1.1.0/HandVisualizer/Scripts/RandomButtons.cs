using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomButtons : MonoBehaviour
{
    CollidableObjects collidables;

    public List<Transform> originalTransforms = new List<Transform>();

    public List<int> intersecting = new List<int>();

    void Start()
    {
        collidables = GetComponent<CollidableObjects>();

        foreach (GameObject x in collidables.objects) 
        {
            originalTransforms.Add(x.transform);
        }

        SetRandomPositions();

       LoopingIntersectSetter();

    }

    public void SetRandomPositions()
    {
        for (int i = 0; i < collidables.objects.Count; i++)
        {
            var deviation = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f));
            collidables.objects[i].transform.position = collidables.objects[i].transform.position + deviation;

            collidables.objects[i].SetActive(true);

            if (CheckBoundIntersection(i))
            {
                intersecting.Add(i);
            }
        }
    }

    public bool CheckBoundIntersection(int index)
    {
        for (int i = 0; i < collidables.objects.Count; i++)
        {
            if (i != index && collidables.objects[index].GetComponentInChildren<Collider>().bounds.Intersects(collidables.objects[i].GetComponentInChildren<Collider>().bounds))
            {
                /*var deviationAgain = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f));
                collidables.objects[index].transform.position = originalTransforms[index].position + deviationAgain;
                Debug.Log("colliders intersect");*/
                return true;
            }
        }
        return false;
    }

    public void LoopingIntersectSetter()
    {
        if (intersecting.Count > 0)
        {
            for (int i = 0; i < intersecting.Count; i++)
            {
               // SetIntersectingPositions(i);
            }
        }
    }

    public void SetIntersectingPositions(int index)
    {
            var deviationAgain = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f));
            collidables.objects[intersecting[index]].transform.position = originalTransforms[intersecting[index]].position + deviationAgain;

            
        /*    if (CheckBoundIntersection(intersecting[index]))
            {
                SetIntersectingPositions(index);
                Debug.Log("loop");
            }
            else
            {
                intersecting.Remove(intersecting[index]);
            
            }*/
    }
    
   //tää pitää saada looppaamaa niin kaua kunnes ei enää törmäile
    // Update is called once per frame
    void Update()
    {
        
    }
}
