using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridToPanel : MonoBehaviour
{
    public GameObject panel;

    public Vector3 desiredDistance;

    PanelManager panelManager;

    public Vector3 panelScale = new Vector3(0.846153855f, 0.846153855f, 0.0252450015f);

    [SerializeField]
    Vector3 angle;

    GameObject gridActual;

    private void Start()
    {
        panelManager = GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>();
        panel = panelManager.GiveMeAPanel();

        gridActual = transform.GetChild(0).gameObject;

        if (panel != null)
        {

            transform.parent = panel.transform;

           // Vector3 withScaleOffset = new Vector3(panelVector.x * panelScale.x, panelVector.y * panelScale.y, panelVector.z * panelScale.z);
            //transform.GetChild(0).position = panelManager.GetPanelTransform(panel).position + desiredDistance;
           
        }
    }

    private void Update()
    {
        if (panel != null)
        {
            gridActual.transform.localPosition = panelManager.GetPanelTransform(panel).localPosition + desiredDistance;
            gridActual.transform.eulerAngles = panelManager.GetPanelRotation(panel);
        }
    }
}
 