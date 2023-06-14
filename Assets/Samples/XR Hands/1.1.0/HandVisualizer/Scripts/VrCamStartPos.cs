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

    void Start()
    {
        StartCoroutine(ResetHead());
    }

    IEnumerator ResetHead()
    {
        yield return new WaitForFixedUpdate();

        var rotAngleY = defaultPos.rotation.y - cam.transform.rotation.y;

        gameObject.transform.Rotate(0, rotAngleY, 0);

        var offset = defaultPos.position - cam.transform.position;

        gameObject.transform.position += offset;  //no work
    }
}
