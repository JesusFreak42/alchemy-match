using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip music;
    [SerializeField] private AudioClip blockMove;
    [SerializeField] private AudioClip btnClick;
    
    private bool muted = false; //whether the game sound is muted
    [SerializeField] private Image muteImg; //the muted/unmuted UI image
    [SerializeField] private Sprite soundSprite;
    [SerializeField] private Sprite mutedSprite;

    private void Start(){
        SetMute(PlayerPrefs.GetInt("soundMuted",0) == 1); //set the muted state based on saved player pref, defaulting to unmuted if no saved pref
    }


    public void ToggleMute(){
        SetMute(!muted);
    }

    private void SetMute(bool m){
        muted = m;
        PlayerPrefs.SetInt("soundMuted", muted ? 1 : 0); //save the muted state in player prefs

        if (muted){
            muteImg.sprite = mutedSprite;
            musicSource.enabled = false;
            sfxSource.enabled = false;
        }
        else{
            muteImg.sprite = soundSprite;
            musicSource.enabled = true;
            sfxSource.enabled = true;
        }
    }

    public bool MusicIsPlaying(){
        return musicSource.isPlaying;
    }

    public bool EffectIsPlaying(){
        return sfxSource.isPlaying;
    }

    private void SetMusic(AudioClip c){
        musicSource.clip = c;
    }

    private void SetEffect(AudioClip c){
        sfxSource.clip = c;
    }

    public void PlayMusic(){
        if (muted) return;
        
        SetMusic(music);
        musicSource.Play();
    }

    public void PlayBlockMove(){
        if (muted) return;
        
        SetEffect(blockMove);
        sfxSource.Play();
    }

    public void PlayButtonClick(){
        if (muted) return;
        
        SetEffect(btnClick);
        sfxSource.Play();
    }

}
