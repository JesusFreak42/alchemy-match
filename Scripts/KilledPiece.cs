using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KilledPiece : MonoBehaviour
{

    [HideInInspector] public bool falling;
    private float speed = 16f;
    [SerializeField] private float gravity = 32f;
    private Vector2 moveDir;
    private RectTransform rect;
    private Image image;
    private float size = 64f;

    public void Initialize(Sprite piece, float s, Vector2 start){
        falling = true;
        moveDir = Vector2.up;
        moveDir.x = Random.Range(-1f, 1f);
        moveDir *= speed / 2;

        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        image.sprite = piece;
        rect.anchoredPosition = start;
        size = s;
        rect.sizeDelta = new Vector2(size,size);
    }

    private void Update(){
        if (!falling) return;

        moveDir.y -= gravity * Time.deltaTime;
        moveDir.x = Mathf.Lerp(moveDir.x, 0, Time.deltaTime);
        rect.anchoredPosition += moveDir * speed * Time.deltaTime;

        if (rect.position.x < -(size) || rect.position.x > Screen.width + size || rect.position.y < -(size) || rect.position.y > Screen.height + size){
            falling = false;
        }
    }

}
