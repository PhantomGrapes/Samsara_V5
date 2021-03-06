﻿using UnityEngine;
using System.Collections;

public class MainCharacterAudioController : MonoBehaviour {
    // define the audio clips
    public AudioClip clipWalk;
    public AudioClip clipHit;
    public AudioClip clipDead;
    public AudioClip clipHeavySword1;
    public AudioClip clipHeavySword2;
    public AudioClip clipArrow1;
    public AudioClip clipArrow2;
    public AudioClip clipHeavySwordSkill1;
    public AudioClip clipHeavySwordSkill2;
    public AudioClip clipSS;
    public AudioClip clipWA;
    public AudioClip clipSSSkill;
    public AudioClip clipDaggerSkill;
    public AudioClip clipDagger;

    public AudioSource audioWalk;
    public AudioSource audioHit;
    public AudioSource audioDead;
    public AudioSource audioHeavySword1;
    public AudioSource audioHeavySword2;
    public AudioSource audioArrow1;
    public AudioSource audioArrow2;
    public AudioSource audioHeavySwordSkill1;
    public AudioSource audioHeavySwordSkill2;
    public AudioSource audioSS;
    public AudioSource audioWA;
    public AudioSource audioSSSkill;
    public AudioSource audioDaggerSkill;
    public AudioSource audioDagger;

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip; 
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol; 
        return newAudio; 
    }
// Use this for initialization
    void Start () {

        audioWalk = AddAudio(clipWalk, true, false, 1f);
        audioWalk.pitch = 2f;
        audioHit = AddAudio(clipHit, false, false, 1f);
        audioDead = AddAudio(clipDead, false, false, 1f);
        audioHeavySword1 = AddAudio(clipHeavySword1, false, false, 1f);
        audioHeavySword2 = AddAudio(clipHeavySword2, false, false, 1f);
        audioArrow1 = AddAudio(clipArrow1, false, false, 1f);
        audioArrow2 = AddAudio(clipArrow2, false, false, 1f);
        audioHeavySwordSkill1 = AddAudio(clipHeavySwordSkill1, false, false, 1f);
        audioHeavySwordSkill2 = AddAudio(clipHeavySwordSkill2, false, false, 1f);
        audioSS = AddAudio(clipSS, false, false, 1f);
        audioWA = AddAudio(clipWA, false, false, 1f);
        audioSSSkill = AddAudio(clipSSSkill, false, false, 1f);
        audioDaggerSkill = AddAudio(clipDaggerSkill, false, false, 1f);
        audioDagger = AddAudio(clipDagger, false, false, 1f);
    }
	
    void Update()
    {
        // control loop audios
        if (GetComponent<Rigidbody2D>().velocity.x != 0 && Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y) < 0.1)
        {
            print("walk");
            if (!audioWalk.isPlaying)
                audioWalk.Play();
        }
        else
            audioWalk.Pause();     
    }
}
