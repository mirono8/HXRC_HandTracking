using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyStart : MonoBehaviour
{
    private void Start()
    {
        FadeIn fadeIn = GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeIn>();
        if (!fadeIn.FaderisFadedOut())
        {
            // fadeIn.ChangeFaderStatus();
            // fadeIn.start = false;
            fadeIn.timer = 0;
        }
    }
    public IEnumerator StartRegularScene()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log(PlayerConfigs.panelHeight);
        SceneManager.LoadSceneAsync("RegularScene");
    }

    public void SetPanelHeight(float value)
    {
        PlayerConfigs.panelHeight = value;
    }
}
