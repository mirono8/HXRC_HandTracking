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

    [Serializable]
    public class ParsedData
    {
        public string size;
        public string type;
    }
    
    public ParsedData data;

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
            Debug.Log(www.downloadHandler.text + "");
            //Debug.Log(JsonConvert.DeserializeObject<ParsedData>(www.downloadHandler.text + ""));
            CreateFromJson(www.downloadHandler.text + "");
        }

        www.Dispose();
    }

    public void CreateFromJson(string s)
    {
        JSONNode node = JSON.Parse(s);
        string size = node[1]["gameData"][0].ToString();
        size.Replace("\"","");
        data.size = size;
        string type = node[1]["gameData"][1].ToString();
        type.Replace("\"", "");
        data.type = type;

    }

    void Start()
    {
        data = new();
        StartCoroutine(GetSetupDataFromSite("https://xrdev.edu.metropolia.fi/gamedata/getdata/xr-space-testi"));
    }

}
