using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridToPanel : MonoBehaviour
{
    public GameObject panel;

    public Vector3 desiredDistance;
    
    private void Update()
    {
        transform.position = panel.transform.position + desiredDistance;
    }
}
