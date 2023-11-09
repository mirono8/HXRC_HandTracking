using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartRegularScene());
    }

    IEnumerator StartRegularScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadSceneAsync("RegularScene");
    }
}
