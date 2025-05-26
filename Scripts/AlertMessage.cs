using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertMessage : MonoBehaviour
{
    
    [SerializeField] private string msg = "";
    private RectTransform rect;

    [SerializeField] private Vector2 startLoc;
    [SerializeField] private Vector2 endLoc;
    [SerializeField] private Vector2 midLoc = new Vector2(0,0);
    [SerializeField] private float midPauseTime = 0.5f; //seconds
    [SerializeField] private float moveSpeed = 1f;

    private bool active = false;
    private bool mid = false;
    private float pauseTimer = 0f;

    private void Start(){
        rect = GetComponent<RectTransform>();
        rect.anchoredPosition = startLoc;
    }

    private void FixedUpdate(){
        if (!active) return;

        if (!mid){
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, midLoc, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(rect.anchoredPosition, midLoc) < 5f){
                mid = true;
            }
        }
        else if (pauseTimer > 0f){
            pauseTimer -= Time.deltaTime;
        }
        else{
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, endLoc, moveSpeed * Time.deltaTime);
        }
    }

    public void PlayAlert(string m){
        msg = m;
        rect.anchoredPosition = startLoc;
        active = true;
        mid = false;
        pauseTimer = midPauseTime;
    }

}
