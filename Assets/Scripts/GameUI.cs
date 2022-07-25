using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    #region string Constants
    const string cancelLabel = "Cancel";
    const string exitLabel = "Exit";
    const string loginLabel = "Login";
    const string registerLabel = "Register";
    const string mailLabel = "E-mail";
    const string passwordLabel = "Password";
    const string tryAgainLabel = "Try Again";
    #endregion
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
    [SerializeField] GameObject modalWindowButtons;

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
        dbAdapter = new MySqlAdapter();
        buttons.Awake();
        ChangeCamera(CameraAngle.menu);
        MakeButtons();
        ProcessAuthentication();
    }
    #endregion
    #region public Functions
    public void ChangeCamera(CameraAngle index)
    {
        for (int i = 0; i < cameraAngles.Length; i++)
            cameraAngles[i].SetActive(false);

        cameraAngles[(int)index].SetActive(true);
    }
    public void DisplayVictory(int winningTeam)
    {
        const string rematchLabel = "Rematch";

        string team = winningTeam == 0 ? "White" : "Black";
        string victoryMessage = $"{team} team wins. Rematch?";
        window.CallWindow(victoryMessage, () => { }, rematchLabel, () => { }, exitLabel);
    }
    #endregion
    private void ProcessAuthentication()
    {
        const string loginMessage = "Please Input Login and Password to Login";
        HideAllMenus();

        if (player == null)
        {
            window.CallWindow(loginMessage, OnLoginButton, loginLabel, OnRegisterButton, registerLabel, mailLabel, passwordLabel);
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
        const string wrongNameMessage = "User name has already taken";

        if (!dbAdapter.RegisterNewUser(window.Field1, window.Field2))
        {
            window.CallWindow(wrongNameMessage, ProcessAuthentication, tryAgainLabel);
            player = null;
        }
    }
    public void OnLoginButton()
    {
        const string wrongPasswordMessage = "Wrong login or password";
        const string successMessage = "Successfully logged in";

        if (!dbAdapter.Login(window.Field1, window.Field2, out player))
        {
            StatusBar.ShowMessage(MessageType.Warning, wrongPasswordMessage);
            window.CallWindow(wrongPasswordMessage, ProcessAuthentication, "Try again");
            player = null;
        }
        else
        {
            ProcessAuthentication();
            StatusBar.ShowMessage(MessageType.Info, successMessage);
        }
    }
    #endregion
    #region Main menu Buttons
    public void OnLocalGameButton()
    {
        SetLocalGame?.Invoke(true);
        HideAllMenus();
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
        const string waitMessage = "Waiting for connection...";

        serverList.RegisterGameServer(port);
        serverList.RefreshServerList();
        StatusBar.ShowMessage(MessageType.Info, waitMessage);
        ConnectToSelf();
        HideAllMenus();
        window.CallWindow(waitMessage, OnStopHost, cancelLabel);
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
        const string localIP = "127.0.0.1";
        HideAllMenus();
        client.Init(localIP, port);
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
        Log.SetActive(true);
        Log.SetActive(false);
        Logo.SetActive(false);
    }

    private void ConnectToSelf()
    {
        const string localIP = "127.0.0.1";
        server.Init(port);
        client.Init(localIP, port);
    }
    #endregion
}
