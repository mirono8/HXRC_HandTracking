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
        public string mode;
    }
    
    public List<ParsedData> sets;

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

        for (int i = 0; i < node[indexOfLatest]["gameData"]["setsList"].Count; i++)
        {
            ParsedData data = new();
            sets.Add(data);

            string id = node[indexOfLatest]["id"].ToString().ToLower().Replace("\"", "");
            sets[i].id = int.Parse(id);

            string size = node[indexOfLatest]["gameData"]["setsList"][i][0].ToString().ToLower().Replace("\"", "");
            sets[i].size = size;

            string type = node[indexOfLatest]["gameData"]["setsList"][i][1].ToString().ToLower().Replace("\"", "");
            sets[i].type = type;

            sets[i].mode = "all";
        }
    }

    public bool Latest()
    {
        int previousId = PlayerPrefs.GetInt("id");

        if(previousId != sets[0].id && sets[0].id != 0)
        {
            //session start
            PlayerPrefs.SetInt("id", sets[0].id);
            return true;
        }
        else
        {
            Debug.Log("id matches with previous session, no new data sent yet");
            //wait
            PlayerPrefs.SetInt("id", sets[0].id);
            return false;
        }
    }
    void Start()
    {
        sets = new();
        StartCoroutine(GetSetupDataFromSite("https://xrdev.edu.metropolia.fi/gamedata/getdata/xr-space-testi"));
        
    }

    public string ReturnSize(int i)
    {
        return sets[i].size;
    }

    public string ReturnType(int i)
    {
        return sets[i].type;
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
