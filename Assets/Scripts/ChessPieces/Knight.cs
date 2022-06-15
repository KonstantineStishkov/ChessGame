using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public Knight()
    {
        value = 30;
    }
    public int Value { get { return (team == 0) ? value : -value; } }
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        // Top right
        int x = currentX + 1;
        int y = currentY + 2;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        x = currentX + 2;
        y = currentY + 1;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        // Top left
        x = currentX - 1;
        y = currentY + 2;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        x = currentX - 2;
        y = currentY + 1;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        // Bottom right
        x = currentX + 1;
        y = currentY - 2;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        x = currentX + 2;
        y = currentY - 1;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        // Bottom left
        x = currentX - 1;
        y = currentY - 2;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        x = currentX - 2;
        y = currentY - 1;
        CheckTargetTile(board, tileCountX, tileCountY, result, x, y);

        return result;
    }

    private void CheckTargetTile(ChessPiece[,] board, int tileCountX, int tileCountY, List<Vector2Int> result, int x, int y)
    {
        if (x < tileCountX && y < tileCountY && x >= 0 && y >= 0)
            if (board[x, y] == null || board[x, y].team != team)
                result.Add(new Vector2Int(x, y));
    }
}
