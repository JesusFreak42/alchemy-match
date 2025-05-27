using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    
    [Header("Game")]
    private bool gameActive = false;
    public string gameMode = "gold";
    [SerializeField] private int matchNum = 3;
    [SerializeField] private int boardHeight = 14;
    [SerializeField] private int boardWidth = 9;
    private float nodeSize = 64f;
    private int[] fills;
    [SerializeField] private float goldGameSeconds = 180f;
    private int gold = 0;
    private int curGoldOnBoard = 0;
    [SerializeField] private int maxGoldOnBoard = 4;
    [SerializeField] private int goldObjective = 5;
    [SerializeField] private float matchingGameSeconds = 180f;
    [SerializeField] private float survivalGameSeconds = 60f;
    [SerializeField] private float survivalSecondsPerGold = 20f;
    private float gameTimer;
    [SerializeField] AlertMessage goldAlert;

    public ArrayLayout boardLayout;
    private Node[,] board;
    private System.Random dice;

    private List<NodePiece> piecesToUpdate;
    private List<FlippedPieces> flippedPieces;
    private List<NodePiece> deadPieces;
    private List<KilledPiece> killedPieces;
    
    [Header("UI Elements")]
    [SerializeField] private RectTransform boardContainer;
    [SerializeField] private RectTransform gameBoard;
    [SerializeField] private RectTransform killedBoard;
    [SerializeField] private RectTransform boardShield;
    [SerializeField] private List<NodeItem> pieces;
    private Sprite[] piecesSprites;
    [SerializeField] private ScoreController scoreboard;
    [SerializeField] private ProgressBar gameTimeBar;
    [SerializeField] private TextMeshProUGUI gameTimeText;
    [SerializeField] private TextMeshProUGUI winGameText;
    [SerializeField] private TextMeshProUGUI loseGameText;
    [SerializeField] private SoundController soundboard;

    [Header("Prefabs")]
    [SerializeField] GameObject nodePiece;
    [SerializeField] GameObject killedPiece;

    private void Start(){
        // StartGame();
        soundboard.PlayMusic();
    }

    private void Update(){
        if (!gameActive) return;

        if (gold >= goldObjective && gameMode != "survival"){
            EndGame(true);
        }

        SetGameTime(gameTimer -= Time.deltaTime);
        if (gameTimer <= 0f){
            EndGame(false);
        }
        
        List<NodePiece> finishedUpdating = new List<NodePiece>();
        if (piecesToUpdate.Count > 0 && !soundboard.EffectIsPlaying()){
            soundboard.PlayBlockMove();
        }
        for (int i = 0; i < piecesToUpdate.Count; i++){
            if (!piecesToUpdate[i].UpdatePiece()){
                finishedUpdating.Add(piecesToUpdate[i]);
            }
        }

        for (int i = 0; i < finishedUpdating.Count; i++){
            NodePiece piece = finishedUpdating[i];
            FlippedPieces flip = GetFlipped(piece);
            NodePiece flippedPiece = null;

            int x = (int) piece.index.x;
            fills[x] = Mathf.Clamp(fills[x] - 1, 0, boardWidth);

            List<Point> connected = IsConnected(piece.index, true);
            bool wasFlipped = flip != null;

            if (wasFlipped){ //if we flipped to make this update
                flippedPiece = flip.GetOtherPiece(piece);
                AddPoints(ref connected, IsConnected(flippedPiece.index, true));
            }

            if (connected.Count == 0){ //if no match, then flip it back
                if (wasFlipped){ //if we flipped
                    FlipPieces(piece.index, flippedPiece.index, false); //flip back
                }
            }
            else{ //if we made a match
                // Debug.Log("connected: " + connected.Count);
                scoreboard.AddPoints(connected.Count * 10);
                
                foreach(Point pnt in connected){ //remove the node pieces connected
                    KillPiece(pnt);
                    Node node = GetNodeAtPoint(pnt);
                    NodePiece nodePiece = node.GetPiece();
                    if (nodePiece != null){
                        nodePiece.gameObject.SetActive(false);
                        deadPieces.Add(nodePiece);
                    }
                    node.SetPiece(null);
                }

                ApplyGravityToBoard();
            }

            Point bPnt = new Point(piece.index.x, boardHeight-1);
            if (piece.index.y == (boardHeight - 1) && pieces[GetValueAtPoint(bPnt)-1].killAtBottom){ //if this piece is at the bottom and it dies at the bottom, kill it
                if (pieces[GetValueAtPoint(bPnt)-1].name == "Gold"){
                    gold++;
                    scoreboard.AddGold(1);
                    curGoldOnBoard--;
                    if (gold < goldObjective){
                        goldAlert.PlayAlert("Gold!");
                    }

                    if (gameMode == "matching"){
                        scoreboard.AddPoints(3000);
                    }
                    else if (gameMode == "survival"){
                        SetGameTime(gameTimer + survivalSecondsPerGold);
                    }
                }

                KillPiece(bPnt);
                Node node = GetNodeAtPoint(bPnt);
                // NodePiece nodePiece = node.GetPiece();
                if (piece != null){
                    piece.gameObject.SetActive(false);
                    deadPieces.Add(piece);
                }
                node.SetPiece(null);
            }
            ApplyGravityToBoard();
            
            flippedPieces.Remove(flip); //remove the flip after update
            piecesToUpdate.Remove(piece);
        }
    }

    private void ApplyGravityToBoard(){
        for (int x = 0; x < boardWidth; x++){
            for (int y = (boardHeight-1); y >= 0; y--){
                Point p = new Point(x,y);
                Node node = GetNodeAtPoint(p);
                int val = GetValueAtPoint(p);
                if (val != 0) continue; //if it's not a hole, do nothing

                for (int ny = (y-1); ny >= -1; ny--){
                    Point next = new Point(x,ny);
                    int nextVal = GetValueAtPoint(next);
                    if (nextVal == 0) continue;
                    if (nextVal != -1){ //if we did not hit an end, but it's not 0, then use this to fill the current hole
                        Node got = GetNodeAtPoint(next);
                        NodePiece piece = got.GetPiece();

                        //set the hole
                        node.SetPiece(piece);
                        piecesToUpdate.Add(piece);

                        //replace the hole
                        got.SetPiece(null);
                    }
                    else{ //hit an end
                        //fill in the hole using dead peices or new ones
                        int newVal = FillPiece(false);
                        NodePiece piece;
                        Point fallPoint = new Point(x, -1 - fills[x]);

                        if (pieces[newVal - 1].name == "Gold"){
                            curGoldOnBoard++;
                        }

                        if (deadPieces.Count > 0){
                            NodePiece revived = deadPieces[0];
                            revived.gameObject.SetActive(true);
                            piece = revived;
                            deadPieces.RemoveAt(0);
                        }
                        else{
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            piece = n;
                        }
                        
                        piece.Initialize(newVal, p, nodeSize, piecesSprites[newVal - 1]);
                        piece.rect.anchoredPosition = GetPositionFromPoint(fallPoint);

                        Node hole = GetNodeAtPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                        fills[x]++;
                    }
                    break;
                }
            }
        }
    }

    private FlippedPieces GetFlipped(NodePiece p){
        FlippedPieces flip = null;
        for (int i = 0; i < flippedPieces.Count; i++){
            if (flippedPieces[i].GetOtherPiece(p) != null){
                flip = flippedPieces[i];
                break;
            }
        }
        return flip;
    }

    public void StartGoldGame(){
        gameMode = "gold";
        SetGameTime(goldGameSeconds);
        scoreboard.SetScoreTextActive(false);
        scoreboard.SetGoldTextActive(true);
        scoreboard.SetGoldSuffix(" / " + goldObjective);
        StartGame();
    }

    public void StartMatchingGame(){
        gameMode = "matching";
        SetGameTime(matchingGameSeconds);
        scoreboard.SetScoreTextActive(true);
        scoreboard.SetGoldTextActive(false);
        StartGame();
    }

    public void StartSurvivalGame(){
        gameMode = "survival";
        SetGameTime(survivalGameSeconds);
        scoreboard.SetScoreTextActive(false);
        scoreboard.SetGoldTextActive(true);
        scoreboard.SetGoldSuffix("");
        StartGame();
    }

    public void ResetGame(){
        if (!soundboard.EffectIsPlaying()){
            soundboard.PlayButtonClick();
        }

        curGoldOnBoard = 0;
        if (gameMode == "gold"){
            StartGoldGame();
        }
        else if (gameMode == "matching"){
            StartMatchingGame();
        }
        else if (gameMode == "survival"){
            StartSurvivalGame();
        }
    }

    public void StartGame(){
        foreach (Transform child in gameBoard.transform) {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in killedBoard.transform) {
            GameObject.Destroy(child.gameObject);
        }

        fills = new int[boardWidth];
        dice = new System.Random(GetRandomSeed().GetHashCode());
        piecesToUpdate = new List<NodePiece>();
        flippedPieces = new List<FlippedPieces>();
        deadPieces = new List<NodePiece>();
        killedPieces = new List<KilledPiece>();

        piecesSprites = new Sprite[pieces.Count];
        for (int i = 0; i < pieces.Count; i++){
            piecesSprites[i] = pieces[i].sprite;
        }

        nodeSize = gameBoard.GetComponent<RectTransform>().rect.width / boardWidth;
        gameBoard.sizeDelta = new Vector2(gameBoard.sizeDelta.x, nodeSize * boardHeight);
        killedBoard.sizeDelta = new Vector2(gameBoard.sizeDelta.x, nodeSize * boardHeight);
        boardShield.sizeDelta = new Vector2(gameBoard.sizeDelta.x, nodeSize * boardHeight);

        if (gameBoard.sizeDelta.y > boardContainer.sizeDelta.y){
            nodeSize = boardContainer.sizeDelta.y / boardHeight;
            gameBoard.sizeDelta = new Vector2(nodeSize * boardWidth, boardContainer.sizeDelta.y);
            killedBoard.sizeDelta = new Vector2(nodeSize * boardWidth, boardContainer.sizeDelta.y);
            boardShield.sizeDelta = new Vector2(nodeSize * boardWidth, boardContainer.sizeDelta.y);
        }

        InitializeBoard();
        VerifyBoard();
        CreateBoard();
        gold = 0;
        scoreboard.SetPoints(0);
        scoreboard.SetGold(0);
        boardShield.gameObject.SetActive(false);
        winGameText.gameObject.SetActive(false);
        loseGameText.gameObject.SetActive(false);
        gameActive = true;
    }

    public void EndGame(bool win){
        gameActive = false;
        boardShield.gameObject.SetActive(true);

        if (win){
            winGameText.gameObject.SetActive(true);
        }
        else{
            loseGameText.gameObject.SetActive(true);
        }
    }

    private void InitializeBoard(){
        board = new Node[boardWidth, boardHeight];

        for (int y = 0; y < boardHeight; y++){
            for (int x = 0; x < boardWidth; x++){
                board[x,y] = new Node(boardLayout.rows[y].row[x] ? -1 : FillPiece(true), new Point(x,y));
            }
        }
    }

    private void VerifyBoard(){
        List<int> remove;

        List<int> unalive = new List<int>(); //don't refill points with pieces who don't spawn at start
        for (int i = 0; i < pieces.Count; i++){
            if (!pieces[i].aliveOnStart){
                unalive.Add(i+1);
            }
        }

        for (int x = 0; x < boardWidth; x++){
            for (int y = 0; y < boardHeight; y++){
                Point p = new Point(x,y);
                int val = GetValueAtPoint(p);

                if (val <= 0) continue;

                remove = new List<int>(unalive);
                while (IsConnected(p, true).Count > 0){
                    val = GetValueAtPoint(p);
                    if (!remove.Contains(val)){
                        remove.Add(val);
                    }
                    SetValueAtPoint(p, NewValue(ref remove));
                }
            }
        }
    }

    private void SetGameTime(float t){
        gameTimer = t;

        if (gameMode == "gold"){
            if (gameTimer > goldGameSeconds) gameTimer = goldGameSeconds;
            gameTimeBar.SetValue(gameTimer / goldGameSeconds);
        }
        else if (gameMode == "matching"){
            if (gameTimer > matchingGameSeconds) gameTimer = matchingGameSeconds;
            gameTimeBar.SetValue(gameTimer / matchingGameSeconds);
        }
        else if (gameMode == "survival"){
            if (gameTimer > survivalGameSeconds) gameTimer = survivalGameSeconds;
            gameTimeBar.SetValue(gameTimer / survivalGameSeconds);
        }
        
        if (gameTimer < 0f) gameTimer = 0f;

        int minutes = Mathf.FloorToInt(gameTimer / 60f);
        int seconds = Mathf.FloorToInt(gameTimer - minutes * 60);
        gameTimeText.SetText(string.Format("{0:0}:{1:00}", minutes, seconds));
    }

    private void CreateBoard(){
        for (int x = 0; x < boardWidth; x++){
            for (int y = 0; y < boardHeight; y++){
                Node node = GetNodeAtPoint(new Point(x,y));
                int val = node.value;
                if (val <= 0) continue;

                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2((nodeSize/2) + (nodeSize * x), -(nodeSize/2) - (nodeSize * y));
                piece.Initialize(val, new Point(x,y), nodeSize, piecesSprites[val-1]);
                node.SetPiece(piece);
            }
        }
    }

    public void ResetPiece(NodePiece piece){
        piece.ResetPosition();
        piecesToUpdate.Add(piece);
    }

    public void FlipPieces(Point one, Point two, bool main){
        if (GetValueAtPoint(one) < 0) return;

        Node nodeOne = GetNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.GetPiece();
        
        if (GetValueAtPoint(two) > 0){
            Node nodeTwo = GetNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.GetPiece();
            nodeOne.SetPiece(pieceTwo);
            nodeTwo.SetPiece(pieceOne);

            if (main){ //if main then flip them
                flippedPieces.Add(new FlippedPieces(pieceOne, pieceTwo));
            }
            soundboard.PlayBlockMove();

            piecesToUpdate.Add(pieceOne);
            piecesToUpdate.Add(pieceTwo);
        }
        else{
            ResetPiece(pieceOne);
        }
    }

    private void KillPiece(Point p){
        List<KilledPiece> available = new List<KilledPiece>();
        for (int i = 0; i < killedPieces.Count; i++){
            if (!killedPieces[i].falling){
                available.Add(killedPieces[i]);
            }
        }

        KilledPiece set = null;
        if (available.Count > 0){
            set = available[0];
        }
        else{
            GameObject kill = GameObject.Instantiate(killedPiece, killedBoard);
            KilledPiece kPiece = kill.GetComponent<KilledPiece>();
            set = kPiece;
            killedPieces.Add(kPiece);
        }

        int val = GetValueAtPoint(p) - 1;
        if (set != null && val >= 0 && val < piecesSprites.Length){
            set.Initialize(piecesSprites[val], nodeSize, GetPositionFromPoint(p));
        }
    }

    private List<Point> IsConnected(Point p, bool main){
        List<Point> connected = new List<Point>();
        int val = GetValueAtPoint(p);
        if (!pieces[val-1].matchable) return connected; //if this piece is unmatchable, return empty connected (no connections)

        Point[] directions = {
            Point.Up(),
            Point.Right(),
            Point.Down(),
            Point.Left()
        };

        foreach(Point dir in directions){ //checking if there are 2 or more shapes in the directions
            List<Point> line = new List<Point>();

            int same = 0;
            for (int i = 1; i < matchNum; i++){
                Point check = Point.Add(p, Point.Multiply(dir, i));

                if (GetValueAtPoint(check) == val){
                    line.Add(check);
                    same++;
                }
            }

            if (same > 1){ //if there are more than 1 of the same shape in the direction, then we know it is a match
                AddPoints(ref connected, line); //add these points to the overarching connected list
            }
        }

        for (int i = 0; i < 2; i++){ //checking if we are in the middle of two of the same shape
            List<Point> line = new List<Point>();
            int same = 0;
            Point[] check = {
                Point.Add(p, directions[i]),
                Point.Add(p, directions[i+2])
            };

            foreach(Point next in check){ //Check both sides of the piece. If they are the same value, add them to the list.
                if (GetValueAtPoint(next) == val){
                    line.Add(next);
                    same++;
                }
            }

            if (same > 1){
                AddPoints(ref connected, line);
            }
        }

        for (int i = 0; i < 4; i++){ //check for a 2x2
            List<Point> square = new List<Point>();
            int same = 0;
            int next = i + 1;
            if (next >= 4){
                next -= 4;
            }

            Point[] check = {
                Point.Add(p, directions[i]),
                Point.Add(p, directions[next]),
                Point.Add(p, Point.Add(directions[i], directions[next]))
            };

            foreach(Point n in check){ //Check all sides of the piece. If they are the same value, add them to the list.
                if (GetValueAtPoint(n) == val){
                    square.Add(n);
                    same++;
                }
            }

            if (same > 2){
                AddPoints(ref connected, square);
            }
        }

        if (main){ //checks for other matches along the current match
            for (int i = 0; i < connected.Count; i++){
                AddPoints(ref connected, IsConnected(connected[i], false));
            }
        }

        return connected;
    } //end IsConnected

    private void AddPoints(ref List<Point> points, List<Point> add){
        foreach(Point p in add){
            bool doAdd = true;
            for (int i = 0; i < points.Count; i++){
                if (points[i].Equals(p)){
                    doAdd = false;
                    break;
                }
            }

            if (doAdd){
                points.Add(p);
            }
        }
    }

    private int FillPiece(bool start){ //where start is the initial board population
        int val = 1;
        val = (dice.Next(0,(piecesSprites.Length * 20)) / ((piecesSprites.Length * 20) / piecesSprites.Length)) + 1;
        while (Random.Range(0f,1f) > pieces[val-1].chanceOfUsing || (!pieces[val-1].aliveOnStart && start) || (pieces[val-1].name == "Gold" && curGoldOnBoard >= maxGoldOnBoard)){
            val = (dice.Next(0,(piecesSprites.Length * 20)) / ((piecesSprites.Length * 20) / piecesSprites.Length)) + 1;
        }
        return val;
    }

    private int GetValueAtPoint(Point p){
        if (p.x < 0 || p.x >= boardWidth || p.y < 0 || p.y >= boardHeight) return -1;

        return board[p.x, p.y].value;
    }

    private void SetValueAtPoint(Point p, int v){
        board[p.x, p.y].value = v;
    }

    private Node GetNodeAtPoint(Point p){
        return board[p.x, p.y];
    }

    private int NewValue(ref List<int> remove){
        List<int> available = new List<int>();
        for (int i = 0; i < piecesSprites.Length; i++){
            available.Add(i + 1);
        }

        foreach (int i in remove){
            available.Remove(i);
        }

        if (available.Count <= 0) return 0;
        return available[dice.Next(0, available.Count)];
    }

    private string GetRandomSeed(){
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";

        for (int i = 0; i < 20; i++){
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        }

        return seed;
    }

    public Vector2 GetPositionFromPoint(Point p){
        return new Vector2((nodeSize/2) + (nodeSize * p.x), -(nodeSize/2) - (nodeSize * p.y));
    }

}

[System.Serializable]
public class Node{
    public int value; //0 = blank, 1 = capsule, 2 = circle, 3 = diamond, 4 = square, 5 = triangle, -1 = hole
    public Point index;
    private NodePiece piece;

    public Node(int v, Point i){
        value = v;
        index = i;
    }

    public void SetPiece(NodePiece p){
        piece = p;
        value = piece == null ? 0 : piece.value;

        if (piece == null) return;

        piece.SetIndex(index);
    }

    public NodePiece GetPiece(){
        return piece;
    }
}

[System.Serializable]
public class FlippedPieces{
    public NodePiece one;
    public NodePiece two;

    public FlippedPieces(NodePiece o, NodePiece t){
        one = o;
        two = t;
    }

    public NodePiece GetOtherPiece(NodePiece p){
        if (p == one){
            return two;
        }
        else if (p == two){
            return one;
        }
        else{
            return null;
        }
    }
}
