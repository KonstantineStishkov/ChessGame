using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDbAdapter
{
    public bool SendServerData(string serverName, int difficulty);
    public List<Server_button> GetServers();
    public bool RegisterNewUser(string userName, string password);
    public bool Login(string userName, string password);
}
