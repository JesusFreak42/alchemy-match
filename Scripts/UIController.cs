using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private ParticleSystem[] menuEffect;

    [SerializeField] private GameController gameController;
    [SerializeField] private SoundController soundboard;
    [SerializeField] private InstructionsController instructions;

    private void Start(){
        ShowMainMenuScreen();
    }

    public void ShowMainMenuScreen(){
        gameScreen.SetActive(false);
        mainMenuScreen.SetActive(true);

        foreach (ParticleSystem effect in menuEffect){
            effect.gameObject.SetActive(true);
            // effect.Play();
        }
    }

    public void ShowGameScreen(){
        mainMenuScreen.SetActive(false);
        gameScreen.SetActive(true);
        
        foreach (ParticleSystem effect in menuEffect){
            effect.gameObject.SetActive(false);
            // effect.Stop();
        }
    }

    public void GameToMainMenu(){
        if (!soundboard.EffectIsPlaying()){
            soundboard.PlayButtonClick();
        }
        gameController.EndGame(false);
        ShowMainMenuScreen();
    }

    public void StartGoldGame(){
        if (!soundboard.EffectIsPlaying()){
            soundboard.PlayButtonClick();
        }
        ShowGameScreen();
        gameController.StartGoldGame();
    }

    public void StartMatchingGame(){
        if (!soundboard.EffectIsPlaying()){
            soundboard.PlayButtonClick();
        }
        ShowGameScreen();
        gameController.StartMatchingGame();
    }

    public void StartSurvivalGame(){
        if (!soundboard.EffectIsPlaying()){
            soundboard.PlayButtonClick();
        }
        ShowGameScreen();
        gameController.StartSurvivalGame();
    }

    public void ShowHelpScreen(){
        if (!soundboard.EffectIsPlaying()){
            soundboard.PlayButtonClick();
        }
        instructions.ShowHelp(gameController.gameMode);
    }

}
