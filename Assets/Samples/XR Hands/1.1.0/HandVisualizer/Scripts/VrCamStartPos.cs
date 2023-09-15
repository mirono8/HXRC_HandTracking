using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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
    void Start()
    {
        StartCoroutine(ResetHead());
    }

    IEnumerator ResetHead()
    {
        yield return new WaitForSeconds(5f);

        var rotAngleY = defaultPos.rotation.y - cam.transform.rotation.y;

        gameObject.transform.Rotate(0, rotAngleY, 0);

        var offset = defaultPos.position - cam.transform.position;

        gameObject.transform.position = offset; 

        Debug.Log(offset + " offset");
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
                gameObject.transform.position = wp.warpHere.position;
                gameObject.transform.Rotate(0, wp.warpHere.transform.rotation.y, 0);

                gameObject.transform.position = new Vector3(transform.position.x, keepYPos, transform.position.z);

                if (rotate) 
                {
                    // var t =+ Time.deltaTime;
                    var lookHereNoY = new Vector3(wp.lookHere.position.x, wp.lookHere.position.y + keepYPos , wp.lookHere.position.y);
                    gameObject.transform.LookAt(lookHereNoY);  // t‰‰ flippaa x rotationin p‰i persett‰ syyst‰ x (lol!)

                    var offsetRot = transform.rotation.y - cam.transform.rotation.y;
                    
                    transform.Rotate(0,offsetRot,0);

                    
                }
    
            }
        }
        else
        {
            Debug.Log("no warp");
        }

    }
}
