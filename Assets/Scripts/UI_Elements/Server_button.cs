using System;
using TMPro;
using UnityEngine;

public class Server_button : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] Sprite[] freshSprites;
    [SerializeField] Sprite[] difficultySprites;

    [Header("Elements")]
    [SerializeField] GameObject freshElement;
    [SerializeField] GameObject difficultyElement;
    [SerializeField] GameObject textElement;

    public string ip;
    public string serverName;
    public int port;
    public int difficulty;
    public DateTime date;
    public bool isSelected;


    public void SetData()
    {
        SetDifficulty();
        SetServerName();
        SetFresh();
    }



    private int fresh
    {
        set
        {
            if (value < freshSprites.Length)
            {
                SpriteRenderer renderer = freshElement.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                renderer.sprite = freshSprites[value];
            }

        }
    }

    private void SetDifficulty()
    {
        if (difficulty < difficultySprites.Length)
        {
            SpriteRenderer renderer = difficultyElement.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            renderer.sprite = difficultySprites[difficulty];
        }
    }

    private void SetServerName()
    {
        if (serverName != null)
        {            
            TextMeshProUGUI tmpro = GetComponentInChildren(typeof(TextMeshProUGUI), false) as TextMeshProUGUI;
            tmpro.text = serverName;
        }
    }

    private void SetFresh()
    {
        if ((DateTime.Now - date).Minutes < 5)
        {
            fresh = 0;
        }
        else if ((DateTime.Now - date).Minutes < 30)
        {
            fresh = 1;
        }
        else
        {
            fresh = 2;
        }
    }
}
