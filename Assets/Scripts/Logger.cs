using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BoardLetters
{
    A = 7,
    B = 6,
    C = 5,
    D = 4,
    E = 3,
    F = 2,
    G = 1,
    H = 0
}

public class Logger : MonoBehaviour
{
    [Header("Log")]
    [SerializeField] TextMeshProUGUI textMesh;

    private StringBuilder stb;
    private void Awake()
    {
        //textMesh = log.GetComponent("LogText") as TextMeshPro;
        stb = new StringBuilder();
    }

    public void AddLine(string line)
    {
        stb.Insert(0, "\n" + line);

        textMesh.text = stb.ToString();
    }

    public void LogTurn(int team, ChessPieceType type, Vector2Int startPosition, Vector2Int finishPosition)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(team == 0 ? "White " : "Black ");
        stringBuilder.Append(type);
        stringBuilder.Append(" ");
        stringBuilder.Append(GetPositionText(startPosition));
        stringBuilder.Append(":");
        stringBuilder.Append(GetPositionText(finishPosition));
        AddLine(stringBuilder.ToString());
    }

    private string GetPositionText(Vector2Int position)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append((BoardLetters)position.x);
        stringBuilder.Append(8 - position.y);
        return stringBuilder.ToString();
    }
}
