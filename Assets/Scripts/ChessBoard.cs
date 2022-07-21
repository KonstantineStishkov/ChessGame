using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SpecialMove
{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}

public enum Difficulty
{
    None = 0,
    Easy,
    Medium,
    Hard
}

public class ChessBoard : MonoBehaviour
{
    [Header("Art Stuff")]
    [SerializeField] private Material[] tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deadSize = 0.3f;
    [SerializeField] private float aliveSize = 0.7f;
    [SerializeField] private float deadSpacing = 0.3f;
    [SerializeField] private float dragOffset = 1.3f;
    [SerializeField] private GameObject ModalWindow;
    [SerializeField] private Logger gameLog;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    private ChessPiece[,] chessPieces;
    private ChessPiece currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private bool isWhiteTurn;
    private bool isGameStarted = false;
    private bool isAITurn = false;
    private Difficulty ai;
    private SpecialMove specialMove;
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();

    //Multi-player logic
    private int playerCount = -1;
    private int currentTeam = -1;

    public void Awake()
    {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
        RegisterEvents();
    }
    public void StartGame(Difficulty difficulty)
    {
        ai = difficulty;
        isWhiteTurn = true;
        isGameStarted = true;

        Debug.Log("Chess Board Awakened");
        gameLog.AddLine("Game Started");
    }
    private void Update()
    {
        if (!isGameStarted || isAITurn)
        {
            return;
        }

        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        if(isWhiteTurn && ai != Difficulty.None)
        {
            isAITurn = true;
            EnemyTurn(ai);
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            // Get the indexes of the title I've hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                SetMaterialToTile("Hover", tiles[hitPosition.x, hitPosition.y]);
                //tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            if (currentHover != hitPosition)
            {
                if (ContainsValidMove(ref availableMoves, currentHover))
                    SetMaterialToTile("Highlight", tiles[currentHover.x, currentHover.y]);
                else
                    SetMaterialToTile("Tile", tiles[currentHover.x, currentHover.y]);

                currentHover = hitPosition;
                SetMaterialToTile("Hover", tiles[hitPosition.x, hitPosition.y]);

                //tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                //currentHover = hitPosition;
                //tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // Pressing down mouse button
            if (Input.GetMouseButtonDown(0))
            {
                if (chessPieces[hitPosition.x, hitPosition.y] != null)
                {
                    if ((chessPieces[hitPosition.x, hitPosition.y].team == 0 && isWhiteTurn && ai == Difficulty.None)
                     || (chessPieces[hitPosition.x, hitPosition.y].team == 1 && !isWhiteTurn)) //is it my turn?
                    {
                        currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];

                        //Get list of available moves
                        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);

                        // Get list of special moves
                        specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves);

                        PreventCheck(currentlyDragging);
                        HighlightTiles("Highlight");
                    }
                }
            }

            // Releasing mouse button
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                if (!validMove)
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));

                HighlightTiles();
                currentlyDragging = null;
            }
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                HighlightTiles();
                currentlyDragging = null;
            }
        }

        //if we're dragging a piece
        if (currentlyDragging)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance))
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
        }
    }

    //Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;

        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];

        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial[0];

        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    //Positioning
    private void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    PositionSinglePiece(x, y, true);
    }
    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    //Spawning the pieces
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        //White team
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        }

        //Black team
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        }
    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();

        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];
        cp.SetScale(Vector3.one * aliveSize);

        return cp;
    }

    //Highlight Tiles
    private void HighlightTiles(string mask = "Tile")
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            SetMaterialToTile(mask, tiles[availableMoves[i].x, availableMoves[i].y]);
        }

        if (mask == "Tile")
            availableMoves.Clear();
    }

    private void SetMaterialToTile(string mask, GameObject tile)
    {
        Material material = GetMaterialByName(mask);

        tile.GetComponent<MeshRenderer>().material = material;
        tile.layer = LayerMask.NameToLayer(mask);
    }

    private Material GetMaterialByName(string mask)
    {
        Material material;
        switch (mask)
        {
            case "Tile":
                material = tileMaterial[0];
                break;
            case "Hover":
                material = tileMaterial[1];
                break;
            case "Highlight":
                material = tileMaterial[2];
                break;
            default:
                material = tileMaterial[0];
                break;
        }

        return material;
    }

    // Checkmate
    private void CheckMate(int team)
    {
        ai = Difficulty.None;
        DisplayVictory(team);
    }

    private void DisplayVictory(int winningTeam)
    {
        ModalWindow.SetActive(true);
        //victoryScreen.SetActive(true);
        //victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }

    public void OnResetButton()
    {
        ClearUI();
        FieldsReset();
        ClearChessPieces();

        SpawnAllPieces();
        PositionAllPieces();
        isWhiteTurn = true;
    }
    public void OnExitButton()
    {
        Application.Quit();
    }

    // Special Moves
    private void ProcessSpecialMove()
    {
        if (specialMove == SpecialMove.EnPassant)
            ProcessEnPassant();

        if (specialMove == SpecialMove.Castling)
            ProcessCastling();

        if (specialMove == SpecialMove.Promotion)
        {
            var lastMove = moveList[moveList.Count - 1][1];
            ChessPiece targetPawn = chessPieces[lastMove.x, lastMove.y];

            if ((targetPawn.team == 0 && lastMove.y == 7) || (targetPawn.team == 1 && lastMove.y == 0))
            {
                ChessPiece newQueen = SpawnSinglePiece(ChessPieceType.Queen, targetPawn.team);
                newQueen.transform.position = chessPieces[lastMove.x, lastMove.y].transform.position;
                Destroy(targetPawn.gameObject);
                chessPieces[lastMove.x, lastMove.y] = newQueen;
                PositionSinglePiece(lastMove.x, lastMove.y);
            }
        }
    }
    private void ProcessEnPassant()
    {
        Vector2Int[] newMove = moveList[moveList.Count - 1];
        ChessPiece myPawn = chessPieces[newMove[1].x, newMove[1].y];
        Vector2Int[] targetPawnPosition = moveList[moveList.Count - 2];
        ChessPiece enemyPawn = chessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y];

        if ((myPawn.currentX == enemyPawn.currentX)
        && (myPawn.currentY == enemyPawn.currentY - 1 || myPawn.currentY == enemyPawn.currentY + 1))
        {
            KillChessPiece(enemyPawn);
            chessPieces[enemyPawn.currentX, enemyPawn.currentY] = null;
        }
    }
    private void ProcessCastling()
    {
        Vector2Int lastMove = moveList[moveList.Count - 1][1];

        // Left Rook
        if (lastMove.x == 2
            && (lastMove.y == 0 || lastMove.y == 7))
        {
            PlaceRookDuringCastling(lastMove.y, 0, 3);
        }

        // Right Rook
        if (lastMove.x == 6
            && (lastMove.y == 0 || lastMove.y == 7))
        {
            PlaceRookDuringCastling(lastMove.y, 7, 5);
        }
    }
    private void PlaceRookDuringCastling(int sideY, int rookOldX, int rookNewX)
    {
        ChessPiece rook = chessPieces[rookOldX, sideY];
        chessPieces[rookNewX, sideY] = rook;
        PositionSinglePiece(rookNewX, sideY);
        chessPieces[rookOldX, sideY] = null;
    }
    private void PreventCheck(ChessPiece currentPiece)
    {
        ChessPiece targetKing = null;
        foreach (ChessPiece cp in chessPieces)
            if (cp != null && cp.type == ChessPieceType.King && cp.team == currentPiece.team)
            {
                targetKing = cp;
                break;
            }

        // Since we're sending ref availableMoves, we will be deleting elements
        SimulateMoveForPiece(currentPiece, ref availableMoves, targetKing);
    }
    private bool CheckForCheckMate()
    {
        if(moveList.Count == 0)
        {
            return false;
        }

        var lastMove = moveList[moveList.Count - 1];
        int targetTeam = (chessPieces[lastMove[1].x, lastMove[1].y].team == 0) ? 1 : 0;

        List<ChessPiece> attackingPieces = new List<ChessPiece>();
        List<ChessPiece> deffendingPieces = new List<ChessPiece>();

        ChessPiece targetKing = null;
        foreach (ChessPiece cp in chessPieces)
        {
            if (cp != null)
            {
                if (cp.team == targetTeam)
                {
                    deffendingPieces.Add(cp);
                    if(cp.type == ChessPieceType.King)
                    {
                        targetKing = cp;
                    }
                }
                else
                {
                    attackingPieces.Add(cp);
                }
            }
        }

        // Is the king attacked right now
        List<Vector2Int> currentAvailableMoves = GetSimulatedMoves(ref chessPieces, attackingPieces);

        // Are we in Check right now?
        if (ContainsValidMove(ref currentAvailableMoves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
        {
            // King is under attack> can we move something to help him?
            foreach(var piece in deffendingPieces)
            {
                List<Vector2Int> defendingMoves = piece.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                SimulateMoveForPiece(piece, ref defendingMoves, targetKing);

                if (defendingMoves.Count != 0)
                    return false;
            }

            return true; // Checkmate exit
        }

        return false;
    }
    private void SimulateMoveForPiece(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece targetKing)
    {
        // Save the current values to reset after
        int actualX = cp.currentX;
        int actualY = cp.currentY;
        List<Vector2Int> movesToRemove = new List<Vector2Int>();

        // Going through all the moves, simulate and check
        foreach (var move in moves)
        {
            int simX = move.x;
            int simY = move.y;

            Vector2Int kingPosition = new Vector2Int(targetKing.currentX, targetKing.currentY);

            // Is it King's move
            if (cp.type == ChessPieceType.King)
                kingPosition = new Vector2Int(simX, simY);

            ChessPiece[,] simulation;
            List<ChessPiece> simAttackingPieces;
            MakeSimulationBoard(cp, out simulation, out simAttackingPieces);

            //Simulate that move
            simulation[actualX, actualY] = null;
            cp.currentX = simX;
            cp.currentY = simY;
            simulation[simX, simY] = cp;

            //Did one of the pice got taking down during simulation
            ChessPiece deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY);
            if (deadPiece != null)
                simAttackingPieces.Remove(deadPiece);

            List<Vector2Int> simMoves = GetSimulatedMoves(ref simulation, simAttackingPieces);

            // Is the king in trouble? If som remove the move
            if (ContainsValidMove(ref simMoves, kingPosition))
            {
                movesToRemove.Add(move);
            }

            // Restore actual cp data
            cp.currentX = actualX;
            cp.currentY = actualY;
        }

        // Remove bad moves
        foreach (var move in movesToRemove)
            moves.Remove(move);
    }
    private List<Vector2Int> GetSimulatedMoves(ref ChessPiece[,] simulation, List<ChessPiece> simAttackingPieces)
    {
        List<Vector2Int> simMoves = new List<Vector2Int>();
        foreach (var a in simAttackingPieces)
        {
            List<Vector2Int> attackMoves = a.GetAvailableMoves(ref simulation, TILE_COUNT_X, TILE_COUNT_Y);
            foreach (var b in attackMoves)
                simMoves.Add(b);
        }

        return simMoves;
    }
    private void MakeSimulationBoard(ChessPiece cp, out ChessPiece[,] simulation, out List<ChessPiece> simAttackingPieces)
    {
        simulation = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
        simAttackingPieces = new List<ChessPiece>();
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    simulation[x, y] = chessPieces[x, y];
                    if (simulation[x, y].team != cp.team)
                        simAttackingPieces.Add(simulation[x, y]);
                }

            }
        }
    }

    // AI
    private void EnemyTurn(Difficulty difficulty)
    {
        Vector2Int targetPosition;
        ChessPiece cp;

        if (CheckForCheckMate())
            CheckMate(0);

        switch (difficulty)
        {
            case Difficulty.Easy:
                cp = GetBestEasyMove(out targetPosition);
                break;
            case Difficulty.Medium:
                cp = GetBestMediumMove(out targetPosition);
                break;
            case Difficulty.Hard:
                cp = GetBestHardMove(out targetPosition);
                break;
            default:
                cp = GetBestEasyMove(out targetPosition);
                break;
        }

        Debug.Log(targetPosition.x + ":" + targetPosition.y);
        MoveTo(cp, targetPosition.x, targetPosition.y);

        availableMoves.Clear();

        isAITurn = false;
    }

    private ChessPiece GetBestEasyMove(out Vector2Int targetPosition)
    {
        int value = -1;
        bool isValid = false;
        ChessPiece cp = null;
        targetPosition = new Vector2Int();
        List<ChessPiece> availablePieces = new List<ChessPiece>();

        foreach(ChessPiece piece in chessPieces)
        {
            if(piece == null || piece.team != 0)
            {
                continue;
            }

            availablePieces.Add(piece);
            availableMoves = piece.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
            PreventCheck(piece);

            if(availableMoves.Count == 0)
            {
                continue;
            }

            foreach(Vector2Int move in availableMoves)
            {
                if(chessPieces[move.x, move.y] != null && chessPieces[move.x, move.y].team != 0 && chessPieces[move.x, move.y].Value > value)
                {
                    value = chessPieces[move.x, move.y].Value;
                    cp = piece;
                    targetPosition = move;
                    isValid = true;
                }
            }
        }

        while(!isValid)
        {
            int rnd = Random.Range(0, availablePieces.Count - 1);
            Debug.LogFormat("List {0}, number {1}", availablePieces.Count, rnd);
            cp = availablePieces[rnd];
            availableMoves = cp.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
            PreventCheck(cp);
            if (availableMoves.Count == 0)
            {
                continue;
            }
            rnd = Random.Range(0, availableMoves.Count - 1);
            Debug.LogFormat("List {0}, number {1}", availableMoves.Count, rnd);
            targetPosition = availableMoves[rnd];
            isValid = true;
        }

        return cp;
    }

    private ChessPiece GetBestMediumMove(out Vector2Int targetPosition)
    {
        targetPosition = new Vector2Int();
        return new ChessPiece();
    }

    private ChessPiece GetBestHardMove(out Vector2Int targetPosition)
    {
        targetPosition = new Vector2Int();
        return new ChessPiece();
    }

    //Operations
    private Vector2Int LookupTileIndex(GameObject hitinfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (tiles[x, y] == hitinfo)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return -Vector2Int.one;
    }
    private bool MoveTo(ChessPiece cp, int x, int y)
    {
        Debug.LogFormat("Conatins {0}", availableMoves.Count);

        if(availableMoves.Count == 0)
        {
            if (CheckForCheckMate())
                CheckMate(cp.team);
        }

        if (!ContainsValidMove(ref availableMoves, new Vector2(x, y)) && !isAITurn)
            return false;

        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        //Is there another piece?
        if (chessPieces[x, y] != null)
        {
            ChessPiece otherChessPiece = chessPieces[x, y];
            Debug.LogFormat("My Piece team {0}, Other Piece Team{1}", cp.team, otherChessPiece.team);

            if (otherChessPiece.team == cp.team)
                return false;

            //If it is the enemy team
            KillChessPiece(otherChessPiece);
        }

        chessPieces[x, y] = cp;
        chessPieces[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x, y);
        gameLog.LogTurn(cp.team, cp.type, new Vector2Int(previousPosition.x, previousPosition.y), new Vector2Int(x, y));

        isWhiteTurn = !isWhiteTurn;
        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) });

        ProcessSpecialMove();

        if (CheckForCheckMate())
            CheckMate(cp.team);

        return true;
    }

    private void KillChessPiece(ChessPiece cp)
    {
        if (cp.team == 0)
        {
            if (cp.type == ChessPieceType.King)
                CheckMate(1);

            deadWhites.Add(cp);
            cp.SetScale(Vector3.one * deadSize);
            cp.SetPosition(new Vector3(8 * tileSize, yOffset, -1 * tileSize)
                - bounds
                + new Vector3(tileSize / 2, 0, tileSize / 2)
                + (Vector3.forward * deadSpacing) * deadWhites.Count);
        }

        if (cp.team == 1)
        {
            deadBlacks.Add(cp);
            cp.SetScale(Vector3.one * deadSize);
            cp.SetPosition(new Vector3(8 * tileSize, yOffset, 8 * tileSize)
                - bounds
                + new Vector3(tileSize / 2, 0, tileSize / 2)
                + (Vector3.left * deadSpacing) * deadBlacks.Count);
        }
    }

    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
                return true;
        }

        return false;
    }
    private void ClearChessPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                    Destroy(chessPieces[x, y].gameObject);

                chessPieces[x, y] = null;
            }
        }

        foreach (ChessPiece cp in deadWhites)
            Destroy(cp.gameObject);
        deadWhites.Clear();

        foreach (ChessPiece cp in deadBlacks)
            Destroy(cp.gameObject);
        deadBlacks.Clear();
    }
    private void FieldsReset()
    {
        currentlyDragging = null;
        availableMoves.Clear();
        moveList.Clear();
    }
    private void ClearUI()
    {
        //victoryScreen.SetActive(false);
        //victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        //victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
    }

    #region
    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;

        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGameClient;
    }
    private void UnRegisterEvents()
    {

    }
    //Server
    private void OnWelcomeServer(NetMessage message, NetworkConnection connection)
    {
        NetWelcome welcome = message as NetWelcome;
        welcome.AssignedTeam = ++playerCount;
        Server.Instance.SendToClient(connection, message);

        if(playerCount == 1)
        {
            Server.Instance.Broadcast(new NetStartGame());
        }
    }
    //Client
    private void OnWelcomeClient(NetMessage message)
    {
        NetWelcome welcome = message as NetWelcome;
        currentTeam = welcome.AssignedTeam;
        Debug.Log($"My assigned team is {welcome.AssignedTeam}");
    }
    private void OnStartGameClient(NetMessage message)
    {
        GameUI.Instance.ChangeCamera((currentTeam == 0) ? CameraAngle.whiteTeam : CameraAngle.blackTeam);
    }
    #endregion
}
