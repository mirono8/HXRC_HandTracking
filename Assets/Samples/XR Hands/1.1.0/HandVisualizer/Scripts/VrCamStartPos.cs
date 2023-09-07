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

    public void WarpToNextPanel(Transform t)
    {
        if (warpToNext)
        {
            Debug.Log("warp");

            /*  var offset = defaultPos.transform.position + (panel.transform.GetChild(0).position + -panel.transform.GetChild(0).forward);

              var rotAngleY = defaultPos.transform.rotation.y - panel.transform.GetChild(0).rotation.y;

              Vector3 vector3 = new Vector3(offset.x + rotAngleY, offset.y, offset.z);
              gameObject.transform.position = offset;

              gameObject.transform.Rotate(0, rotAngleY, 0);*/

            gameObject.transform.position = t.position;
            gameObject.transform.rotation = t.rotation;
        }
        else
        {
            Debug.Log("no warp");
        }

    }
}
