using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    

    public List<Panel> panels;

    [Serializable]
    public class Panel
    {
        [SerializeField]
        public bool occupado = false;
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
}

