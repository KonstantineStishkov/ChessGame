using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject DifficultyMenu;
    [SerializeField] GameObject OnlineMenu;
    [SerializeField] GameObject Log;

    [Header("Chess Board")]
    [SerializeField] ChessBoard chessBoard;

    public static GameUI Instance { get; set; }
    public Server server;
    public Client client;

    private void Awake()
    {
        Instance = this;
        MainMenu.SetActive(true);
        DifficultyMenu.SetActive(false);
        OnlineMenu.SetActive(false);
        Log.SetActive(false);
    }

    //Buttons
    public void OnLocalGameButton()
    {
        Debug.Log("OnLocalGameButton");
    }

    public void OnOnlineGameButton()
    {
        MainMenu.SetActive(false);
        OnlineMenu.SetActive(true);
    }

    public void onAgainstAIButton()
    {
        MainMenu.SetActive(false);
        DifficultyMenu.SetActive(true);
    }

    public void onEasyGameButton()
    {
        Debug.Log("Easy Difficulty");
        MainMenu.SetActive(false);
        DifficultyMenu.SetActive(false);
        Log.SetActive(true);
        chessBoard.StartGame(Difficulty.Easy);
    }

    public void onNormalGameButton()
    {
        Debug.Log("Normal Difficulty");
    }

    public void onHardGameButton()
    {
        Debug.Log("Hard Difficulty");
    }
}
