using UnityEngine;
using System.Collections.Generic;

//this will be responsable for downloading the map and players when the game starts
//currently just downloads players, as that's all there is right now

//also, unloads players and map on disconnect
public class MapManager : BlockerObject
{
	public GameObject mapToUse;
	public GameObject loadedMap;
	
	void OnServerInitialized()
	{
		
	}
	
	
    void OnPlayerConnected (NetworkPlayer player)
    {
		//send player characters to load
        for (var i = 0; i < playerManager.players.Count; i++)
        {
            NetworkPlayer computer = (playerManager.players[i].GetComponent("NetPlayer") as NetPlayer).networkPlayer;
            int playerNumber = (playerManager.players[i].GetComponent("NetPlayer") as NetPlayer).localPlayerNumber;
            networkView.RPC("AddNewPlayer", player, computer, playerNumber);
        }
		if(menuManager.gameState == MenuManager.GameState.Game)
		{
			networkView.RPC("LoadMap", player, mapToUse.name);
			networkView.RPC("initializeGame", player);	
		}
		for(var i = 0; i < world.transform.FindChild("Bullets").childCount; i++)
		{
			networkView.RPC("spawnObject", player, world.transform.FindChild("Bullets").GetChild(i).position, world.transform.FindChild("Bullets").GetChild(i).rotation.eulerAngles, world.transform.FindChild("Bullets").GetChild(i).name, "testBullet", "World/Bullets");
			networkView.RPC ("setBulletVelocity", player, world.transform.FindChild("Bullets").GetChild(i).rigidbody.velocity, "World/Bullets/"+world.transform.FindChild("Bullets").GetChild(i).name);
			networkView.RPC ("setObjectGravity", player, world.transform.FindChild("Bullets").GetChild(i).GetComponent<ObjectStats>().grav, "World/Bullets/"+world.transform.FindChild("Bullets").GetChild(i).name);		
		}
		
		
		
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        //remove the map
		RemoveMap ();
		
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
	
	
	
	[RPC]
	void initializeGame()
	{
		//load map
		LoadMap(mapToUse.name);
		//move all players to spawn
		if(Network.peerType == NetworkPeerType.Server)
		{
			foreach(NetPlayer player in playerManager.players)
			{
				respawnPlayer(player.name);
			}
		}
		//switch to player camera
		if(playerManager.localPlayers.Count > 0)
		{
			playerManager.setToLocalCameras();
			playerManager.UpdateCameraSplit();	
		}
		playerManager.RevealPlayers();
	}
	
	void respawnPlayer(string name) //called on server, sets the players position in random area around spawn
	{
		Vector3 newPos = mapToUse.transform.FindChild("Spawn").position;
		world.transform.FindChild("RootTeam/" + name).transform.position = newPos;
		networkView.RPC("setPlayerPos", RPCMode.Others, name, newPos);
	}
	[RPC]
	void setPlayerPos(string name, Vector3 pos) //called on clients, copies players location from server
	{
		world.transform.FindChild("RootTeam/" + name).transform.position = pos;
	}
	
	
	[RPC]
	void LoadMap(string maptoLoad)
	{
		// instantiate the map on the local machine.
		//Ball spawning and some other junk
		loadedMap = Instantiate(Resources.Load("Maps/" + maptoLoad), Vector3.zero, Quaternion.identity) as GameObject;
		loadedMap.AddComponent<WorldBounds>();
		raceManager.init();
	}
	
	// making this an rpc seemed very reasonable to me
	[RPC]
	void RemoveMap()
	{
		Destroy (loadedMap);
	}
}
