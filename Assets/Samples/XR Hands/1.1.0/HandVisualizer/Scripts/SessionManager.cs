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

    IEnumerator GetInitialState()
    {
        yield return new WaitUntil(SessionActiveStatus);

        setCount = setStart.setupData.sets.Count;

        if (setCount != 0)
            for (int i = 0; i < setCount - 1; i++) { setStart.setGrid.Add(Instantiate(setStart.gridPrefab, setStart.gameObject.transform)); }
        else
            Debug.Log("no sets?");//setStart.setGrid.Add(Instantiate(setStart.gridPrefab, setStart.gameObject.transform));

        panelManager.FindAllPanels();

        panelManager.ToggleHighlighting(panelManager.panels[currentSet].panel);

        setStart.SetupInteractables();
    }

    public void TryStartNextSet()
    {
        currentSet++;
        if (currentSet < setCount)
        {
            panelManager.ToggleHighlighting(panelManager.panels[currentSet].panel);
            //currentSet++;
            if (panelManager.panels.Count <= currentSet)
            {
                setStart.AssignSetParams(currentSet, true);
            }
            else
                setStart.AssignSetParams(currentSet);

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
        if (setStart.currentSetGameObjs.Any() && setStart.currentSetGameObjs.Last().GetComponent<InteractableActivityManager>().interactSuccess && !allClear)
            TryStartNextSet();
    }
}
