using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandData;
public class InteractableEventTracker : MonoBehaviour
{
    HandDataOut handData;
    SaveManager saveManager;

    
    private void Start()
    {
        handData = GameObject.FindGameObjectWithTag("HandData").GetComponent<HandDataOut>();
        saveManager = handData.gameObject.GetComponentInChildren<SaveManager>();

    }

    public void StartInteractionEvent()
    {

    }

    private void OnEnable()
    {
        
    }
}
