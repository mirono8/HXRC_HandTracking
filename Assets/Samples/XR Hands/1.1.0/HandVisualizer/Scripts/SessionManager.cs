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
    int currentSet = 0;

    public bool allClear = false;

    PanelManager panelManager;

    [SerializeField]
    int roundsToPause;   

    private void Awake()
    {
        setStart = GetComponentInChildren<SetStart>(true);
        panelManager = GetComponentInChildren<PanelManager>(true);
    }

    private void Start()
    {
        StartCoroutine(GetInitialState());
    }

    IEnumerator GetInitialState() // loads intial state of the session from the scene
    {
        yield return new WaitUntil(SessionActiveStatus);

        setCount = setStart.setupData.sets.Count;

        panelManager.FindAllPanels();

        if (setCount != 0)
        {
            for (int i = 0; i < setCount - 1; i++)
            {
                setStart.setGrid.Add(Instantiate(setStart.gridPrefab, setStart.gameObject.transform));
                setStart.setGrid[i].SetActive(false);

                if (i < panelManager.panels.Count)
                {
                    setStart.setGrid[i].SetActive(true);
                }
            }
            setStart.setGrid[setCount-1].SetActive(false);

        }
        else
        { 
            Debug.Log("no sets?");//setStart.setGrid.Add(Instantiate(setStart.gridPrefab, setStart.gameObject.transform));
        }

        if (setStart.CurrentRound() == roundsToPause)
        {
            setStart.automaticFade = false;
        }
        else
        {
            setStart.automaticFade = true;
        }

        setStart.GetCurrentSetNumber(currentSet);

        for (int i = 0; i < setStart.setGrid.Count; i++)
        {
            setStart.warpPoints.Add(setStart.setGrid[i].GetComponent<GridToPanel>().warpPoint);
        }

        panelManager.ToggleHighlighting(panelManager.panels[currentSet].panel);

        setStart.SetupInteractables();

        setStart.GameObjectsToTrack();
    }

    public void TryStartNextSet() // tries to start the next set if there are multiple
    {
        currentSet++;

        if (currentSet < setCount)
        {

            setStart.ClearComponents();

            //currentSet++;
            if (currentSet < panelManager.panels.Count )
            {
                panelManager.ToggleHighlighting(panelManager.panels[currentSet].panel);
                setStart.AssignSetParams(currentSet);
                setStart.GetCurrentSetNumber(currentSet);
            }
            else
            {
               // panelManager.ToggleHighlighting(panelManager.ReusePanel());       //use freethispanel!
                setStart.AssignSetParams(currentSet, true);
                setStart.GetCurrentSetNumber(currentSet);
            }

            if (setStart.CurrentRound() == roundsToPause)
            {
                setStart.automaticFade = false;
            }
            else
            {
                setStart.automaticFade = true;
            }

            setStart.ClearCurrentSet();
            setStart.SetupInteractables();
            setStart.GameObjectsToTrack();
            StartCoroutine(setStart.RunSet());
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
        if (setStart.CurrentSessionMode() != "matrix")
        {
            if (setStart.currentSetGameObjs.Any() && setStart.currentSetGameObjs.Last().GetComponent<InteractableActivityManager>().interactSuccess && !allClear) // starts next set, there's probably a better way to do this :)
                TryStartNextSet();
        }
        else
        {
            
            if (setStart.setGrid[currentSet].GetComponent<ButtonMatrix>())
            {
                if (setStart.setGrid[currentSet].GetComponent<ButtonMatrix>().IsSetDone())
                {
                    TryStartNextSet();
                }
            }
        }
    }
}
