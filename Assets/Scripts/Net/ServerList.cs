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

    //[Header("Functional")]
    //[SerializeField] IDbAdapter DbAdapter;

    private string ServerName;
    private IDbAdapter DbAdapter;

    public void Awake()
    {
        RefreshServerList();
    }

    public ServerList(IDbAdapter dbAdapter, string name)
    {
        ServerName = name;
        DbAdapter = dbAdapter;
    }

    public void RefreshServerList()
    {
        ClearServerList();
        FillServerList(DbAdapter.GetServers());
    }

    public void RegisterGameServer()
    {
        DbAdapter.SendServerData(ServerName, 1);

        RefreshServerList();
    }

    public void ServerSelected(Server_button button, List<Server_button> serverButtons)
    {
        button.isSelected = true;
        foreach (Server_button item in serverButtons)
        {
            if (!item.Equals(button))
                item.isSelected = false;
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
