using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 2;

    [Scene] [SerializeField] private string menuScene = string.Empty;

    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;


    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;


    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    //waiting room players list
    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    //public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public List<NetworkGamePlayerLobby> GamePlayers { get; } = new List<NetworkGamePlayerLobby>();

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


    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            if (!IsReadyToStart()) { return; }

            //mapHandler = new MapHandler(mapSet, numberOfRounds);

           
            ServerChangeScene("SceneGame_1");
        }
    }

    
    public override void ServerChangeScene(string newSceneName)
    {
        // From menu to game
        Debug.Log("Server Chagne Scene menuScene: " + menuScene);
        Debug.Log("Server Chagne Scene path" + SceneManager.GetActiveScene().path);
        Debug.Log("Server Chagne Scene new SceneName" + newSceneName);
        if (SceneManager.GetActiveScene().path == menuScene && newSceneName.Contains("SceneGame_"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                //remove room player
                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
                //NetworkServer.Destroy(conn.identity.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }


    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.Contains("SceneGame"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);

        }
    }


}
