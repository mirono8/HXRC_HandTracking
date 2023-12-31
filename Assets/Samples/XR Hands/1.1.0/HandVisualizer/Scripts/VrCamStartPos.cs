using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using UnityEngine.XR;
using UnityEngine.XR.Management;
public class VrCamStartPos : MonoBehaviour
{

    [SerializeField]
    Camera cam;

    [SerializeField]
    Transform defaultPos;

    [SerializeField]
    bool warpToNext = true;

    [SerializeField]
    bool rotate;

    XROrigin xrOrigin;
    XRInputSubsystem xrInput;
    FadeIn fadeIn;

    void Start()
    {
        xrOrigin = GetComponent<XROrigin>();
      //  fadeIn = GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeIn>();

        var xrSettings = XRGeneralSettings.Instance;
        if (xrSettings == null)
        {
            Debug.Log("xrsettings is null");
            return;

        }

        var xrManager = xrSettings.Manager;
        if (xrManager == null)
        {
            Debug.Log($"XRManagerSettings is null.");
            return;
        }

        var xrLoader = xrManager.activeLoader;
        if (xrLoader == null)
        {
            Debug.Log($"XRLoader is null.");
            return;
        }

        Debug.Log($"Loaded XR Device: {xrLoader.name}");

        var xrDisplay = xrLoader.GetLoadedSubsystem<XRDisplaySubsystem>();
        Debug.Log($"XRDisplay: {xrDisplay != null}");


        xrInput = xrLoader.GetLoadedSubsystem<XRInputSubsystem>();
        Debug.Log($"XRInput: {xrInput != null}");

        StartCoroutine(ResetHeadAtStart());
    }



    IEnumerator ResetHeadAtStart()
    {
        /*if (fadeIn != null)
        {
            fadeIn.transform.position = xROrigin.transform.forward;
        }*/
        //BUILD TEST
        Scene scene = SceneManager.GetActiveScene();
        
        if (scene.name != "Lobby")
        {
            yield return new WaitForSeconds(5f);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }

        ResetHead(defaultPos);
    }

    public void ResetHead(Transform target)
    {
        xrInput.TryRecenter();
        xrOrigin.MoveCameraToWorldLocation(target.position);
        Debug.Log("moved");

        Vector3 targetForward = target.forward;
        targetForward.y = 0;
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;

        float angle = Vector3.SignedAngle(camForward, targetForward, Vector3.up);

        xrOrigin.transform.RotateAround(cam.transform.position, Vector3.up, angle);
    }
    public void RotateWhileTrue(bool b)
    {
        rotate = b;
    }
    public void WarpToNextPanel(GridToPanel.WarpPoint wp)
    {
        if (warpToNext)
        {
            Debug.Log("warp");

            /*  var offset = defaultPos.transform.position + (panel.transform.GetChild(0).position + -panel.transform.GetChild(0).forward);

              var rotAngleY = defaultPos.transform.rotation.y - panel.transform.GetChild(0).rotation.y;

              Vector3 vector3 = new Vector3(offset.x + rotAngleY, offset.y, offset.z);
              gameObject.transform.position = offset;

              gameObject.transform.Rotate(0, rotAngleY, 0);*/

            float keepYPos = transform.position.y;

            if (wp.lookHere != null || wp.warpHere != null)
            {
               /* gameObject.transform.position = wp.warpHere.position;
                
                gameObject.transform.Rotate(0, wp.warpHere.transform.rotation.y, 0);

                gameObject.transform.position = new Vector3(transform.position.x, keepYPos, transform.position.z);*/

                /*  if (rotate) 
                  {
                      // var t =+ Time.deltaTime;
                      var lookHereNoY = new Vector3(wp.lookHere.position.x, wp.lookHere.position.y + keepYPos , wp.lookHere.position.y);
                      gameObject.transform.LookAt(lookHereNoY);  // t�� flippaa x rotationin p�i persett� syyst� x (lol!)

                      var offsetRot = transform.rotation.y - cam.transform.rotation.y;

                      transform.Rotate(0,offsetRot,0);


                  }*/
                ResetHead(wp.warpHere);
                transform.position = new Vector3(transform.position.x, keepYPos, transform.position.z);

                //  gameObject.transform.rotation = wp.warpHere.rotation;


            }
            
        }
        else
        {
            Debug.Log("no warp");
        }

    }
}
