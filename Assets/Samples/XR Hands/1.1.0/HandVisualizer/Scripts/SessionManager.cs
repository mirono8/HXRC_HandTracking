using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SessionManager : States
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

    bool firstSet = true;

    FadeIn fader;

    [SerializeField]
    Transform UIDefault;

    float sessionEndTime;

    private void Awake()
    {
        setStart = GetComponentInChildren<SetStart>(true);
        panelManager = GetComponentInChildren<PanelManager>(true);
    }

    private void Start()
    {
        StartCoroutine(GetInitialState());

        fader = GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeIn>();

        
    }

    IEnumerator GetInitialState() // loads intial state of the session from the scene
    {
        yield return new WaitUntil(SessionActiveStatus);

        setCount = setStart.setupData.sets.Count;
        InjectSetCount();

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

        if (setStart.CurrentRound() == roundsToPause || firstSet)
        {
            setStart.automaticFade = false;
            setStart.ResetRoundCounter();
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

       

        firstSet = false;
    }

    public void TryStartNextSet() // tries to start the next set if there are multiple
    {
        
        currentSet++;
        InjectCurrentSet();

        ChangeState(State.Paused);

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
                setStart.ResetRoundCounter();
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

            ChangeState(State.End);

            foreach (PanelManager.Panel p in panelManager.panels)
            {
                if (p.panel.GetComponentInChildren<GridToPanel>())
                {
                    p.panel.GetComponentInChildren<GridToPanel>().gameObject.SetActive(false);
                    
                }
            }
            fader.SessionEnded(sessionEndTime);  //build and test
        }
    }

    public void SetSessionEndTime(float f)
    {
        sessionEndTime = f;
    }

    public Transform GetUIDefaultTranform()
    {
        return UIDefault;
    }

    bool SessionActiveStatus()
    {
        return setStart.gameObject.activeSelf;
    }

    public bool FaderStatus()
    {
        return fader.FaderStatus();
    }

    public void InjectSetCount()
    {
        fader.SetSetCount(setCount);
    }

    public void InjectCurrentSet()
    {
        fader.SetCurrentSet(currentSet);
    }
    private void Update()
    {
        if (fader.FaderStatus())
        {
            ChangeState(State.Active);
        }
        else
        {
            ChangeState(State.Paused);
        }

        if (setStart.CurrentSessionMode() == "onebyone")
        {
            // starts next set, there's probably a better way to do this :)
            if (setStart.currentSetGameObjs.Any() && setStart.currentSetGameObjs.Last().GetComponent<InteractableActivityManager>().interactSuccess && !allClear)
                TryStartNextSet();
        }
        else if (setStart.CurrentSessionMode() == "all")
        {
            if (setStart.currentSetGameObjs.Any() && setStart.currentSetGameObjs.FirstOrDefault().GetComponent<InteractableActivityManager>() != null && setStart.collidablesReady)
            {
                if (setStart.currentSetGameObjs.Any() && setStart.currentGrid.GetComponent<CollidableObjects>() != null)  //collidables muka null mut eipä oo
                {
                    if (setStart.currentGrid.GetComponent<CollidableObjects>().GetInteractSuccessCount()
                        == setStart.currentGrid.GetComponent<CollidableObjects>().objects.Count && !allClear)   //setStart.currentSetGameObjs.FirstOrDefault().GetComponent<InteractableActivityManager>().collidables.GetInteractSuccessCount()
                    {
                        TryStartNextSet();
                    }
                }
            }
        }
        else
        {
            if (!firstSet)  
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
}
