using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponent : MonoBehaviour
{
    [SerializeField]
    FadeIn fadeIn;

    Vector3 savePos;

    [SerializeField]
    GameObject canvas;

    private void OnEnable()
    {
        savePos = GameObject.FindGameObjectWithTag("Player").transform.position;
        //t�h�n joku fixed k�den mitta, k�yt� touchpad prefab, parempi positio!

        transform.position = savePos + new Vector3(0, savePos.y * -1, 0.353f);

        transform.LookAt(savePos);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FingerCollider"))
        {
            StartCoroutine(fadeIn.FadeCanvasOut());

            //UIObj.SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            canvas.SetActive(false);
        }
    }

}
