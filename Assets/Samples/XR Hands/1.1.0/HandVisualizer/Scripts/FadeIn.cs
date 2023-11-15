using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class FadeIn : MonoBehaviour
{
    [SerializeField]
    TMP_Text countdown;

    [SerializeField]
    TMP_Text info;

    [SerializeField]
    TMP_Text setData;

    int setCount;
    int currentSet;

    [SerializeField]
    float timer;

    [SerializeField]
    bool start;

    [SerializeField]
    bool fadedOut;
    public Image fader;


    // The time it takes to smooth the movement
    public float smoothTime = 0.3f;
    public float smoothRotationTime = 0.5f;

    // The current velocity of the movement
    private Vector3 velocity;

    public GameObject canvasCamera;

    public GameObject UI;

    public Color infoTextColor;

    bool end;
    public IEnumerator FadeCanvasOut()
    {
        Debug.Log("Fading out");

        start = true;

        do
        {
            var num = Mathf.RoundToInt(timer);
            countdown.text = num.ToString();

            if (timer <= 0)
            {
                countdown.text = "Start!";

                fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, fader.color.a - (0.02f * 1f));  //kerroin hidastaa

                countdown.color = new Color(countdown.color.r, countdown.color.g, countdown.color.b, fader.color.a - (0.02f * 1f));

                info.color = new Color(info.color.r, info.color.g, info.color.b, fader.color.a - (0.02f * 1f));

                setData.color = new Color(setData.color.r, setData.color.g, setData.color.b, setData.color.a - (0.02f * 1f));

            }

            if (fader.color.a <= 0)
            {
                start = false;
                fadedOut = true;
                info.color = infoTextColor;
                info.alpha = 0;
                UI.GetComponentInChildren<UIComponent>().ResetComponents();
                GameObject.FindGameObjectWithTag("CanvasCamera").GetComponent<Camera>().cullingMask = LayerMask.GetMask("CanvasFirst");
            }

            yield return null;

        } while (!fadedOut);


    }

    public IEnumerator FadeCanvasIn()
    {
        Debug.Log("Fading in");

        setData.text = currentSet.ToString() + "/" + setCount.ToString();
        do
        {
            if (fader.color.a < 1 && fadedOut)
            {
               // start = true;

                countdown.text = "Initializing next set..";

                

                fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, fader.color.a + (0.02f * 1f));

                countdown.color = new Color(countdown.color.r, countdown.color.g, countdown.color.b, fader.color.a + (0.02f *1f));

                info.color = new Color(info.color.r, info.color.g, info.color.b, fader.color.a + (0.02f * 1f));

                setData.color = new Color(setData.color.r, setData.color.g, setData.color.b, setData.color.a + (0.02f * 1f));
                //gameObject.SetActive(false);


            }
            else
            {
                fadedOut = false;
                info.color = infoTextColor;
                GameObject.FindGameObjectWithTag("CanvasCamera").GetComponent<Camera>().cullingMask = LayerMask.GetMask("CanvasFirst", "Hands");
                //info.alpha = 0;
            }

            yield return null;

        } while (fadedOut);

        
    }

    public bool FaderStatus()
    {
        return fadedOut;
    }

    public void ChangeFaderStatus()
    {
        if (!fadedOut)
        {
            fadedOut = true;
        }
        else
        {
            fadedOut = false;
        }
    }

    public void FaderInfoText(string s)
    {
        switch (s)
        {
            case "all":
                info.text = "Interactable objects appear <b><color=white>simultaneously</b></color=white> on the panel.\n  Interact with the <color=#2EDD20>highlighted</color> interactable.";
                break;

            case "onebyone":
                info.text = "Interactable objects appear <b><color=white>one by one</b></color=white> on the panel.\n  Interact with the <color=#2EDD20>highlighted</color> interactable.";
                break;

            case "matrix":
                info.text = "Interactable objects appear in a <b><color=white>grid</b></color=white> on the panel.\n  Interact with the <color=#2EDD20>highlighted</color> interactable.";
                break;
        }
    }

    public void SessionEnded(float time)
    {
        if (fadedOut)
        {
            end = true;
            StartCoroutine(FadeCanvasIn());
            countdown.text = "Session has ended";
            info.text = "Total elapsed time: " + time;
        }
    }

    public void WaitForUser()
    {
        UI.SetActive(true);
        Debug.Log("waiting for user input");
        
    }

    public void SetSetCount(int i)
    {
        setCount = i;
    }

    public void SetCurrentSet(int i)
    {
        currentSet = i;
    }
    private void Start()
    {
        infoTextColor = info.color;
        setData.text = currentSet.ToString() + "/" + setCount.ToString();
       
    }
    private void FixedUpdate()
    {
        if (!start)
        {
            timer = 3.45f;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if (end)
        {
            countdown.text = "Session has ended";
        }
        Vector3 targetPos = canvasCamera.transform.TransformPoint(new Vector3(0, 0, 13f));


        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, (targetPos), ref velocity, smoothTime);

        transform.position = smoothedPosition;

        var lookAtPosition = new Vector3(canvasCamera.transform.position.x, canvasCamera.transform.position.y, canvasCamera.transform.position.z);
        transform.LookAt(lookAtPosition);
    }
}
