using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class CollidableObjects : MonoBehaviour
{

    public List<GameObject> objects = new();

    private void OnEnable()
    {
        StartCoroutine(PopulateCollidables());
    }

    IEnumerator PopulateCollidables()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < transform.childCount; i++)
        {
            objects.Add(transform.GetChild(i).gameObject);
        }

        gameObject.GetComponent<RandomButtons>().ReadyForSetup();
    }
}
