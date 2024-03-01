using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip MoveSound1;
    public AudioClip MoveSound2;
    public AudioClip MoveSound3;
    public AudioClip MoveSound4;

    public AudioClip BGM1;

    public AudioClip MagicSkill;
    public AudioClip QueenSkill;
    public AudioClip RookSkill;
    public AudioClip KnightSkill;
    public AudioClip PawnSkill;

    public AudioClip BasicAttack;
    AudioSource MyAudio;
    public static SoundManager instance;

    private void Awake()
    {
        if (SoundManager.instance == null)
        {
            SoundManager.instance = this;
        }
    }

    void Start()
    {
        MyAudio = this.GetComponent<AudioSource>();
    }

    public  void PlaySoundOneTime(AudioClip audioClip)
    {
        MyAudio.PlayOneShot(audioClip);
    }

    public void PlaySoundLoop()
    {
        MyAudio.clip = BGM1;
        MyAudio.Play();
        MyAudio.loop = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
