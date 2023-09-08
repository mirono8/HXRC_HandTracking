using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SessionManager : MonoBehaviour
{

    public SetStart setStart;

    [SerializeField]
    int setCount;
    [SerializeField]
    int currentSet;

    public bool allClear = false;

    PanelManager panelManager;

    FadeIn fader;
    private void Awake()
    {
        setStart = GetComponentInChildren<SetStart>(true);
        panelManager = GetComponentInChildren<PanelManager>(true);
    }

    private void Start()
    {
        fader = GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeIn>();
        StartCoroutine(GetInitialState());
    }

    IEnumerator GetInitialState() // loads intial state of the session from the scene
    {
        yield return new WaitUntil(SessionActiveStatus);

        setCount = setStart.setupData.sets.Count;

        if (setCount != 0)
        {
            for (int i = 0; i < setCount - 1; i++)
            {
                setStart.setGrid.Add(Instantiate(setStart.gridPrefab, setStart.gameObject.transform));
                
            }

            
        }
        else
        { 
        Debug.Log("no sets?");//setStart.setGrid.Add(Instantiate(setStart.gridPrefab, setStart.gameObject.transform));
        }

        panelManager.FindAllPanels();

        setStart.GetCurrentSetNumber(currentSet);

        for (int i = 0; i < setStart.setGrid.Count; i++)
        {
            setStart.warpPoints.Add(setStart.setGrid[i].GetComponent<GridToPanel>().warpPoint);
        }

        panelManager.ToggleHighlighting(panelManager.panels[currentSet].panel);

        setStart.SetupInteractables();
    }

    public void TryStartNextSet() // tries to start the next set if there are multiple
    {
        currentSet++;
        if (currentSet < setCount)
        {
            panelManager.ToggleHighlighting(panelManager.panels[currentSet].panel);

            //currentSet++;
            if (panelManager.panels.Count <= currentSet)
            {
                setStart.AssignSetParams(currentSet, true);
                setStart.GetCurrentSetNumber(currentSet);
            }
            else
            {
                setStart.AssignSetParams(currentSet);
                setStart.GetCurrentSetNumber(currentSet);
            }
            setStart.ClearCurrentSet();
            setStart.SetupInteractables();
            setStart.GameObjectsToTrack();
            setStart.RunSet();
        }
        else
        {
            allClear = true;

            foreach (PanelManager.Panel p in panelManager.panels)
            {
                if (p.panel.GetComponentInChildren<GridToPanel>())
                {
                    p.panel.GetComponentInChildren<GridToPanel>().gameObject.SetActive(false);
                    
                }
            }
        }
    }

    bool SessionActiveStatus()
    {
        return setStart.gameObject.activeSelf;
    }

    private void Update()
    {
        if (setStart.currentSetGameObjs.Any() && setStart.currentSetGameObjs.Last().GetComponent<InteractableActivityManager>().interactSuccess && !allClear) // starts next set, there's probably a better way to do this :)
            TryStartNextSet();
    }
}
