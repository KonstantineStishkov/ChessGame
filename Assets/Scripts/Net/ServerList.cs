using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ServerList : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Server_button serverButton;
    [SerializeField] GameObject list;

    private IDbAdapter DbAdapter;
    public string SelectedIP { get; private set; }
    public string ServerName { get; private set; }
    public int Difficulty { get; private set; }

    public void Awake()
    {
        RefreshServerList();
    }

    public void Initialize(IDbAdapter dbAdapter, string name, int difficulty)
    {
        ServerName = name;
        DbAdapter = dbAdapter;
        Difficulty = difficulty;
    }

    public void RefreshServerList()
    {
        ClearServerList();
        FillServerList(DbAdapter.GetServers());
    }

    public void RegisterGameServer(ushort port)
    {
        DbAdapter.SendServerData(ServerName, port, Difficulty);

        RefreshServerList();
    }

    public void ServerSelected(Server_button button, List<Server_button> serverButtons)
    {
        button.isSelected = true;
        for (int i = 0; i < serverButtons.Count; i++)
        {
            if (serverButtons[i].Equals(button))
                SelectedIP = serverButtons[i].ip;
        }
    }

    private void ClearServerList()
    {
        Component[] children = GetComponentsInChildren(typeof(Server_button));

        for (int i = 0; i < children.Length; i++)
        {
            Destroy(children[i].gameObject);
        }
    }

    private void FillServerList(List<Server_button> serverButtons)
    {
        foreach (Server_button item in serverButtons)
        {
            Server_button button = Instantiate(serverButton, transform);
            button.serverName = item.serverName;
            button.ip = item.ip;
            button.port = item.port;
            button.date = item.date;
            button.difficulty = item.difficulty;
            button.SetData();

            Button btn = button.GetComponent(typeof(Button)) as Button;
            btn.onClick.AddListener(() => ServerSelected(button, serverButtons));
        }
    }
}
