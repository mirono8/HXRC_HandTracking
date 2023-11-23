using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyStart : MonoBehaviour
{



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
