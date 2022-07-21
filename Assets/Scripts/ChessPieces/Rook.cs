using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    public Rook()
    {
        value = 50;
    }
    public new int Value { get { return (team == 0) ? value : -value; } }
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        // Down
        for (int i = currentY - 1; i >= 0; i--)
        {
            ChessPiece targetPiece = board[currentX, i];
            Vector2Int vector = new Vector2Int(currentX, i);

            if (targetPiece == null)
                result.Add(vector);

            if (targetPiece != null)
            {
                if (targetPiece.team != team)
                    result.Add(vector);

                break;
            }
        }

        // Up
        for (int i = currentY + 1; i < tileCountY; i++)
        {
            ChessPiece targetPiece = board[currentX, i];
            Vector2Int vector = new Vector2Int(currentX, i);

            if (targetPiece == null)
                result.Add(vector);

            if (targetPiece != null)
            {
                if (targetPiece.team != team)
                    result.Add(vector);

                break;
            }
        }

        // Left
        for (int i = currentX - 1; i >= 0; i--)
        {
            ChessPiece targetPiece = board[i, currentY];
            Vector2Int vector = new Vector2Int(i, currentY);

            if (targetPiece == null)
                result.Add(vector);

            if (targetPiece != null)
            {
                if (targetPiece.team != team)
                    result.Add(vector);

                break;
            }
        }

        // Right
        for (int i = currentX + 1; i < tileCountX; i++)
        {
            ChessPiece targetPiece = board[i, currentY];
            Vector2Int vector = new Vector2Int(i, currentY);

            if (targetPiece == null)
                result.Add(vector);

            if (targetPiece != null)
            {
                if (targetPiece.team != team)
                    result.Add(vector);

                break;
            }
        }

        return result;
    }
}
