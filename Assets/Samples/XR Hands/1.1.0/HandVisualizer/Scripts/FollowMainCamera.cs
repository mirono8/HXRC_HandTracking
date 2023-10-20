using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMainCamera : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject mainCamera;

    FadeIn fadeIn;
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = mainCamera.transform.position;

    }
}
