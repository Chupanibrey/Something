using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public AudioClip takeCrystalSound;
    public AudioClip deathSound;
    public AudioClip eatSound;
    public AudioClip jumpPlayerSound;
    public AudioClip ropeSound;
    public AudioClip keySound;

    public void PlayClip(AudioClip sound)
    {
        GetComponent<AudioSource>().PlayOneShot(sound);
    }
}