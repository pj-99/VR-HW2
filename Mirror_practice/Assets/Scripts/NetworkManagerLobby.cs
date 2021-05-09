using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 2;

    [Scene] [SerializeField] private string menuScene = string.Empty;

    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;


    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    //waiting room players list
    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    //public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    
    private  void Awake()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").Where(x => x != playerPrefab).ToList();
    }
    
    public override void OnStartClient()
    {
        
        /**
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            //Debug.Log("on start client in for"+prefab.name);
            ClientScene.UnregisterPrefab(prefab);
            ClientScene.RegisterPrefab(prefab);
        }
        //Debug.Log("on start client end");
        */
    }


    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        OnClientConnected?.Invoke();
    }


    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        // if game is start, not let people in
        if (SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null) {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
            RoomPlayers.Remove(player);
            NotfiyPlayersReadyState();
        }
        base.OnServerDisconnect(conn);
    }

    public void NotfiyPlayersReadyState()
    {
        foreach (var player in RoomPlayers) {
            player.HandleReadyToStart(IsReadyToStart()); 
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) return false;
        foreach (var player in RoomPlayers) {
            if (!player.IsReady) return false;
        }
        return true;
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            //choose fist person as leader
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }


}
