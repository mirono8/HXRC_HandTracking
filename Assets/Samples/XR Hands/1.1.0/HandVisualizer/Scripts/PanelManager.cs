using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    
    public List<Panel> panels;

    public Vector3 panelScale = new Vector3(0.846153855f, 0.846153855f, 0.0252450015f);

    [SerializeField]
    bool highlight;

    float t;

    Color original;
    Color variant;
    GameObject panelToHighlight;

    [SerializeField]
    GameObject latestPanel;

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

        if (x != null && x.panel != latestPanel)
        {
            x.occupado = true;
          //
            /* if (latestPanel != null)
             {
                 var y = panels.Find(y => y.panel == latestPanel);
                 Debug.Log(y + "is freed");
                 y.occupado = false;
             }*/
            FreeNextPanel(x.panel);
            return x.panel;
        }
        else 
        {
            Debug.Log("fail!!!!!!!!");
            return null;
        }
    }

    public void FreeNextPanel(GameObject current)
    {
        //tee vaa listast seuraava lol
        var x = panels.Find(x => x.panel == current);
        var i = panels.IndexOf(x);

        if (i != panels.Count - 1)
        {
            panels[i+1].occupado = false;
        }
        else
        {
            panels[0].occupado = false;
        }
    }

    public GameObject ReusePanel()  //rendered useless with freethispanel? test
    {
        var x = panels.Find(x => x.panel == !x.reusePanel);
        if (x != null)
        {
            x.occupado = false;
            x.reusePanel = true;
            return x.panel;
        }
        else
        {
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].reusePanel = false;
                ReusePanel();
            }
        }
        return null;
    }

    public void FreeThisPanel(GameObject g)
    {
        var x = panels.Find(x => x.panel == g);

        if (x != null)
        {
            Debug.Log("freerf");
            x.occupado = false;
        }
    }

    public void LastPanelUsed(GameObject g)
    {

        latestPanel = g;

    }

    public GameObject LatestPanel()
    {
        return latestPanel;
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

    public void HighlightPanel()
    {

        panelToHighlight.GetComponent<MeshRenderer>().material.color = Color.Lerp(original, variant ,Mathf.PingPong(Time.time, 1));
    }

    public void ToggleHighlighting(GameObject panel)
    {
        if (!highlight)
        {
            highlight = true;
            t = 0;
            panelToHighlight = panel.transform.GetChild(0).gameObject;
            original = panelToHighlight.GetComponent<MeshRenderer>().material.color;
            variant = new Color(original.r, original.g, original.b + 0.1f);
        }
        
    }
    private void FixedUpdate()
    {
        if (highlight)
        {
            t += Time.deltaTime;

            if (t < 5)
                HighlightPanel();
            else
            {
                panelToHighlight.GetComponent<MeshRenderer>().material.color = original;
                panelToHighlight = null;
                highlight = false;
            }
        }
    }

}

