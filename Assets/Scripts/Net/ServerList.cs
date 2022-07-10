using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ServerList : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Server_button serverButton;
    [SerializeField] GameObject list;

    [Header("Settings")]
    [SerializeField] string connectionString;

    const string getGamesAddress = "http://89.208.137.229/Chess_game/GetGames.php";
    const string hostGameAddress = "http://89.208.137.229/Chess_game/HostGame.php";
    const string testAddress = "http://89.208.137.229/Chess_game/TestPost.php";

    public void Awake()
    {
        RefreshServerList();
    }

    public void RefreshServerList()
    {
        ClearServerList();
        FillServerList(GetServerList());
    }

    public void RegisterGameServer()
    {
        byte[] responseBytes = new byte[512];
        NameValueCollection data = new NameValueCollection();
        data.Add("port", "8007");
        data.Add("serverName", "testName");
        data.Add("difficulty", "1");
        using (WebClient client = new WebClient())
        {
            responseBytes = client.UploadValues(hostGameAddress, "POST", data);
        }

        string response = Encoding.UTF8.GetString(responseBytes);
    }

    private void ClearServerList()
    {
        Component[] children = GetComponentsInChildren(typeof(Server_button));

        for (int i = 0; i < children.Length; i++)
        {
            Destroy(children[i].gameObject);
        }
    }

    private List<Server_button> GetServerList()
    {
        byte[] responseBytes;
        using (WebClient client = new WebClient())
        {
            NameValueCollection data = new NameValueCollection();
            responseBytes = client.UploadValues(getGamesAddress, "POST", data);
        }

        string response = Encoding.UTF8.GetString(responseBytes);

        string[] servers = response.Split('*');

        List<Server_button> server_list = new List<Server_button>();
        Server_button serverButton = new Server_button();

        foreach (string item in servers)
        {
            var text = item.Replace("\n", "");
            text = text.Replace("@", "");
            item.Trim();
            Regex regex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            int num;

            if (regex.IsMatch(text))
            {
                serverButton.ip = text;
                continue;
            }

            if (DateTime.TryParse(text, out DateTime date))
            {
                serverButton.date = date;
                continue;
            }

            if (int.TryParse(text, out num))
            {
                if (num > 0 && num < 4)
                {
                    serverButton.difficulty = num;
                }
                else if (num < 65537)
                {
                    serverButton.port = num;
                }

                continue;
            }

            if (item == "@")
            {
                server_list.Add(serverButton);
                serverButton = new Server_button();
                continue;
            }

            if (item != null && serverButton.serverName == null)
            {
                serverButton.serverName = item;
            }
        }

        return server_list;
    }

    private void FillServerList(List<Server_button> serverButtons)
    {
        foreach (var item in serverButtons)
        {
            Server_button button = Instantiate(serverButton, transform);
            button.serverName = item.serverName;
            button.ip = item.ip;
            button.port = item.port;
            button.date = item.date;
            button.difficulty = item.difficulty;
            button.SetData();
        }
    }
}
