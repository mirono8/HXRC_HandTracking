using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFeedback : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField]
    SoundClips soundClips;

    [SerializeField]
    List<AudioClip> queue;

    bool disableAfter;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }

    public void PlaySoundClip(int clipId, string type = "hand")  //by default, clipId 0 == positive, clipId 1 == negative
    {
        if (audioSource != null)
        {
            AudioClip clip = null;

            switch (type)  //needs testing whether isplaying clause is needed
            {
                case "hand": clip = soundClips.audioCategories.Find(x => x.name == "HandProximitySound").audioClips[clipId];

                    if (audioSource.isPlaying)
                    {
                        if (queue.Count == 0)
                        {
                            AddToQueue(clip);
                        }
                        else
                        {
                            queue.Clear();
                            AddToQueue(clip);
                        }
                    }
                    else
                    {
                        audioSource.PlayOneShot(clip);
                    }
                    break;

                case "button": clip = soundClips.audioCategories.Find(x => x.name == "ButtonPress").audioClips[clipId];
                    queue.Clear();
                    audioSource.PlayOneShot(clip);
                    break;
            }
        }
    }

    public void PlayAndDisable(int clipId)
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(soundClips.audioCategories.Find(x => x.name == "ButtonPress").audioClips[clipId]);
            disableAfter = true;
        }

    }
    public void AddToQueue(AudioClip clip)
    {
        queue.Add(clip);
    }

    private void Update()
    {
       if(queue.Count > 0 && !audioSource.isPlaying)
       {
            audioSource.PlayOneShot(queue[0]);
            queue.Clear();
       }

        if (disableAfter)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.mute = true;
            }
        }
    }
}
