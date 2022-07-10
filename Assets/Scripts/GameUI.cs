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

    [Header("Functional objects")]
    [SerializeField] ServerList serverList;

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

    //Main menu Buttons
    public void OnLocalGameButton()
    {
        HideAllMenus();
    }

    public void OnOnlineGameButton()
    {
        HideAllMenus();
        OnlineMenu.SetActive(true);
    }

    public void onAgainstAIButton()
    {
        HideAllMenus();
        DifficultyMenu.SetActive(true);
    }

    //Difficulty Menu Buttons
    public void onEasyGameButton()
    {
        Debug.Log("Easy Difficulty");
        HideAllMenus();
        Log.SetActive(true);
        chessBoard.StartGame(Difficulty.Easy);
    }

    public void onNormalGameButton()
    {
        Debug.Log("Normal Difficulty");
        HideAllMenus();
    }

    public void onHardGameButton()
    {
        Debug.Log("Hard Difficulty");
        HideAllMenus();
    }

    //Online Game Buttons
    public void onHostGame()
    {
        serverList.RegisterGameServer();
        serverList.RefreshServerList();
    }

    //Common functions
    private void HideAllMenus()
    {
        MainMenu.SetActive(false);
        DifficultyMenu.SetActive(false);
        OnlineMenu.SetActive(false);
        Log.SetActive(false);
    }
}
