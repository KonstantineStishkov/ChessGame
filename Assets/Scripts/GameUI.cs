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
    [SerializeField] GameObject ModalWindowObject;

    [Header("Top Bar")]
    [SerializeField] GameObject TopBar;
    [SerializeField] GameObject[] StarObjects;
    [SerializeField] Sprite[] StarIcons;
    [SerializeField] TextMeshProUGUI PlayerNameTMP;


    //[Header("Input Fields")]
    //[SerializeField] TMP_InputField Login;
    //[SerializeField] TMP_InputField Password;

    [Header("Functional objects")]
    [SerializeField] ServerList serverList;
    [SerializeField] Server server;
    [SerializeField] Client client;
    [SerializeField] ModalWindow window;

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
        ProcessAuthentication();
    }

    private void ProcessAuthentication()
    {
        HideAllMenus();

        if (player == null)
        {
            window.CallWindow(WindowType.DoubleInput, "Please Input Login and Password to Login", OnLoginButton, OnRegisterButton, "Login", "Register");
            dbAdapter = new MySqlAdapter();
        }
        else
        {
            ModalWindowObject.SetActive(false);
            FillTopBar();
            MainMenu.SetActive(true);
            TopBar.SetActive(true);
        }
    }

    //Get Name Window
    public void OnRegisterButton()
    {
        if(!dbAdapter.RegisterNewUser(window.Field1, window.Field2))
        {
            window.CallWindow(WindowType.Info, "User name has already taken", ProcessAuthentication);
            player = null;
        }
    }
    public void OnLoginButton()
    {
        if (!dbAdapter.Login(window.Field1, window.Field2, out player))
        {
            window.CallWindow(WindowType.Info, "Wrong login or password", ProcessAuthentication);
            player = null;
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
        serverList.Initialize(dbAdapter, player.Name, player.Level);
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
        client.Shutdown();
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
