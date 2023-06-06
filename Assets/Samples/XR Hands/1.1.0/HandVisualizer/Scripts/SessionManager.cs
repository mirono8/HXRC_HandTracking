using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SessionManager : MonoBehaviour
{

    SetStart setStart;

    [SerializeField]
    int setCount;
    [SerializeField]
    int currentSet;

    public bool allClear = false;

    PanelManager panelManager;

    
    private void Awake()
    {
        setStart = GetComponentInChildren<SetStart>(true);
        panelManager = GetComponentInChildren<PanelManager>(true);
    }

    private void Start()
    {
       
        StartCoroutine(GetInitialState());
    }

    IEnumerator GetInitialState()
    {
        yield return new WaitUntil(SessionActiveStatus);

        setCount = setStart.setupData.sets.Count;

        for (int i = 0; i < setCount - 1; i++) { setStart.setGrid.Add(Instantiate(setStart.gridPrefab, setStart.gameObject.transform)); }

        panelManager.FindAllPanels();

        setStart.SetupInteractables();
    }

    public void TryStartNextSet()
    { 
        if(currentSet < setCount)
        {
            currentSet++;
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
            allClear = true; setStart.gameObject.SetActive(false);
    }

    bool SessionActiveStatus()
    {
        return setStart.gameObject.activeSelf;
    }
}
