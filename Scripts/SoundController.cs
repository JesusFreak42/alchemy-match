using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectSource;

    [SerializeField] private AudioClip music;
    [SerializeField] private AudioClip blockMove;
    [SerializeField] private AudioClip btnClick;

    public bool MusicIsPlaying(){
        return musicSource.isPlaying;
    }

    public bool EffectIsPlaying(){
        return effectSource.isPlaying;
    }

    private void SetMusic(AudioClip c){
        musicSource.clip = c;
    }

    private void SetEffect(AudioClip c){
        effectSource.clip = c;
    }

    public void PlayMusic(){
        SetMusic(music);
        musicSource.Play();
    }

    public void PlayBlockMove(){
        SetEffect(blockMove);
        effectSource.Play();
    }

    public void PlayButtonClick(){
        SetEffect(btnClick);
        effectSource.Play();
    }

}
