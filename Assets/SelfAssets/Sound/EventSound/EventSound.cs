using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSound : MonoBehaviour
{
    public AudioClip eventSoundFX1;
    public AudioClip eventSoundFX2;
    public AudioClip eventSoundFX3;
    public AudioClip eventSoundFX4;
    AudioSource eventAudioSource;

    void Start()
    {
        eventAudioSource = GetComponent<AudioSource>();
    }

    public void PlayEventSound(string audio) 
    {
        if (audio == "Self Slain") 
        {
            eventAudioSource.PlayOneShot(eventSoundFX1, 1.0f);
        }
        if (audio == "Self Slain Enemy")
        {
            eventAudioSource.PlayOneShot(eventSoundFX2, 1.0f);
        }
        if (audio == "Ally Slain")
        {
            eventAudioSource.PlayOneShot(eventSoundFX3, 1.0f);
        }
        if (audio == "Enemy Slain")
        {
            eventAudioSource.PlayOneShot(eventSoundFX3, 1.0f);
        }
    }
}
