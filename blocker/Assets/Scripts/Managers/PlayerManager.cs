using UnityEngine;
using System.Linq;
using System.Collections.Generic;


/* 
 * This deal with individual players joining the game. 
 * The Server/Client are already connected. Many players may still join the game, because each machine can support up to five (take that, halo!) player split screen.
 * 
 * A player is identifed by their computer number and their player number. That is, what machine in the game, and what player on the machine.
 */


//asks server to add new player
//tells all players to add new player
//adds new player
//lays out splitscreen players
public class PlayerManager : BlockerObject 
{

    public GameObject playerModel;

    public List<NetPlayer> players = new List<NetPlayer>();			//This is all players in the game.
    public List<NetPlayer> localPlayers = new List<NetPlayer>();	//These are just the local players.
	
	public override void Start()
	{
		base.Start();	//We need to call BlockerObjects start function to get some references linked up.
	}
	
	//We can only send the server a request. It will allow or deny that more players can join the game.
    [RPC]
    public void AddNewPlayerRequest(NetworkMessageInfo info)
    {
        int lowestAvailableValue = 0;   //find the lowest available localPlayerNumber of the players on the machine asking to add a player. 
        int i = 0;
        while(i < players.Count)
        {
            if (players[i].networkPlayer == info.sender)
            {
                if (players[i].localPlayerNumber == lowestAvailableValue) //The new player will ask for the lowest number available.
                {
                    lowestAvailableValue++;
                    i = 0;
                    continue;
                }
            }
            i++;
            
        }
		if(lowestAvailableValue < 5)//this is the max local players on the map
		{
			GetComponent<NetworkView>().RPC("AddNewPlayer", RPCMode.Others, info.sender, lowestAvailableValue);
        	if (Network.peerType == NetworkPeerType.Server) this.AddNewPlayer(info.sender, lowestAvailableValue);	//The server doesn't hear its own RPCs, we have to call the functions locally.
		}
    }
	
	//called on the client machines to tell them to add a new player to the game.
	//This is called once the server has approved the connection.
    [RPC]
    void AddNewPlayer(NetworkPlayer computer, int numOnComputer)
    {
        GameObject newPlayer = Instantiate(playerModel, Vector3.zero, Quaternion.identity) as GameObject;
        newPlayer.name = "player " + computer.ToString() + "-" + numOnComputer;
        newPlayer.transform.Find("Doll").GetComponent<NetPlayer>().networkPlayer = computer;
        newPlayer.transform.Find("Doll").GetComponent<NetPlayer>().localPlayerNumber = numOnComputer;
        newPlayer.transform.parent = GameObject.Find("/World/RootTeam").transform;
		
		newPlayer.transform.position = GameObject.Find("Spawn").transform.position;
		

        if (numOnComputer == 0) //Default is: First player is Keyboard player, all other players use a controller.
        {
            newPlayer.transform.Find("Doll").GetComponent<NetPlayer>().KeyboardPlayer = true;
        }
        else
        {
            newPlayer.transform.Find("Doll").GetComponent<NetPlayer>().ControllerNumber = numOnComputer-1;
        }

        players.Add(newPlayer.transform.Find("Doll").GetComponent<NetPlayer>());
        if (computer == Network.player)
        {
            localPlayers.Add(newPlayer.transform.Find("Doll").GetComponent<NetPlayer>()); //if It is a local player, add another division to the camera.
            UpdateCameraSplit();
        }
        newPlayer.transform.Find("Camera").GetComponent<Camera>().enabled = false; //Turn off the camera, for now.
        
		HidePlayers();
    }
	
	
	//Ask to leave the game. Mostly used on misclicks. The server won't wait for this message if a player disconnects
    [RPC]
    public void RemovePlayerRequest(int numOnComputer, NetworkMessageInfo info)
    {
        GetComponent<NetworkView>().RPC("RemovePlayer", RPCMode.Others, info.sender, numOnComputer);
        if (Network.peerType == NetworkPeerType.Server) this.RemovePlayer(info.sender, numOnComputer);
    }
	
	//Tells all clients to remove a given player.
	//Players are identifed by two things: 
	//computer: The machine the player is on
	//numOnComputer: the player to be removed on the computer.
    [RPC]
    public void RemovePlayer(NetworkPlayer computer, int numOnComputer)
    {
		
        for (int i = 0; i < players.Count; i++)
        {
            NetPlayer player = players[i].GetComponent("NetPlayer") as NetPlayer;
            if (player.localPlayerNumber == numOnComputer && player.networkPlayer == computer)
            {
                Destroy(players[i].gameObject);
                if (player.networkPlayer == Network.player)
                {
                    localPlayers.Remove(players[i]);
                }
                players.Remove(players[i]);
                UpdateCameraSplit();
                break;
            }
        }
		if(localPlayers.Count == 0)
		{
			setToWorldCamera();	
		}
    }
	
	//This makes all players visable.
	public void RevealPlayers()
	{
		foreach (NetPlayer player in players)
		{
			player.transform.Find ("Model").gameObject.SetActive(true);
			player.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
	//Opposite. Hides all players.
	public void HidePlayers()
	{
		foreach (NetPlayer player in players)
		{
			player.transform.Find ("Model").gameObject.SetActive(false);
			player.GetComponent<Rigidbody>().isKinematic = true;
		}
	}
	
	//This switches to a single camera that shows the map. This is used in menus.
	public void setToWorldCamera()
	{
		foreach(NetPlayer player in localPlayers)
		{
			player.playerCamera.GetComponent<Camera>().enabled = false;
		}
		world.GetComponent<Camera>().enabled = true;
	}
	//Durring the game, we use one camera per local player (local, as opposed to on a remote computer)
	public void setToLocalCameras()
	{
		foreach(NetPlayer player in localPlayers)
		{
			player.playerCamera.GetComponent<Camera>().enabled = true;
		}
		world.GetComponent<Camera>().enabled = false;
	}
	
	//This functioned used to be much more complex.
	//Now, its just a lookup table. 
	//If there is 1 player, show X amount of screen. If there are two, show Y amount. Etc
	public void UpdateCameraSplit()
	{
		if(localPlayers.Count == 1)
		{
			
	       	localPlayers[0].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
		}
		else if(localPlayers.Count == 2)
		{
			localPlayers[0].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1);
			localPlayers[1].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0.5f, 0, 0.5f, 1);
		}
		else if(localPlayers.Count == 3)
		{
			localPlayers[0].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0   , 0.5f   , 0.5f, 0.5f);
			localPlayers[1].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0.5f, 0.5f   , 0.5f, 0.5f);
			localPlayers[2].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0   , 0f     , 1   , 0.5f);
		}
		else if(localPlayers.Count == 4)
		{
			localPlayers[0].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0,    0.5f, 0.5f, 0.5f);
			localPlayers[1].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
			
			localPlayers[2].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0,    0f, 0.5f, 0.5f);
			localPlayers[3].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
		}
		else if(localPlayers.Count == 5)
		{
			localPlayers[0].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0,    2f/3f, 0.5f, 1f/3f);
			localPlayers[1].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0.5f, 2f/3f, 0.5f, 1f/3f);
			
			localPlayers[2].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0,    1f/3f, 0.5f, 1f/3f);
			localPlayers[3].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0.5f, 1f/3f, 0.5f, 1f/3f);
			
			localPlayers[4].GetComponent<NetPlayer>().playerCamera.GetComponent<Camera>().rect = new Rect(0,    0, 1, 1f/3f);
		}
		
		foreach(NetPlayer player in localPlayers) //Once we divide the screen, we need to set up the 3d compass overlays again.
		{
			player.initCompassLayer();	
		}
		
	}
	
	/* The old camera split code has been deleted. Please review the REPOs history to get it back.
	 * It was good stuff, but the UI overlay confused matters.
	 */
}