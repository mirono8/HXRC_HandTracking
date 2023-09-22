using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;

public class CollidableObjects : MonoBehaviour
{

    public List<GameObject> objects = new();

    private void OnEnable()
    {
        if (!objects.Any())
            StartCoroutine(PopulateCollidables()); 
    }

    IEnumerator PopulateCollidables() // gathers all children (interactables) to track and starts first set
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            objects.Add(transform.GetChild(0).transform.GetChild(i).gameObject);

        }
        foreach (GameObject obj in objects)
        {
            Debug.Log("initial kinematic toggle on!!!!");
            obj.GetComponent<InteractableActivityManager>().ToggleKinematic(true);
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
}
         