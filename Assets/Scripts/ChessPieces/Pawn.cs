using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public Pawn()
    {
        value = 10;
    }
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;

        //One in front
        if (board[currentX, currentY + direction] == null)
            result.Add(new Vector2Int(currentX, currentY + direction));

        //Two in front
        if (board[currentX, currentY + direction] == null)
        {
            //White team
            if (team == 0 && currentY == 1 && board[currentX, currentY + (direction * 2)] == null)
                result.Add(new Vector2Int(currentX, currentY + (direction * 2)));

            //Black team
            if (team == 1 && currentY == 6 && board[currentX, currentY + (direction * 2)] == null)
                result.Add(new Vector2Int(currentX, currentY + (direction * 2)));
        }

        //Kill move
        if (currentX != tileCountX - 1) //Left
            if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
                result.Add(new Vector2Int(currentX + 1, currentY + direction));

        if (currentX != 0) //Right
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
                result.Add(new Vector2Int(currentX - 1, currentY + direction));

        return result;
    }

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        int direction = (team == 0) ? 1 : -1;

        if ((team == 0 && currentY == 6) || (team == 1 && currentY == 1))
            return SpecialMove.Promotion;

        //En Passant
        if (moveList.Count > 0)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];

            if ((board[lastMove[1].x, lastMove[1].y].type == ChessPieceType.Pawn)        // If the Last moved piece was pawn
             && (Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2)                          // Pawn moved 2 tiles in last move
             && (board[lastMove[1].x, lastMove[1].y].team != team)                       // Move was from other team
             && (lastMove[1].y == currentY)                                              // Both pawns on the same Y
             && (lastMove[1].x == currentX - 1 || lastMove[1].x == currentX + 1))        // Landed nearest left or nearest right
            {
                availableMoves.Add(new Vector2Int(lastMove[1].x, currentY + direction));
                return SpecialMove.EnPassant;
            }
        }

        return SpecialMove.None;
    }
}
