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
    }

    private void Update()
    {
        if (panel != null)
            transform.position = panel.transform.position + desiredDistance;
    }
}
