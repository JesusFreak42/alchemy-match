using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePieces : MonoBehaviour
{
    
    public static MovePieces instance;
    
    private GameController game;
    private NodePiece moving;
    private Point newIndex;
    private Vector2 mouseStart;
    [SerializeField] private int offset = 16;

    private void Awake(){
        instance = this;
    }

    private void Start(){
        game = GetComponent<GameController>();
    }

    private void Update(){
        if (moving != null){
            Vector2 dir = (Vector2) Input.mousePosition - mouseStart;
            Vector2 nDir = dir.normalized;
            Vector2 aDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            newIndex = Point.Clone(moving.index);
            Point add = Point.Zero();

            //make add either (1,0) || (-1,0) || (0,1) || (0,-1) depending on the direction of the mouse
            if (dir.magnitude > 32){ //leeway 32px of space
                if (aDir.x > aDir.y){
                    add = new Point((nDir.x > 0 ? 1 : -1), 0);
                }
                else if (aDir.y > aDir.x){
                    add = new Point(0, (nDir.y > 0 ? -1 : 1));
                }
            }
            newIndex.Add(add);

            Vector2 pos = game.GetPositionFromPoint(moving.index);
            if (!newIndex.Equals(moving.index)){
                pos += Point.Multiply(new Point(add.x, -add.y), offset).ToVector();
            }
            moving.MovePositionTo(pos);
        }
    }

    public void MovePiece(NodePiece piece){
        if (moving != null) return;

        moving = piece;
        mouseStart = Input.mousePosition;
    }

    public void DropPiece(){
        if (moving == null) return;

        // Debug.Log("dropped piece");
        if (!newIndex.Equals(moving.index)){
            game.FlipPieces(moving.index, newIndex, true);
        }
        else{
            game.ResetPiece(moving);
        }
        moving = null;
    }

}
