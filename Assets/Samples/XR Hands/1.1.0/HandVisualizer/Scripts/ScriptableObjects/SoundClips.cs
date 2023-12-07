using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundClipScriptable", menuName = "ScriptableObjects/SoundClipScriptableObject", order = 1)]
public class SoundClips : ScriptableObject
{
    [Serializable]
    public class AudioCategory
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public List<AudioClip> audioClips = new List<AudioClip>();
    }

    [SerializeField]
    public List<AudioCategory> audioCategories = new();

}
