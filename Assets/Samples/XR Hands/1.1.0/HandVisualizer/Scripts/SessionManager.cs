using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SessionManager : MonoBehaviour
{

    SessionStart sessionStart;

    [SerializeField]
    int setCount;
    [SerializeField]
    int currentSet = 0;

    public bool allClear = false;

    PanelManager panelManager;

    private void Awake()
    {
        sessionStart = GetComponentInChildren<SessionStart>(true);
        panelManager = GetComponentInChildren<PanelManager>(true);
    }

    private void Start()
    {
        StartCoroutine(GetInitialState());
    }

    IEnumerator GetInitialState()
    {
        yield return new WaitUntil(SessionActiveStatus);

        setCount = sessionStart.setupData.sets.Count;

        for (int i = 0; i < setCount - 1; i++) { Instantiate(sessionStart.gridPrefab, sessionStart.gameObject.transform); }

        panelManager.FindAllPanels();
    }

    public void TryStartNextSet()
    { 
        if(currentSet < setCount)
        {
            currentSet++;
            sessionStart.AssignSetParams(currentSet);
            sessionStart.RunSet();
        }
        else
            allClear = true;
    }

    bool SessionActiveStatus()
    {
        return sessionStart.gameObject.activeSelf;
    }
}
