using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject AuthenticationMenu;
    [SerializeField] GameObject DifficultyMenu;
    [SerializeField] GameObject OnlineMenu;
    [SerializeField] GameObject Log;

    [Header("Input Fields")]
    [SerializeField] TMP_InputField Login;
    [SerializeField] TMP_InputField Password;

    //[Header("Functional objects")]
    //[SerializeField] ServerList serverList;

    [Header("Chess Board")]
    [SerializeField] ChessBoard chessBoard;

    private IDbAdapter dbAdapter;
    private ServerList serverList;

    public static GameUI Instance { get; set; }

    private void Awake()
    {
        Instance = this;
        HideAllMenus();
        AuthenticationMenu.SetActive(true);
        dbAdapter = new MySqlAdapter();
    }

    //Get Name Window
    public void OnRegisterButton()
    {
        dbAdapter.RegisterNewUser(Login.text, Password.text);
    }
    public void OnLoginButton()
    {
        if (dbAdapter.Login(Login.text, Password.text))
        {
            serverList = new ServerList(dbAdapter, Login.text);
            HideAllMenus();
            MainMenu.SetActive(true);
        }
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

    public void OnAgainstAIButton()
    {
        HideAllMenus();
        DifficultyMenu.SetActive(true);
    }

    //Difficulty Menu Buttons
    public void OnEasyGameButton()
    {
        Debug.Log("Easy Difficulty");
        HideAllMenus();
        Log.SetActive(true);
        chessBoard.StartGame(Difficulty.Easy);
    }

    public void OnNormalGameButton()
    {
        Debug.Log("Normal Difficulty");
        HideAllMenus();
    }

    public void OnHardGameButton()
    {
        Debug.Log("Hard Difficulty");
        HideAllMenus();
    }

    //Online Game Buttons
    public void OnHostGame()
    {
        serverList.RegisterGameServer();
        serverList.RefreshServerList();
    }

    public void OnRefreshList()
    {
        serverList.RefreshServerList();
    }

    //Common functions
    private void HideAllMenus()
    {
        MainMenu.SetActive(false);
        AuthenticationMenu.SetActive(false);
        DifficultyMenu.SetActive(false);
        OnlineMenu.SetActive(false);
        Log.SetActive(false);
    }
}
