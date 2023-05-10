using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tasks : MonoBehaviour
{
    public class Task
    {
        public string name;
        public List<GameObject> targets = new List<GameObject>();
        public bool isCompleted;
    }

}