using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using SimpleJSON;
using System;

public class GetSetupData : MonoBehaviour
{
    //h ttps://xrdev.edu.metropolia.fi/gamedata/getdata/xr-space-testi

    public float timer = 0f;
    [Serializable]
    public class ParsedData
    {
        public int id;
        public string size;
        public string type;
    }
    
    public ParsedData data;

    public GameObject onSessionStart;

    IEnumerator GetSetupDataFromSite(string uri)
    {
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
          //  Debug.Log(www.downloadHandler.text + "");
            CreateFromJson(www.downloadHandler.text + "");
        }

        www.Dispose();
    }

    public void CreateFromJson(string s)
    {
        JSONNode node = JSON.Parse(s);

        int latestId = 0;
        int indexOfLatest = 0;
        for (int i = 0; i < node.Count; i++)
        {
            if (latestId < node[i]["id"] && node[i]["title"] == "xr-space-testi")
            {
                latestId = node[i]["id"];
                indexOfLatest = i;
            }
        }
        Debug.Log(latestId);

        string id = node[indexOfLatest]["id"].ToString().ToLower().Replace("\"", "");
        data.id = int.Parse(id);

        string size = node[indexOfLatest]["gameData"][0].ToString().ToLower().Replace("\"", "");
        data.size = size;

        string type = node[indexOfLatest]["gameData"][1].ToString().ToLower().Replace("\"", "");
        data.type = type;
        
    }

    public bool Latest()
    {
        int previousId = PlayerPrefs.GetInt("id");

        if(previousId != data.id && data.id != 0)
        {
            //session start
            PlayerPrefs.SetInt("id", data.id);
            return true;
        }
        else
        {
            Debug.Log("id matches with previous session, no new data sent yet");
            //wait
            PlayerPrefs.SetInt("id", data.id);
            return false;
        }
    }
    void Start()
    {
        data = new();
        StartCoroutine(GetSetupDataFromSite("https://xrdev.edu.metropolia.fi/gamedata/getdata/xr-space-testi"));
    }

    public string ReturnSize()
    {
        return data.size;
    }

    public string ReturnType()
    {
        return data.type;
    }
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 5)
        {
            onSessionStart.SetActive(true);
            /* if (!Latest())
             {
                 timer = 0;
                 StartCoroutine(GetSetupDataFromSite("https://xrdev.edu.metropolia.fi/gamedata/getdata/xr-space-testi"));
             }
             else
             {
                 onSessionStart.SetActive(true);
                 gameObject.SetActive(false);
             }
         }*/
        }
    }
}
