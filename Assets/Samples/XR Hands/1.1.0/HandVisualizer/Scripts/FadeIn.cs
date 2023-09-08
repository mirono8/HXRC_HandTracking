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
                fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, fader.color.a - (0.02f * 0.75f));
                countdown.color = new Color(countdown.color.r, countdown.color.g, countdown.color.b, fader.color.a - (0.02f * 0.75f));

            }

            if (fader.color.a <= 0)
            {
                start = false;
                faded = true;
            }

            yield return null;

        } while (!faded);

       
    }

    public IEnumerator FadeCanvasIn()
    {
        Debug.Log("Fading in");
        /* if (fader.color.a <= 1 && faded)
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
         }*/

        do
        {
            if (fader.color.a < 1 && faded)
            {
               // start = true;

                countdown.text = "Initializing next set..";
                fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, fader.color.a + (0.02f * 0.75f));
                countdown.color = new Color(countdown.color.r, countdown.color.g, countdown.color.b, fader.color.a + (0.02f * 0.75f));
                //gameObject.SetActive(false);
                debug = true;

            }
            else
            {
                faded = false;
            }

            yield return null;
        } while (faded);

        
    }

    public bool FaderStatus()
    {
        return faded;
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


        /* if (faded && start)
         {
             FadeBack();
         }*/


        //jos liian nopee, jää jumiin??? sequencing-ongelma setstart tai randombuttons
    }
}
