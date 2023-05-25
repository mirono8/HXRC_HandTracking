using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using HandData;
using System.Linq;
using System;
using Unity.XR.CoreUtils;
using Unity.Plastic.Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    HandDataOut data;

    string sessionStartTime;
    private readonly string dirPath = "./Assets/";

    [JsonProperty("data")]
    public DataWrapper combinedData;

    readonly float stampInterval = 0.05f;

    [SerializeField]
    float timeSinceLastStamp = 0;

    [SerializeField]

    public class DataWrapper
    {
        [JsonProperty("eT")]
        public float elapsedTime;

        [JsonProperty("T")]
        public Tasks.Task currentTask;

        [SerializeField]
        [JsonProperty("iE")]
        public EventList interactableEvents;
        public DataList left;
        public DataList right;

        [Serializable]
        public class EventList
        {
            [SerializeField]
            [JsonProperty("e")]
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
        [JsonProperty("tS")]
        public string timeStamp;

        [JsonProperty("hP")]
        public string handPosition;

        [JsonProperty("cE")]
        public string collisionEvent;

    }

    [System.Serializable]
    public class InteractableEvent
    {
        [JsonProperty("sP")]
        public string startPoint;
        [JsonProperty("eP")]
        public string endPoint;

        [JsonProperty("iT")]
        public string interactableType;
        [JsonProperty("iS")]
        public string interactableSize;

        [JsonProperty("di")]
        public string distance;
        [JsonProperty("du")]
        public string duration;

        [JsonProperty("tr")]
        public List<string> trajectory;
    }

    [HideInInspector]
    public DataBlock newBlock = new();

    private void Awake()
    {
        data = GameObject.FindGameObjectWithTag("HandData").GetComponent<HandDataOut>();
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

    }

    private void Update()
    {
        timeSinceLastStamp += Time.deltaTime;

        combinedData.elapsedTime += Time.deltaTime;

        if (combinedData.currentTask.isCompleted)
        {
            Save();
            //GEt new task
            sessionStartTime = data.GetDate();
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
        var block = new DataBlock();

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

    public void Save()
    {
        for (int i = 0; i < combinedData.left.blocks.Count; i++)
        {
            if (combinedData.left.blocks[i].collisionEvent == null)
                combinedData.left.blocks[i].collisionEvent = "0";
            
            if (combinedData.left.blocks[i].timeStamp == null)
                combinedData.left.blocks[i].timeStamp =  "0";

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

        var j = JsonConvert.SerializeObject(combinedData, Formatting.Indented);

        // var json = JsonUtility.ToJson(combinedData);

        var saveFolder = Directory.CreateDirectory(dirPath + "JSONFiles/");

        File.WriteAllText(saveFolder + "HandTrackingData-" + sessionStartTime + ".json", j);
    }
}
