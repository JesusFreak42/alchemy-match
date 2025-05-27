using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionsController : MonoBehaviour
{

    [SerializeField] private GameObject helpObj;
    [SerializeField] private TextMeshProUGUI helpText;
    
    [TextArea(2,10)][SerializeField] private string goldModeText = "";
    [TextArea(2,10)][SerializeField] private string matchingModeText = "";
    [TextArea(2,10)][SerializeField] private string survivalModeText = "";
    
    [SerializeField] private SoundController soundboard;

    private void Start(){
        HideHelp();
    }

    private void ShowHelp(){
        helpObj.SetActive(true);
    }

    private void HideHelp(){
        helpObj.SetActive(false);
    }

    public void CloseHelpBtn(){
        if (!soundboard.EffectIsPlaying()){
            soundboard.PlayButtonClick();
        }

        HideHelp();
    }

    public void ShowHelp(string mode){
        if (mode == "gold"){
            helpText.SetText(goldModeText);
        }
        else if (mode == "matching"){
            helpText.SetText(matchingModeText);
        }
        else if (mode == "survival"){
            helpText.SetText(survivalModeText);
        }
        else{
            helpText.SetText("");
        }

        ShowHelp();
    }

}
