using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFeedback : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField]
    SoundClips soundClips;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }

    public void PlaySoundClip(int clipId, string type = "hand")  //by default, clipId 0 == positive, clipId 1 == negative
    {
        if (audioSource != null)
        {
            if (!audioSource.isPlaying)
            {
                switch (type)
                {
                    case "hand": audioSource.PlayOneShot(soundClips.audioCategories.Find(x => x.name == "HandProximitySound").audioClips[clipId]); break;

                    case "button": audioSource.PlayOneShot(soundClips.audioCategories.Find(x => x.name == "ButtonPress").audioClips[clipId]); break;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("FunnyTestKey"))
        {
            Debug.Log("pressedtestkey");
            PlaySoundClip(0,"button");
        }
    }
}
