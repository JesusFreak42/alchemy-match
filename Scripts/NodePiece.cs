using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    
    public float size = 64f;
    public int value;
    public Point index;

    [HideInInspector] public Vector2 pos;
    public RectTransform rect;
    public Image image;

    private bool updating = false;
    [SerializeField] private float moveSpeed = 16f;

    public void Initialize(int v, Point p, float s, Sprite piece){
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        value = v;
        SetIndex(p);
        image.sprite = piece;
        size = s;
        rect.sizeDelta = new Vector2(size,size);
    }

    public void SetIndex(Point p){
        index = p;
        ResetPosition();
        UpdateName();
    }

    public void ResetPosition(){
        pos = new Vector2((size/2) + (size * index.x), -(size/2) - (size * index.y));
    }

    public void MovePosition(Vector2 move){
        rect.anchoredPosition += move * moveSpeed * Time.deltaTime;
    }

    public void MovePositionTo(Vector2 move){
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, move, moveSpeed * Time.deltaTime);
    }

    public bool UpdatePiece(){
        if (Vector3.Distance(rect.anchoredPosition, pos) > 1){
            MovePositionTo(pos);
            updating = true;
            return true;
        }
        else{
            rect.anchoredPosition = pos;
            updating = false;
            return false;
        }
    }

    public bool IsUpdating(){
        return updating;
    }

    private void UpdateName(){
        transform.name = "Node [" + index.x + "," + index.y + "]";
    }

    public void OnPointerDown(PointerEventData eventData){
        if (updating) return;
        // Debug.Log("Grab " + transform.name);
        MovePieces.instance.MovePiece(this);
    }

    public void OnPointerUp(PointerEventData eventData){
        // Debug.Log("Release " + transform.name);
        MovePieces.instance.DropPiece();
    }

}
