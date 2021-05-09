using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;

public class NetworkGamePlayerLobby : NetworkBehaviour
{
    //create when player connect, destroy when player leave


    //sync var can only been change by server, and update iteself
    [SyncVar]
    private string displayName = "Loading...";


    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }
 


    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        Room.GamePlayers.Add(this);

    }
    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);

    }


    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }




}
