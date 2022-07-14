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

    [Header("Top Bar")]
    [SerializeField] GameObject TopBar;
    [SerializeField] GameObject[] StarObjects;
    [SerializeField] Sprite[] StarIcons;
    [SerializeField] TextMeshProUGUI PlayerNameTMP;


    [Header("Input Fields")]
    [SerializeField] TMP_InputField Login;
    [SerializeField] TMP_InputField Password;

    [Header("Functional objects")]
    [SerializeField] ServerList serverList;
    [SerializeField] Server server;
    [SerializeField] Client client;

    [Header("Chess Board")]
    [SerializeField] ChessBoard chessBoard;

    [Header("Settings")]
    [SerializeField] ushort port;

    private IDbAdapter dbAdapter;
    private Player player;

    public static GameUI Instance { get; set; }

    private void Awake()
    {
        Instance = this;
        HideAllMenus();

        if(player == null)
        {
            AuthenticationMenu.SetActive(true);
            dbAdapter = new MySqlAdapter();
        }
        else
        {
            MainMenu.SetActive(true);
            TopBar.SetActive(true);
        }
    }

    //Get Name Window
    public void OnRegisterButton()
    {
        dbAdapter.RegisterNewUser(Login.text, Password.text);
    }
    public void OnLoginButton()
    {
        if (dbAdapter.Login(Login.text, Password.text, out player))
        {
            HideAllMenus();
            MainMenu.SetActive(true);
            FillTopBar();
            TopBar.SetActive(true);
        }
    }

    //Main menu Buttons
    public void OnLocalGameButton()
    {
        HideAllMenus();
        ConnectToSelf();
    }

    public void OnOnlineGameButton()
    {
        HideAllMenus();
        serverList.Initialize(dbAdapter, Login.text, player.Level);
        OnlineMenu.SetActive(true);
    }

    public void OnAgainstAIButton()
    {
        HideAllMenus();
        ConnectToSelf();
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
        serverList.RegisterGameServer(port);
        serverList.RefreshServerList();
        ConnectToSelf();
    }

    public void OnJoinGame()
    {
        HideAllMenus();
        client.Init(serverList.SelectedIP, port);
    }

    public void OnRefreshList()
    {
        serverList.RefreshServerList();
    }

    public void OnBack()
    {
        server.Shutdown();
        HideAllMenus();
        MainMenu.SetActive(true);
    }

    //Top Bar
    private void FillTopBar()
    {
        for(int i = 0; i < 5; i++)
        {
            UnityEngine.UI.Image renderer = StarObjects[i].GetComponent(typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
            renderer.sprite = player.Level > i ? StarIcons[1] : StarIcons[0];                
        }

        PlayerNameTMP.text = player.Name;
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

    private void ConnectToSelf()
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007);
    }
}
