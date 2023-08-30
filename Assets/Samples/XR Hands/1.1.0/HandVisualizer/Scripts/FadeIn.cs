using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeIn : MonoBehaviour
{
    public TMP_Text countdown;

    [SerializeField]
    float timer;

    [SerializeField]
    bool start;

    [SerializeField]
    bool faded;
    public Image fader;
    public void StartFade()
    {

        start = true;
        
        var num = Mathf.RoundToInt(timer);
        countdown.text = num.ToString();

        if (timer <= 0)
        {
           // start = false;
            countdown.text = "Start!";
            fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, fader.color.a - (0.02f * 0.25f));
            countdown.color = new Color(fader.color.r, fader.color.g, fader.color.b, fader.color.a - (0.02f * 0.25f));
            
        }

        if(fader.color.a <= 0)
        {
            start = false;
            faded = true;
            gameObject.SetActive(false);
        }

    }

    public bool FaderStatus()
    {
        return faded;
    }
    private void OnDisable()
    {
        start = false;
        
    }

    private void OnEnable()
    {
        faded = false;
    }

    private void FixedUpdate()
    {
        if (!start)
        {
            timer = 3;
        }
        else
        {
            timer -= Time.deltaTime;
            StartFade();
        }
    }
}
