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

    [SerializeField]
    bool debug;

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
            countdown.color = new Color(countdown.color.r, countdown.color.g, countdown.color.b, fader.color.a - (0.02f * 0.25f));
            
        }

        if(fader.color.a <= 0)
        {
            start = false;
            faded = true;

        }

    }

    public void FadeBack()
    {
        
        if (fader.color.a <= 1 && faded)
        {
            start = true;

            countdown.text = "Initializing next set..";
            fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, fader.color.a + (0.02f * 0.25f));
            countdown.color = new Color(countdown.color.r, countdown.color.g, countdown.color.b, fader.color.a + (0.02f * 0.25f));
            //gameObject.SetActive(false);
            debug = true;
            
        }
        else
        {
            faded = false;
            start = false;
            debug = false;
        }
    }

    public bool FaderStatus()
    {
        return faded;
    }


    private void FixedUpdate()
    {
        if (!start)
        {
            timer = 3;
        }
        else if(!faded)
        {
            timer -= Time.deltaTime;
            StartFade();
        }

        if (faded && start)
        {
            FadeBack();
        }
        //logiikka kuntoon!!! out of sequence
    }
}
