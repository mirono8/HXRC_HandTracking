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
        public int iCount;
        //add button count!!
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

        //uncomment for set debugging use
        ParsedData bogo = new();
        bogo.size = "random";
        bogo.type = "button";
        bogo.mode = "all";
        bogo.iCount = 10;
        sets.Add(bogo);
       


        //GRIDS
        for (int i = 0; i < 3; i++)
        {
            ParsedData defaultData = new();
            defaultData.size = "medium";
            defaultData.type = "button";
            defaultData.mode = "matrix";
            defaultData.iCount = 6;
            sets.Add(defaultData);

            ParsedData defaultData1 = new();
            defaultData1.size = "medium";
            defaultData1.type = "button";
            defaultData1.mode = "matrix";
            defaultData1.iCount = 9;
            sets.Add(defaultData1);

            ParsedData defaultData2 = new();
            defaultData2.size = "medium";
            defaultData2.type = "button";
            defaultData2.mode = "matrix";
            defaultData2.iCount = 3;
            sets.Add(defaultData2);
        }


        //ONE BY ONE
        ParsedData defaultData3 = new();
        defaultData3.size = "medium";
        defaultData3.type = "button";
        defaultData3.mode = "onebyone";
        defaultData3.iCount = 10;
        sets.Add(defaultData3);

        ParsedData defaultData4 = new();
        defaultData4.size = "small";
        defaultData4.type = "button";
        defaultData4.mode = "onebyone";
        defaultData4.iCount = 10;
        sets.Add(defaultData4);

        ParsedData defaultData5 = new();
        defaultData5.size = "large";
        defaultData5.type = "button";
        defaultData5.mode = "onebyone";
        defaultData5.iCount = 10;
        sets.Add(defaultData5);

        for(int i = 0; i < 3; i++)
        {
            ParsedData defaultData6 = new();
            defaultData6.size = "random";
            defaultData6.type = "button";
            defaultData6.mode = "onebyone";
            defaultData6.iCount = 10;
            sets.Add(defaultData6);
        }

        //ALL AT ONCE
        ParsedData defaultData7 = new();
        defaultData7.size = "medium";
        defaultData7.type = "button";
        defaultData7.mode = "all";
        defaultData7.iCount = 10;
        sets.Add(defaultData7);

        ParsedData defaultData8 = new();
        defaultData8.size = "small";
        defaultData8.type = "button";
        defaultData8.mode = "all";
        defaultData8.iCount = 10;
        sets.Add(defaultData8);

        ParsedData defaultData9 = new();
        defaultData9.size = "large";
        defaultData9.type = "button";
        defaultData9.mode = "all";
        defaultData9.iCount = 10;
        sets.Add(defaultData9);

        for (int i = 0; i < 3; i++)
        {
            ParsedData defaultData10 = new();
            defaultData10.size = "random";
            defaultData10.type = "button";
            defaultData10.mode = "all";
            defaultData10.iCount = 10;
            sets.Add(defaultData10);
        }

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

                // string mode = node[indexOfLatest]["gameData"]["setsList"][i][2].ToString().ToLower().Replace("\"", "");
                //sets[i].mode = mode






                //LISƒƒ REACT JA SIT UNCOMMENT   vvvvv Nƒƒ ON PLACEHOLDER
                sets[i].mode = "all";

                //int iCount = node[indexOfLatest]["gameData"]["setsList"][i][3].ToInt()
                sets[i].iCount = 5; //

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

    public int ReturnInteractableCount(int i)
    {
        return sets[i].iCount;
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
