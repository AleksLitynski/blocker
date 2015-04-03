using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/*	
 * 	This Class is responsible for adding all players to the map.
 * 	This is all done at the start of the game.
 * 	It also unloads players/map when a disconnect has occured.
 */
public class MapManager : BlockerObject
{
	//When the server starts, add a box that prevents player from falling off the world forever.
	void OnServerInitialized()
	{
		menuManager.bgMap.AddComponent<WorldBounds>();
	}
	
	//When a player asks to join the game...
    void OnPlayerConnected (NetworkPlayer player)
    {
		//send player characters to load... tell the player they are allowed to join. Give them a player Number.
        for (var i = 0; i < playerManager.players.Count; i++)
        {
            NetworkPlayer computer = playerManager.players[i].GetComponent<NetPlayer>().networkPlayer;
            int playerNumber = playerManager.players[i].GetComponent<NetPlayer>().localPlayerNumber;
            GetComponent<NetworkView>().RPC("AddNewPlayer", player, computer, playerNumber);
        }
		if(menuManager.gameState == MenuManager.GameState.Game) //If you are in game, tell them to load the current map.
		{
			GetComponent<NetworkView>().RPC("LoadMap", player, menuManager.bgMap.name);
			GetComponent<NetworkView>().RPC("initializeGame", player, menuManager.bgMap.name);	
		}
		if(menuManager.gameState == MenuManager.GameState.Lobby) //Tell them to just load the map...
		{
			GetComponent<NetworkView>().RPC("LoadMap", player, menuManager.bgMap.name);
		}
		for(var i = 0; i < world.transform.FindChild("Bullets").childCount; i++) //Tell them about all bullets (currently only dynamic content) they must position.
		{
			GetComponent<NetworkView>().RPC("spawnObject", player, world.transform.FindChild("Bullets").GetChild(i).position, world.transform.FindChild("Bullets").GetChild(i).rotation.eulerAngles, world.transform.FindChild("Bullets").GetChild(i).name, "testBullet", "World/Bullets");
			GetComponent<NetworkView>().RPC ("setBulletVelocity", player, world.transform.FindChild("Bullets").GetChild(i).GetComponent<Rigidbody>().velocity, "World/Bullets/"+world.transform.FindChild("Bullets").GetChild(i).name);
			GetComponent<NetworkView>().RPC ("setObjectGravity", player, world.transform.FindChild("Bullets").GetChild(i).GetComponent<ObjectStats>().grav, "World/Bullets/"+world.transform.FindChild("Bullets").GetChild(i).name);		
		}
		
		
		
    }
	
	//When YOU disconnect from the server
    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        //remove the map
		RemoveMap ();
		
        //remove all characters
        playerManager.players = new List<NetPlayer>();
        playerManager.localPlayers = new List<NetPlayer>();
        Transform rootTeam = gameObject.transform.Find("RootTeam");
        for (int i = 0; i < rootTeam.childCount; i++)
        {
            GameObject toGo = rootTeam.GetChild(i).gameObject;
            Destroy(toGo);
        }
    }
	
	//When SOMEONE ELSE disconnects
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        for (int i = 0; i < playerManager.players.Count; i++)
        {
            if (playerManager.players[i].networkPlayer == player)
            {
                GetComponent<NetworkView>().RPC("RemovePlayer", RPCMode.Others, player, playerManager.players[i].localPlayerNumber); //Tell everyone to remove them.
                if (Network.peerType == NetworkPeerType.Server)
                {
                    playerManager.RemovePlayer(player, playerManager.players[i].localPlayerNumber); //Remove them yourself
                    i--;
                } 
            }
        }
    }
	
	//This tells everyone to start the game.
	[RPC]
	void initializeGame(string mapName)
	{
		//load map
		LoadMap(mapName);
		//move all players to spawn
		if(Network.peerType == NetworkPeerType.Server)
		{
			foreach(NetPlayer player in playerManager.players)
			{
				respawnPlayer(player.player.name);
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
	
	public void respawnPlayer(string name) //called on server, sets the players position in random area around spawn
	{
		Transform spawnArea = menuManager.bgMap.transform.FindChild("Spawn").transform;
		GameObject player = findNetPlayerNamed(name);
		Vector3 spawnLocation = spawnArea.transform.position;
		Quaternion spawnRotation = spawnArea.transform.rotation;
		Vector3 down = -spawnArea.transform.up;
		int attempts = 0;
		while(attempts < 50)
		{
			//get random point in spawn area.
			spawnLocation = new Vector3(Random.Range(spawnArea.position.x - spawnArea.localScale.x/2 ,spawnArea.position.x + spawnArea.localScale.x/2), 
									Random.Range(spawnArea.position.y - spawnArea.localScale.y/2 ,spawnArea.position.y + spawnArea.localScale.y/2),  
									Random.Range(spawnArea.position.z - spawnArea.localScale.z/2 ,spawnArea.position.z + spawnArea.localScale.z/2));
			
			//check if putting the player there causes a collision
			if(Physics.OverlapSphere(spawnLocation, player.transform.Find ("Doll").GetComponent<Collider>().bounds.max.y).Length <= 1)
			{
				break;
			}
			player.transform.Find("Doll").rotation = spawnArea.transform.rotation;
			attempts++;
			
		}
		//The player is a etheral entity, the "doll" is the body you see in game. It got updated a few times, the logical divide helps.
		player.transform.Find ("Doll").GetComponent<Rigidbody>().velocity = new Vector3();
		
		player.transform.Find ("Doll").GetComponent<ObjectStats>().grav = down * 9.81f;
		
		GetComponent<NetworkView>().RPC("setPlayerPos", RPCMode.All, name, spawnLocation, spawnRotation);
		 
	}
	
	//Tells the client to position the player initally.
	[RPC]
	void setPlayerPos(string name, Vector3 pos, Quaternion rot) //called on clients, copies players location from server
	{
		GameObject player = findNetPlayerNamed(name);
		player.transform.Find("Doll").transform.position = pos;
		player.transform.Find("Doll").transform.rotation = rot;
	}
	
	//Loads a given map. Self documenting name.
	[RPC]
	void LoadMap(string maptoLoad)
	{
		// instantiate the map on the local machine.
		//Ball spawning and some other junk
		maptoLoad = maptoLoad.Remove(maptoLoad.Length-7);
		DestroyImmediate (menuManager.bgMap);
		GameObject newMap = Instantiate(Resources.Load("Maps/" + maptoLoad), Vector3.zero, Quaternion.identity) as GameObject;
		newMap.AddComponent<WorldBounds>();
		if(!newMap.GetComponent<NetworkView>())
		{
			newMap.AddComponent<NetworkView>();
			newMap.GetComponent<NetworkView>().stateSynchronization = NetworkStateSynchronization.Off; //Disable the sync. We only communicate using the world.RPC function.
			newMap.GetComponent<NetworkView>().observed = null;
		}
		menuManager.bgMap = newMap;
		
	}
	
	// making this an rpc seemed very reasonable to me
	[RPC]
	void RemoveMap()
	{
		Destroy (menuManager.bgMap);
	}
	
	//This looks up a player object from their string name. 
	//I'm sure a Foldl could have sufficed.
	GameObject findNetPlayerNamed(string name)
	{
		foreach(NetPlayer player in playerManager.players)
		{
			if(player.player.name == name )
			{
				return player.player.gameObject;	
			}
		}
		throw new System.Exception("couldn't find player");
	}
}
