using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    #region Properties
    [Header("Menus")]
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject DifficultyMenu;
    [SerializeField] GameObject OnlineMenu;
    [SerializeField] GameObject OnlineMenuButtons;
    [SerializeField] GameObject Log;
    [SerializeField] GameObject Logo;
    [SerializeField] GameObject ModalWindowObject;

    [Header("Top Bar")]
    [SerializeField] GameObject TopBar;
    [SerializeField] StatusBar StatusBar;
    [SerializeField] GameObject[] StarObjects;
    [SerializeField] Sprite[] StarIcons;
    [SerializeField] TextMeshProUGUI PlayerNameTMP;

    [Header("Camera Options")]
    [SerializeField] GameObject[] cameraAngles;

    [Header("Functional objects")]
    [SerializeField] ServerList serverList;
    [SerializeField] Server server;
    [SerializeField] Client client;
    [SerializeField] Buttons buttons;
    [SerializeField] ModalWindow window;

    [Header("Chess Board")]
    [SerializeField] ChessBoard chessBoard;

    [Header("Settings")]
    [SerializeField] ushort port;

    private IDbAdapter dbAdapter;
    private Player player;

    public Action<bool> SetLocalGame;

    public static GameUI Instance { get; set; }

    private void Awake()
    {
        Instance = this;
        ChangeCamera(CameraAngle.menu);
        MakeButtons();
        ProcessAuthentication();
    }
    #endregion
    public void ChangeCamera(CameraAngle index)
    {
        for (int i = 0; i < cameraAngles.Length; i++)
            cameraAngles[i].SetActive(false);

        cameraAngles[(int)index].SetActive(true);
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
            Logo.SetActive(true);

            TopBar.SetActive(true);
        }
    }
    private void MakeButtons()
    {
        buttons.SetButton(MainMenu, "Play Local", OnLocalGameButton);
        buttons.SetButton(MainMenu, "Play Online", OnOnlineGameButton);
        buttons.SetButton(MainMenu, "Play Against AI", OnAgainstAIButton);
        buttons.SetButton(MainMenu, "Settings", OnSettingsButton);
        buttons.SetButton(MainMenu, "Credits", OnCreditsButton);
        buttons.SetButton(MainMenu, "Exit", OnExitButton);

        buttons.SetButton(OnlineMenuButtons, "Host Game", OnHostGame);
        buttons.SetButton(OnlineMenuButtons, "Join Game", OnJoinGame);
        buttons.SetButton(OnlineMenuButtons, "Join Directly", OnJoinDirectly);
        buttons.SetButton(OnlineMenuButtons, "Refresh List", OnRefreshList);
        buttons.SetButton(OnlineMenuButtons, "Back", OnBack);

        buttons.SetButton(DifficultyMenu, "Easy", OnEasyGameButton);
        buttons.SetButton(DifficultyMenu, "Medium", OnMediumGameButton);
        buttons.SetButton(DifficultyMenu, "Hard", OnHardGameButton);
    }
    #region Get Name Window
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
            StatusBar.ShowMessage(MessageType.Warning, "Wrong login or password");
            window.CallWindow(WindowType.Info, "Wrong login or password", ProcessAuthentication);
            player = null;
        }
        else
        {
            ProcessAuthentication();
            StatusBar.ShowMessage(MessageType.Info, "Successfully logged in");
        }
    }
    #endregion
    #region Main menu Buttons
    public void OnLocalGameButton()
    {
        SetLocalGame?.Invoke(true);
        HideAllMenus();
        Log.SetActive(true);
        ConnectToSelf();
    }

    public void OnOnlineGameButton()
    {
        SetLocalGame?.Invoke(false);
        HideAllMenus();
        serverList.Initialize(dbAdapter, player.Name, player.Level);
        OnlineMenu.SetActive(true);
    }

    public void OnAgainstAIButton()
    {
        HideAllMenus();
        DifficultyMenu.SetActive(true);
    }
    public void OnSettingsButton()
    {

    }
    public void OnCreditsButton()
    {

    }
    public void OnExitButton()
    {

    }
    #endregion
    #region Difficulty Menu Buttons
    public void OnEasyGameButton()
    {
        Debug.Log("Easy Difficulty");
        HideAllMenus();
        ChangeCamera(CameraAngle.blackTeam);
        Log.SetActive(true);
        chessBoard.StartGame(Difficulty.Easy);
    }

    public void OnMediumGameButton()
    {
        Debug.Log("Normal Difficulty");
        HideAllMenus();
    }

    public void OnHardGameButton()
    {
        Debug.Log("Hard Difficulty");
        HideAllMenus();
    }
    #endregion
    #region Online Game Buttons
    public void OnHostGame()
    {
        serverList.RegisterGameServer(port);
        serverList.RefreshServerList();
        StatusBar.ShowMessage(MessageType.Info, "Waiting for connection...");
        ConnectToSelf();
        HideAllMenus();
        window.CallWindow(WindowType.Info, "Waiting for connection...", OnStopHost);
    }

    public void OnStopHost()
    {

    }

    public void OnJoinGame()
    {
        HideAllMenus();
        client.Init(serverList.SelectedIP, port);
    }
    public void OnJoinDirectly()
    {
        HideAllMenus();
        client.Init("127.0.0.1", port);
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
        Logo.SetActive(true);
    }
    #endregion
    #region Top Bar
    private void FillTopBar()
    {
        for(int i = 0; i < 5; i++)
        {
            UnityEngine.UI.Image renderer = StarObjects[i].GetComponent(typeof(UnityEngine.UI.Image)) as UnityEngine.UI.Image;
            renderer.sprite = player.Level > i ? StarIcons[1] : StarIcons[0];                
        }

        PlayerNameTMP.text = player.Name;
    }
    #endregion
    #region Common functions
    private void HideAllMenus()
    {
        MainMenu.SetActive(false);
        DifficultyMenu.SetActive(false);
        OnlineMenu.SetActive(false);
        Log.SetActive(false);
        Logo.SetActive(false);
    }

    private void ConnectToSelf()
    {
        server.Init(port);
        client.Init("127.0.0.1", port);
    }
    #endregion
}
