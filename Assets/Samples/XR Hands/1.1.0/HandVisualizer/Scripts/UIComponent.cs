using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponent : MonoBehaviour
{
    [SerializeField]
    FadeIn fadeIn;

    [SerializeField]
    SessionManager sessionManager;

    Vector3 savePos;

    [SerializeField]
    GameObject canvas;

    [SerializeField]
    GameObject UIParent;

    public Vector3 offset;

    float offsetX;
    float offsetY;
    float offsetZ;

    private void OnEnable()
    {

        // savePos = GameObject.FindGameObjectWithTag("Player").transform.position;
        //tähän joku fixed käden mitta, käytä touchpad prefab, parempi positio!
        // offset = new Vector3(0, savePos.y * -1, 0.353f);

        savePos = sessionManager.GetUIDefaultTranform().position;
       // savePos =  new Vector3 (6f, 3f, 2f);
       // transform.position = savePos + offset;

        transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform);
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

    public void ResetComponents()
    {
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        canvas.SetActive(true);

        UIParent.SetActive(false);
    }

    private void Update()
    {
        transform.position = sessionManager.GetUIDefaultTranform().position;

        offsetX = savePos.x;
        offsetY = savePos.y;
       // offsetZ = GameObject.FindGameObjectWithTag("Player").transform.forward ;

       // transform.localPosition = new Vector3(offsetX,offsetY *-1 +0.1f, GameObject.FindGameObjectWithTag("Player").transform.forward.z / 2);
        

        transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform);
    }
}
