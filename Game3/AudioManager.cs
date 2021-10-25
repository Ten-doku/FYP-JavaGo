using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip Magic1;

    public AudioClip Magic2;
    public AudioClip Heal;
    public AudioClip Sword;
    public AudioClip Button;
    public AudioClip Mushroom;

    public static AudioManager instance;

    private void Awake(){
       instance = this;
        
    }

}
