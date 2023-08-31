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

    public bool overrideWithDefaults;
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
            GoByDefault();
        }
        else
        {
          //  Debug.Log(www.downloadHandler.text + "");
            CreateFromJson(www.downloadHandler.text + "");
        }

        www.Dispose();
    }

    void GoByDefault()
    {
        Debug.Log("defaulted");
        ParsedData defaultData = new();
        defaultData.size = "medium";
        defaultData.type = "button";
        defaultData.mode = "all";
        sets.Add(defaultData);

        ParsedData defaultData1 = new();
        defaultData1.size = "medium";
        defaultData1.type = "switch";
        defaultData1.mode = "all";
        sets.Add(defaultData1);

        ParsedData defaultData2 = new();
        defaultData2.size = "medium";
        defaultData2.type = "button";
        defaultData2.mode = "oneByOne";
        sets.Add(defaultData2);
    }

    public void CreateFromJson(string s)
    {
        JSONNode node = JSON.Parse(s);

        int latestId = 0;
        int indexOfLatest = 0;

        if (node != "")
        {
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
        else
        {
            Debug.Log("No web data");
            GoByDefault();
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
        if (!overrideWithDefaults)
        {
            StartCoroutine(GetSetupDataFromSite("https://xrdev.edu.metropolia.fi/api/gamedata/getdata/xr-space-testi"));
        }
        else
            GoByDefault();
        //GoByDefault();
    }

    public string ReturnSize(int i)
    {
        return sets[i].size;
    }

    public string ReturnType(int i)
    {
        return sets[i].type;
    }

    public string ReturnMode(int i)
    {
        return sets[i].mode;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 5)
        {
            onSessionStart.SetActive(true);
           /* if (!Latest())  //kommentoitu et pystyy uudelleenk‰ytt‰‰ aiempaa sessiota
            {
                timer = 0;
                StartCoroutine(GetSetupDataFromSite("https://xrdev.edu.metropolia.fi/api/gamedata/getdata/xr-space-testi"));
            }
            else
            {
                onSessionStart.SetActive(true);
                gameObject.SetActive(false);
            }*/
           gameObject.SetActive(false);
        }
    }
}
