var playerModel : Transform;
var mapModel : Transform;


//add new player
function OnPlayerConnected(player: NetworkPlayer)
{
	GetComponent.<NetworkView>().RPC("startLoadingSelf",  RPCMode.AllBuffered, player.ToString());
}
@RPC
function startLoadingSelf(player:String)
{
	if(Network.player.ToString() == player)
	{
		addNewPlayer("player " + player);
	}
}

//add server player and load map
function OnServerInitialized()
{
	var newMapModel = Network.Instantiate(mapModel, transform.position, transform.rotation, 0);
	GetComponent.<NetworkView>().RPC("renameObject", RPCMode.AllBuffered, newMapModel.name, "Map");
	
	addNewPlayer("player " + Network.player.ToString());
}

//add player. Sets camera onto player
function addNewPlayer(playerName:String)
{
	var spawnPoint = GameObject.Find("Map/Spawn");
	var newPlayerModel = Network.Instantiate(playerModel, spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
	GetComponent.<NetworkView>().RPC("renameObject", RPCMode.AllBuffered, newPlayerModel.name, playerName);
	GameObject.Find("/World/PlayerCamera").GetComponent("CameraStarter").FocusCameraOnObject(playerName + "/Arms", new Vector3(-1.5, 1.8, -3.0), 10.0);
	
}



//remove player from remaining games
function OnPlayerDisconnected (player : NetworkPlayer)
{
	removePlayer(GameObject.Find("player " + player.ToString()), "network");
	Network.RemoveRPCs(player);
}

//remove map and players from local game
function OnDisconnectedFromServer(mode : NetworkDisconnection)
{
	Destroy(GameObject.Find("Map"));
	var players = GameObject.FindGameObjectsWithTag("Player");
	for(var player in players)
	{
		removePlayer(player, "local");
	}
}

//remove player
function removePlayer(player:GameObject, netOrLoc:String)
{
	if(netOrLoc == "network")
	{
		Network.Destroy(player);
	}
	else if(netOrLoc == "local")
	{
		if(player.GetComponent("GenericRPC") != null)
		{
			player.GetComponent("GenericRPC").FocusCameraOnObject(player.name, "World", 100.0, 100.0, 2.0, 30.0);
		}
		Destroy(player);
	}
}

