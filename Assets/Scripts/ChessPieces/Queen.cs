using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public Queen()
    {
        value = 90;
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
        // Top Right
        for (int x = currentX + 1, y = currentY + 1; x < tileCountX && y < tileCountY; x++, y++)
        {
            if (board[x, y] == null)
                result.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team)
                    result.Add(new Vector2Int(x, y));

                break;
            }
        }
        // Top Left
        for (int x = currentX - 1, y = currentY + 1; x >= 0 && y < tileCountY; x--, y++)
        {
            if (board[x, y] == null)
                result.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team)
                    result.Add(new Vector2Int(x, y));

                break;
            }
        }
        // Bottom Right
        for (int x = currentX + 1, y = currentY - 1; x < tileCountX && y >= 0; x++, y--)
        {
            if (board[x, y] == null)
                result.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team)
                    result.Add(new Vector2Int(x, y));

                break;
            }
        }
        // Bottom Left
        for (int x = currentX - 1, y = currentY - 1; x >= 0 && y >= 0; x--, y--)
        {
            if (board[x, y] == null)
                result.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team)
                    result.Add(new Vector2Int(x, y));

                break;
            }
        }

        return result;
    }
}
