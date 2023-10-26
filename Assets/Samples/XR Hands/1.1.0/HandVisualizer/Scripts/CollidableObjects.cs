using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;

public class CollidableObjects : MonoBehaviour
{
    public List<GameObject> objects = new();

    [SerializeField]
    List<int> randomOrder = new List<int>();

    [SerializeField]
    List<int> interactedObjs = new List<int>();

    bool orderRandomized;

    [SerializeField]
    int interactSuccessCount = 0;
    private void OnEnable()
    {
        /*  if (!objects.Any())
              StartCoroutine(PopulateCollidables()); */

        
    }

    public int GetInteractSuccessCount()
    {
        return interactSuccessCount;
    }

    public void AddToCounter()
    {
        interactSuccessCount++;
    }
    public IEnumerator PopulateCollidables() // gathers all children (interactables) to track and starts first set
    {
        yield return new WaitForSeconds(1);
     /*   for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            objects.Add(transform.GetChild(0).transform.GetChild(i).gameObject);

        }*/
        foreach (GameObject obj in objects)
        {
            obj.GetComponent<InteractableActivityManager>().ToggleKinematic(true);

            obj.GetComponent<InteractableActivityManager>().GetMyColliders();


        }

        if (gameObject.GetComponent<RandomButtons>())
        {
            StartCoroutine(gameObject.GetComponent<RandomButtons>().ReadyForSetup());
        }
        else if (gameObject.GetComponent<ButtonMatrix>())
        {
            //matrix resurrection stuff!
            StartCoroutine(gameObject.GetComponent<ButtonMatrix>().ReadyForSetup());
        }
    }

    public GameObject GetNearestNeighbor(int i) // finds the nearest interactable, so as to not make it the next highlighted object in other scripts
    {
        Transform nearest = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = objects[i].transform.position;

        foreach (GameObject obj in objects)
        {
            if (obj != objects[i])
            {
                Vector3 directionToTarget = obj.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    nearest = obj.transform;
                }
            }
        }
        Debug.Log("my (" + i +") nearest was " + nearest.gameObject.GetComponent<InteractableActivityManager>().myOrderIndex);

        return nearest.gameObject;
    }

    public void AddToInteracted(int i)
    {
        if(!interactedObjs.Contains(i))
            interactedObjs.Add(i);
    }

    public GameObject PickNextUnusedInteractable(int i)
    {
        for (int objIndex = 0; objIndex < objects.Count; objIndex++)
        {
            if (objIndex != i)
            {
                if (!interactedObjs.Contains(objIndex))
                {
                    Debug.Log("skipped order, picked index of " + objIndex);
                    return objects[objIndex];
                }
            }
        }
        return null;
    }
    public void ToggleColliders(bool b)
    {

       // Debug.Log("toggling interactable colliders as " + b);
        for (int i = 0; i < objects.Count; i++)
        {
            var interactable = objects[i].GetComponent<InteractableActivityManager>();

            for (int j = 0; j < interactable.colliders.Count; j++)
            {
                interactable.colliders[j].isTrigger = b;
            }
        }
    }


    public void RandomizeOrderIndex()  
    {
        for (int i = 0; i < objects.Count; i++)
        {
            randomOrder.Add(i);
        }

        for (int i = randomOrder.Count-1; i >= 0; i--)
        {
            var r = Random.Range(0, i);
            var temp = randomOrder[r];
            randomOrder[r] = randomOrder[i];
            randomOrder[i] = temp;
            
        }
        Debug.Log("order randomized");
        RandomOrderSet(true);

    }



    public void ResetRandomOrder()
    {
        randomOrder.Clear();
        RandomOrderSet(false);
    }
    public void RandomOrderSet(bool b)
    {
        orderRandomized = b;
    }

    public bool RandomOrderReady()
    {
        return orderRandomized;
    }

    public int GetRandomOrder(int i)
    {
        return randomOrder[i];
    }

}
         