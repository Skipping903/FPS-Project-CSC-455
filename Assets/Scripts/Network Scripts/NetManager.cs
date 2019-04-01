using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager
{
    private bool firstPlayerJoined;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerID)
    {
        GameObject playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        List<Transform> spawnPositions = NetworkManager.singleton.startPositions;

        if (!this.firstPlayerJoined)
        {
            this.firstPlayerJoined = true;
            playerObj.transform.position = spawnPositions[0].position;
        }
        else
        {
            playerObj.transform.position = spawnPositions[1].position;
        }

        NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerID);
    }

    void SetPortAndAddress()
    {
        NetworkManager.singleton.networkAddress = "localhost";
        NetworkManager.singleton.networkPort = 7777;
    }

    public void HostGame()
    {
        this.SetPortAndAddress();
        NetworkManager.singleton.StartHost();
    }

    public void JoinGame()
    {
        this.SetPortAndAddress();
        NetworkManager.singleton.StartClient();
    }
} // class