using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    private const int kingX = 4;
    private const int leftRookX = 0;
    private const int rightRookX = 7;
    private const int whiteBoardSide = 0;
    private const int blackBoardSide = 7;

    public King()
    {
        value = 900;
    }
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        int x, y;

        // Right
        x = currentX + 1;
        y = currentY;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        // Up-Right
        x = currentX + 1;
        y = currentY + 1;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        // Down-Right
        x = currentX + 1;
        y = currentY - 1;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        // Left
        x = currentX - 1;
        y = currentY;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        // Up-Left
        x = currentX - 1;
        y = currentY + 1;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        // Down-Left
        x = currentX - 1;
        y = currentY - 1;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        // Up
        x = currentX;
        y = currentY + 1;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        // Down
        x = currentX;
        y = currentY - 1;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        return result;
    }
    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        SpecialMove result = SpecialMove.None;

        Vector2Int[] kingMove = moveList.Find(m => m[0].x == kingX && m[0].y == ((team == 0) ? 0 : 7));
        Vector2Int[] leftRook = moveList.Find(m => m[0].x == leftRookX && m[0].y == ((team == 0) ? 0 : 7));
        Vector2Int[] rightRook = moveList.Find(m => m[0].x == rightRookX && m[0].y == ((team == 0) ? 0 : 7));

        if (kingMove == null && currentX == kingX)
        {
            int side = (team == 0) ? whiteBoardSide : blackBoardSide;
            if (leftRook == null
                && board[0, side].type == ChessPieceType.Rook
                && board[0, side].team == team
                && board[3, side] == null
                && board[2, side] == null
                && board[1, side] == null)
            {
                availableMoves.Add(new Vector2Int(2, side));
                result = SpecialMove.Castling;
            }

            if (rightRook == null
                && board[7, side].type == ChessPieceType.Rook
                && board[7, side].team == team
                && board[6, side] == null
                && board[5, side] == null)
            {
                availableMoves.Add(new Vector2Int(6, side));
                result = SpecialMove.Castling;
            }
        }

        return result;
    }

    private void CheckTargetTile(ChessPiece[,] board, int tileCountX, int tileCountY, List<Vector2Int> result, int x, int y)
    {
        if (x >= 0 && x < tileCountX && y >= 0 && y < tileCountY)
            if (board[x, y] == null || board[x, y].team != team)
                result.Add(new Vector2Int(x, y));
    }
}
