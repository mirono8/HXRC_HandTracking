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
        public DataList left;
        public DataList right;

        [JsonProperty("eT")]
        public float elapsedTime;

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

    [HideInInspector]
    public DataBlock newBlock = new();

    private void Awake()
    {
        data = GetComponentInParent<HandDataOut>();
    }

    private void Start()
    {

        combinedData = new DataWrapper();
        combinedData.left = new();
        combinedData.right = new();
        combinedData.left.blocks = new();
        combinedData.right.blocks = new();

        sessionStartTime = data.GetDate();

    }

    private void Update()
    {
        timeSinceLastStamp += Time.deltaTime;

        combinedData.elapsedTime += Time.deltaTime;
    }

    public DataBlock BlockMe()
    {
        if (newBlock != null)
        {
            newBlock.handPosition = " "; //new Vector3(0, 0, 0)
            newBlock.timeStamp = " ";
            newBlock.collisionEvent = " ";
        }
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

       // block.handPosition.ToString("F5");
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
        block.collisionEvent = new string(hand.myHandedness.ToString().FirstToUpper() + " hand " + collision.collidingFinger.ToLower() + " collided with " + collision.otherCollider + " at " + collision.startTime + ". ");

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
        Save();
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
