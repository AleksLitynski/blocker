using UnityEngine;
using System.Collections.Generic;

//this will be responsable for downloading the map and players when the game starts
//currently just downloads players, as that's all there is right now

//also, unloads players and map on disconnect
public class MapManager : BlockerObject
{
    void OnPlayerConnected (NetworkPlayer player)
    {
        //send player map to load

        //send player characters to load
        for (var i = 0; i < playerManager.players.Count; i++)
        {
            NetworkPlayer computer = (playerManager.players[i].GetComponent("NetPlayer") as NetPlayer).networkPlayer;
            int playerNumber = (playerManager.players[i].GetComponent("NetPlayer") as NetPlayer).localPlayerNumber;
            networkView.RPC("AddNewPlayer", player, computer, playerNumber);
        }
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        //remove the map

        //remove all characters
        playerManager.players = new List<NetPlayer>();
        playerManager.localPlayers = new List<NetPlayer>();
        Transform rootTeam = gameObject.transform.Find("RootTeam");
        for (int i = 0; i < rootTeam.GetChildCount(); i++)
        {
            GameObject toGo = rootTeam.GetChild(i).gameObject;
            Destroy(toGo);
        }
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        for (int i = 0; i < playerManager.players.Count; i++)
        {
            if (playerManager.players[i].networkPlayer == player)
            {
                networkView.RPC("RemovePlayer", RPCMode.Others, player, playerManager.players[i].localPlayerNumber);
                if (Network.peerType == NetworkPeerType.Server)
                {
                    playerManager.RemovePlayer(player, playerManager.players[i].localPlayerNumber);
                    i--;
                } 
            }
        }
    }
}
