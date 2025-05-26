using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    
    private int points = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private string scorePrefix = "Score: ";
    [SerializeField] private string scoreSuffix = "";
    
    private int gold = 0;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private string goldPrefix = "Gold: ";
    [SerializeField] private string goldSuffix = "";

    private void Start(){
        SetPoints(0);
    }

    private void SetScoreText(){
        scoreText.SetText(scorePrefix + points.ToString() + scoreSuffix);
    }

    public void SetScoreTextActive(bool b){
        scoreText.gameObject.SetActive(b);
    }

    public int GetPoints(){
        return points;
    }

    public void SetPoints(int p){
        points = p;
        SetScoreText();
    }

    public void AddPoints(int p){
        points += p;
        SetScoreText();
    }

    public void SubtractPoints(int p){
        points -= p;
        SetScoreText();
    }

    private void SetGoldText(){
        goldText.SetText(goldPrefix + gold.ToString() + goldSuffix);
    }

    public void SetGoldSuffix(string s){
        goldSuffix = s;
    }

    public void SetGoldTextActive(bool b){
        goldText.gameObject.SetActive(b);
    }

    public int GetGold(){
        return gold;
    }

    public void SetGold(int g){
        gold = g;
        SetGoldText();
    }

    public void AddGold(int g){
        gold += g;
        SetGoldText();
    }

    public void SubtractGold(int g){
        gold -= g;
        SetGoldText();
    }

}
