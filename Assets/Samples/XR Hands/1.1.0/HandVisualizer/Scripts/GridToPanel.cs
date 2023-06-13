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
    private void Start()
    {
        panelManager = GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>();
        panel = panelManager.GiveMeAPanel();


        if (panel != null)
        {

            transform.parent = panel.transform;
            var panelVector = panelManager.GetPanelTransform(panel).position;
           // Vector3 withScaleOffset = new Vector3(panelVector.x * panelScale.x, panelVector.y * panelScale.y, panelVector.z * panelScale.z);
            //transform.GetChild(0).position = panelManager.GetPanelTransform(panel).position + desiredDistance;
            transform.GetChild(0).localPosition = panelManager.GetPanelTransform(panel).localPosition + desiredDistance;
            transform.GetChild(0).eulerAngles = panelManager.GetPanelRotation(panel);
        }
    }

    public Vector3 CalculateButtonOffset(Vector3 vector)
    {
        return vector;
    }

    public GameObject SendPanel()
    {
        return transform.GetChild(0).gameObject;
    }
}
 