using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridToPanel : MonoBehaviour
{
    public GameObject panel;

    public Vector3 desiredDistance;

    PanelManager panelManager;

    private void Start()
    {
        panelManager = GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>();
        panel = panelManager.GiveMeAPanel();
        if(panel != null)
            gameObject.transform.parent = panel.transform;

        if (panel != null) //fix
            transform.localScale = new Vector3(1f * (transform.localScale.x - panel.transform.localScale.x),
                1f * (transform.localScale.y - panel.transform.localScale.y), 1f * (transform.localScale.z - panel.transform.localScale.z));
    }

    private void Update()
    {
        if (panel != null)
        {
            transform.position = panel.transform.position + desiredDistance;
            transform.localRotation = panel.transform.parent.localRotation;
            
           // panel.transform.localScale = transform.localScale;
        }
    }
}
