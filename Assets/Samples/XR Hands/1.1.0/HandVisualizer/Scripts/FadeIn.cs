using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeIn : MonoBehaviour
{
    public TextMesh countdown;

    float timer;

    [SerializeField]
    bool start;

    public Image fader;
    public void StartFade()
    {

        fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, fader.color.a - (timer * 0.25f));
        
        countdown.text = timer.ToString();

        if (fader.color.a <= 0)
        {
            start = false;
            countdown.text = "Start!";
            countdown.color = new Color(fader.color.r, fader.color.g, fader.color.b, fader.color.a - (timer * 0.75f));
        }
    }

    private void Start()
    {
        start = true;
    }
    private void FixedUpdate()
    {
        if (!start)
        {
            timer = 0;
        }
        else
        {
            timer = Time.deltaTime;
            StartFade();
        }
    }
}
