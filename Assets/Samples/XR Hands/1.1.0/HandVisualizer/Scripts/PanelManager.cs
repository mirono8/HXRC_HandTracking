using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    
    public List<Panel> panels;

    public Vector3 panelScale = new Vector3(0.846153855f, 0.846153855f, 0.0252450015f);

    [Serializable]
    public class Panel
    {
        [SerializeField]
        public bool occupado = false;
        [SerializeField]
        public bool reusePanel;
        [SerializeField]
        public GameObject panel;

        public Panel(GameObject panel)
        {
            this.panel = panel;
        }
    }

    public void FindAllPanels()
    {
        var arr = GameObject.FindGameObjectsWithTag("Panel");
        for (int i = 0; i < arr.Length; i++)
        {
            panels.Add(new Panel(arr[i]));
        }
    }

    public GameObject GiveMeAPanel()
    {
        var x = panels.Find(x => !x.occupado);
        if (x != null)
        {
            x.occupado = true;
            return x.panel;
        }
        else { return null; }

    }

    public GameObject ReusePanel()
    {
        var x = panels.Find(x => x.panel == x.reusePanel);
        if (x != null)
        {
            x.occupado = false;
            return x.panel;
        }
        return null;
    }

    public Transform GetPanelTransform(GameObject panel)
    {
        var x = panels.Find(x => x.panel == panel);
        if (x != null)
        {
            return panel.transform.GetChild(0);
        }
        else
            return null;
    }

    public Vector3 GetPanelRotation(GameObject panel)
    {
        var x = panels.Find(x => x.panel == panel);

        if(x != null)
        {
            return panel.transform.GetChild(0).transform.eulerAngles;
        }
        else
            return Vector3.zero;
    }
}

