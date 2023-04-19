using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip backgroundMusic;

    void Start()
    {
        // Set the audio clip for the audio source
        audioSource.clip = backgroundMusic;

        // Set the loop property to true
        audioSource.loop = true;

        // Play the audio clip
        audioSource.Play();
    }
}
