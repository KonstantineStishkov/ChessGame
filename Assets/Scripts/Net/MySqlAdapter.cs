using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class MySqlAdapter : IDbAdapter
{
    const string getGamesAddress = "http://89.208.137.229/Chess_game/GetGames.php";
    const string hostGameAddress = "http://89.208.137.229/Chess_game/HostGame.php";
    const string registerAddress = "http://89.208.137.229/Chess_game/RegisterUser.php";
    const string loginAddress = "http://89.208.137.229/Chess_game/LoginUser.php";
    const string getExpAddress = "http://89.208.137.229/Chess_game/GetUserExp.php";
    const string getIpAddress = "http://checkip.dyndns.org";

    const string isSuccess = "1";

    public bool SendServerData(string serverName, ushort port, int difficulty)
    {
        return SendRequest(hostGameAddress, ComposeServerData(serverName, port, difficulty)) != null;
    }

    public List<Server_button> GetServers()
    {
        string response = SendRequest(getGamesAddress);

        return ParseServerResponse(response);
    }

    public bool RegisterNewUser(string userName, string password)
    {
        return SendRequest(registerAddress, ComposeUserData(userName, password)) == isSuccess;
    }

    public bool Login(string userName, string password, out Player player)
    {
        player = new Player();
        player.Name = userName;

        bool result = SendRequest(loginAddress, ComposeUserData(userName, password)) == isSuccess;
        if (result)
            player.Experience = GetPlayerExperience(userName, password);

        return result;
    }

    private int GetPlayerExperience(string userName, string password)
    {
        if (int.TryParse(SendRequest(getExpAddress, ComposeUserData(userName, password)), out int exp))
            return exp;
        else
            throw new ArgumentException();
    }
    private string GetMyIP()
    {
        string response = SendRequest(getIpAddress);
        string result = response.Split(':')[1].Split('<')[0];
        return result;
    }

    private static List<Server_button> ParseServerResponse(string response)
    {
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

    private NameValueCollection ComposeServerData(string serverName, ushort port, int difficulty)
    {
        NameValueCollection data = new NameValueCollection();
        data.Add("ip", GetMyIP().Trim());
        data.Add("port", port.ToString());
        data.Add("serverName", serverName);
        data.Add("difficulty", difficulty.ToString());

        return data;
    }

    private NameValueCollection ComposeUserData(string userName, string password)
    {
        NameValueCollection data = new NameValueCollection();
        data.Add("login", userName);
        data.Add("password", password);

        return data;
    }

    private string SendRequest(string address, NameValueCollection sendingData = null)
    {
        byte[] responseBytes;
        string result = string.Empty;

        try
        {
            using (WebClient client = new WebClient())
            {
                if (sendingData == null)
                    responseBytes = client.DownloadData(address);
                else
                    responseBytes = client.UploadValues(address, "POST", sendingData);
            }

            result = Encoding.UTF8.GetString(responseBytes);
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        return result;
    }
}
