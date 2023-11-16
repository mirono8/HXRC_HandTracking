using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using HandData;
using System.Linq;
using System;
using Unity.XR.CoreUtils;
using SimpleJSON;
using UnityEngine.PlayerLoop;
using System.Text;
using UnityEngine.Networking;

public class SaveManager : MonoBehaviour
{
    HandDataOut data;

    string sessionStartTime;
    private readonly string dirPath = "./Assets/";

    //[JsonProperty("data")]
    public DataWrapper combinedData;

    readonly float stampInterval = 0.05f;
  
    float timeSinceLastStamp = 0;

    SessionManager sessionManager;

    string json;
    [SerializeField]
    public class DataWrapper
    {
        //[JsonProperty("eT")]
        
        public float elapsedTime;

       // [JsonProperty("T")]
        public Tasks.Task currentTask;

        [SerializeField]
      //  [JsonProperty("iE")]
        public EventList interactableEvents;
        public DataList left;
        public DataList right;

        [Serializable]
        public class EventList
        {
            [SerializeField]
            //[JsonProperty("e")]
            public List<InteractableEvent> events;
        }

        [Serializable]
        public class DataList
        {
            [SerializeField]
            public List<DataBlock> blocks;
        }
    }

    [System.Serializable]
    public class DataBlock
    {
        //[JsonProperty("tS")]
        public string timeStamp;

       // [JsonProperty("hP")]
        public string handPosition;

       // [JsonProperty("cE")]
        public string collisionEvent;

    }

    [System.Serializable]
    public class InteractableEvent
    {
        //[JsonProperty("sP")]
        public string startPoint;
        //[JsonProperty("eP")]
        public string endPoint;

       // [JsonProperty("iT")]
        public string interactableType;
       // [JsonProperty("iS")]
        public string interactableSize;

       // [JsonProperty("di")]
        public string distance;
       // [JsonProperty("du")]
        public string duration;

       // [JsonProperty("tr")]
        public List<string> trajectory;
    }

    [HideInInspector]
    public DataBlock newBlock = new();

    private void Awake()
    {
        data = GameObject.FindGameObjectWithTag("HandData").GetComponent<HandDataOut>();
        sessionManager = GameObject.FindGameObjectWithTag("SessionManager").GetComponent<SessionManager>();
    }

    private void Start()
    {

        combinedData = new DataWrapper();
        combinedData.left = new();
        combinedData.right = new();
        combinedData.left.blocks = new();
        combinedData.right.blocks = new();

        combinedData.currentTask = new Tasks.Task();

        combinedData.interactableEvents = new();
        combinedData.interactableEvents.events = new();
        sessionStartTime = data.GetDate();

         json = JsonUtility.ToJson(combinedData);
    }

    private void Update()
    {
        timeSinceLastStamp += Time.deltaTime;

        if (sessionManager.CurrentState() == States.State.Active)
        {
            combinedData.elapsedTime += Time.deltaTime;
        }
        
        if (sessionManager.CurrentState() == States.State.End)
        {
          /*  float endTime = combinedData.elapsedTime;
            sessionManager.SetSessionEndTime(endTime);*/
            Save();
            gameObject.SetActive(false);
        }
        

        if (combinedData.currentTask.isCompleted)
        {
            Save();
            //GEt new task
            sessionStartTime = data.GetDate();
        }

        if (Input.GetButtonDown("FunnyTestKey"))
            StartCoroutine(SendSessionDataToSite("https://xrdev.edu.metropolia.fi/api/gamedata/createdata/", combinedData));
    }
    public float SetSessionEndTime()
    {
        return combinedData.elapsedTime;
    }
    IEnumerator SendSessionDataToSite(string uri, DataWrapper data)   //uri = https://xrdev.edu.metropolia.fi/api/gamedata/
    {
        // test=?
       /* byte[] rawJson = Encoding.UTF8.GetBytes(json);
        UnityWebRequest www = new UnityWebRequest(uri, "POST");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(rawJson);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        
        yield return www.SendWebRequest();
        Debug.Log("Status: " + www.responseCode);
       */
        WWWForm form = new();
        byte[] rawJson = Encoding.UTF8.GetBytes(json);

        form.AddField("title", "xr-space-data-test");
       // form.AddField("data", JsonUtility.ToJson(data));
        //form.AddBinaryData("data", rawJson);

        
        UnityWebRequest www = UnityWebRequest.Post(uri, form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete");
        }
    }

    public DataBlock BlockMe()
    {

        newBlock.handPosition = " "; //new Vector3(0, 0, 0)
        newBlock.timeStamp = " ";
        newBlock.collisionEvent = " ";

        return newBlock;
    }

    
    public void Stamp(DataBlock block)
    {
        
        block.timeStamp = DateTime.Now.ToString("HH-mm-ss");

    }

    public void SaveHandLocation(HandDataOut.Hand hand)
    {
        DataBlock block = new DataBlock();

        if (timeSinceLastStamp > stampInterval)
        {
            Stamp(block);
            timeSinceLastStamp = 0;
        }

        block.handPosition = hand.handPosition.ToString("F5");

        if (hand.myHandedness == HandDataOut.Hand.MyHandedness.left)
        {
           combinedData.left.blocks.Add(block);
        }

        if (hand.myHandedness == HandDataOut.Hand.MyHandedness.right)
        {
            combinedData.right.blocks.Add(block);
        }
    }

    public void SaveFingerCollision(HandDataOut.Hand hand, TrackColliders.CollisionEvent collision)
    {
        var block = new DataBlock();

        block.collisionEvent = new string(hand.myHandedness.ToString().FirstToUpper() + " hand " + collision.collidingFinger.ToLower() + " -> " + collision.otherCollider + " at " + collision.startTime);

      //  Debug.Log(block.collisionEvent.ToString());
        if (hand.myHandedness == HandDataOut.Hand.MyHandedness.left)
        {
            combinedData.left.blocks.Add(block);
        }

        if (hand.myHandedness == HandDataOut.Hand.MyHandedness.right)
        {
            combinedData.right.blocks.Add(block);
        }
    }

    private void OnApplicationQuit()
    {
        
        Save(); //for now
        
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }

    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save();
            
        }
    }


    public void Save()
    {
        
            CheckOnNull();

            //interactable events here!!!!!     //alikansio aina startissa maybe!
            
            // var j = JsonConvert.SerializeObject(combinedData, Formatting.Indented);
            json = JsonUtility.ToJson(combinedData);
            
            // var json = JsonUtility.ToJson(combinedData);

#if UNITY_EDITOR

            var saveFolder = Directory.CreateDirectory(dirPath + "JSONFiles/");

#else

   var saveFolder = Application.persistentDataPath;

#endif


       // if (sessionManager.allClear)
      //  {
            File.WriteAllText(saveFolder + "HandTrackingData-" + sessionStartTime + ".json", json);

       
      //  }
    }
    

    public void CheckOnNull()
    {

        for (int i = 0; i < combinedData.left.blocks.Count; i++)
        {
            if (combinedData.left.blocks[i].collisionEvent == null)
                combinedData.left.blocks[i].collisionEvent = "0";

            if (combinedData.left.blocks[i].timeStamp == null)
                combinedData.left.blocks[i].timeStamp = "0";

            if (combinedData.left.blocks[i].handPosition == null)
                combinedData.left.blocks[i].handPosition = "0";


        }

        for (int i = 0; i < combinedData.right.blocks.Count; i++)
        {
            if (combinedData.right.blocks[i].collisionEvent == null)
                combinedData.right.blocks[i].collisionEvent = "0";

            if (combinedData.right.blocks[i].timeStamp == null)
                combinedData.right.blocks[i].timeStamp = "0";

            if (combinedData.right.blocks[i].handPosition == null)
                combinedData.right.blocks[i].handPosition = "0";
        }

        for (int i = 0; i < combinedData.interactableEvents.events.Count; i++)
        {
            if (combinedData.interactableEvents.events[i].distance == null)
                combinedData.interactableEvents.events[i].distance = "0";

            if (combinedData.interactableEvents.events[i].interactableType == null)
                combinedData.interactableEvents.events[i].interactableType = "0";

            if (combinedData.interactableEvents.events[i].endPoint == null)
                combinedData.interactableEvents.events[i].endPoint = "0";

            if (combinedData.interactableEvents.events[i].startPoint == null)
                combinedData.interactableEvents.events[i].startPoint = "0";

            if (combinedData.interactableEvents.events[i].interactableSize == null)
                combinedData.interactableEvents.events[i].interactableSize = "0";

            if (combinedData.interactableEvents.events[i].duration == null)
                combinedData.interactableEvents.events[i].duration = "0";
        }
    }

}
