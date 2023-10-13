using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponent : MonoBehaviour
{
    [SerializeField]
    FadeIn fadeIn;

    Vector3 savePos;

    [SerializeField]
    GameObject UIObj;

    private void OnEnable()
    {
        savePos = GameObject.FindGameObjectWithTag("Player").transform.position;
        //tähän joku fixed käden mitta, käytä touchpad prefab

        transform.position = savePos + new Vector3(0, savePos.y * -1, 0.353f);

        transform.LookAt(savePos);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FingerCollider"))
        {
            StartCoroutine(fadeIn.FadeCanvasOut());

            UIObj.SetActive(false);
        }
    }

}
