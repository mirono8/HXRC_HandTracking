using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridToPanel : MonoBehaviour
{
    public GameObject panel;

    public Vector3 desiredDistance; // z0,01

    PanelManager panelManager;

    public Vector3 panelScale = new Vector3(0.846153855f, 0.846153855f, 1f);

    [SerializeField]
    Vector3 angle;

    GameObject gridActual;

    [SerializeField]
    Transform cameraWarpPoint;

    public Vector3 warpDistance;

    [System.Serializable]
    public class WarpPoint
    {
        [SerializeField]
        public Transform warpHere;

        [SerializeField]
        public Transform lookHere;
    }


    public WarpPoint warpPoint = new();
    private void Start()
    {

        desiredDistance = new Vector3(0,0,-0.035f);


        panelManager = GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>();
        panel = panelManager.GiveMeAPanel();

        gridActual = transform.GetChild(0).gameObject;

        if (panel != null)
        {
            cameraWarpPoint.position = new Vector3(gridActual.transform.position.x, gridActual.transform.position.y, gridActual.transform.position.z);

            warpPoint.warpHere = cameraWarpPoint;
            warpPoint.lookHere = gridActual.transform;
            transform.parent = panel.transform;
            // Vector3 withScaleOffset = new Vector3(panelVector.x * panelScale.x, panelVector.y * panelScale.y, panelVector.z * panelScale.z);
            //transform.GetChild(0).position = panelManager.GetPanelTransform(panel).position + desiredDistance;
            /*  cameraWarpPoint.transform.position = panelManager.GetPanelTransform(panel).position + warpDistance;  // this is useful, commented for now

              //cameraWarpPoint.transform.Rotate(panelManager.GetPanelTransform(panel).localPosition, panelManager.GetPanelRotation(panel).y);
              var quat = Quaternion.Euler(panelManager.GetPanelRotation(panel).x, panelManager.GetPanelRotation(panel).y, panelManager.GetPanelRotation(panel).z);

              Debug.Log("without quat " +cameraWarpPoint.transform.position);
              Debug.Log(cameraWarpPoint.transform.position = quat * cameraWarpPoint.transform.position);

              cameraWarpPoint.transform.position = quat * cameraWarpPoint.transform.position;

              cameraWarpPoint.transform.position = new Vector3(cameraWarpPoint.transform.position.x*-1, cameraWarpPoint.transform.position.y*-1, panelManager.GetPanelTransform(panel).localPosition.z + warpDistance.z);

              cameraWarpPoint.transform.eulerAngles = new Vector3(cameraWarpPoint.transform.rotation.x, panelManager.GetPanelRotation(panel).y, cameraWarpPoint.transform.rotation.z);*/

             // + warpDistance.z

        }
    }

    private void Update()
    {
        if (panel != null)
        {
            gridActual.transform.localPosition = panelManager.GetPanelTransform(panel).localPosition + desiredDistance;
            gridActual.transform.eulerAngles = panelManager.GetPanelRotation(panel);

            cameraWarpPoint.position = new Vector3(gridActual.transform.position.x, gridActual.transform.position.y, gridActual.transform.position.z) + ((gridActual.transform.forward / 2));
            cameraWarpPoint.LookAt(gridActual.transform);
            //KÄÄNNÄ BNAPIT OIKEIN PÄIN KOSKA PREFAB MUUYTTU!!!

            
            // cameraWarpPoint.transform.localPosition = panelManager.GetPanelTransform(panel).localPosition + warpDistance;
            //  cameraWarpPoint.transform.eulerAngles = panelManager.GetPanelRotation(panel);  // ei oo tarkallee siin mis pitäs, ehkä forward auttaa

        }
    }
}
 