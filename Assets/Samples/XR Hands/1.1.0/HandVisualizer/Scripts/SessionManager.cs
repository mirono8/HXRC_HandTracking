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
    int currentSet;

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

        for (int i = 0; i < setCount - 1; i++) { sessionStart.setGrid.Add(Instantiate(sessionStart.gridPrefab, sessionStart.gameObject.transform)); }

        panelManager.FindAllPanels();

        sessionStart.RunSet();
    }

    public void TryStartNextSet()
    { 
        if(currentSet < setCount)
        {
            currentSet++;
            if (panelManager.panels.Count <= currentSet)
            {
                sessionStart.AssignSetParams(currentSet, true);
            }
            else
                sessionStart.AssignSetParams(currentSet);

            sessionStart.ClearListsForReuse();
            sessionStart.RunSet();
            sessionStart.PopulateCollidables();
        }
        else
            allClear = true;
    }

    bool SessionActiveStatus()
    {
        return sessionStart.gameObject.activeSelf;
    }
}
