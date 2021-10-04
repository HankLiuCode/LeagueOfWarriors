using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public AudioClip soundFX0;
    public AudioClip soundFX1;
    public AudioClip soundFX2;
    public AudioClip soundFX3;
    public AudioClip soundFX4;
    public AudioClip soundFX5;
    AudioSource audiosource;

    void Start()
    {
        audiosource = GetComponent<AudioSource>();
    }

    public void PlaySFX(string audio)
    {
        if (audio == "attack")
        {
            audiosource.PlayOneShot(soundFX0, 0.7f);
        }
        if (audio == "Skill_Q") 
        {
            audiosource.PlayOneShot(soundFX1, 0.7f);
        }
        if (audio == "Skill_W")
        {
            audiosource.PlayOneShot(soundFX2, 0.7f);
        }
        if (audio == "Skill_E")
        {
            audiosource.PlayOneShot(soundFX3, 0.7f);
        }
        if (audio == "Skill_R")
        {
            audiosource.PlayOneShot(soundFX4, 0.7f);
        }
        if (audio == "dead")
        {
            audiosource.PlayOneShot(soundFX5, 0.7f);
        }
    }
}