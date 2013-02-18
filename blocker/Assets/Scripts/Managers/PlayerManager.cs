using UnityEngine;
using System.Linq;
using System.Collections.Generic;

//asks server to add new player
//tells all players to add new player
//adds new player
//lays out splitscreen players
public class PlayerManager : BlockerObject 
{

    public GameObject playerModel;

    public List<NetPlayer> players = new List<NetPlayer>();
    public List<NetPlayer> localPlayers = new List<NetPlayer>();
	
	public override void Start()
	{
		base.Start();
	}
	
    [RPC]
    public void AddNewPlayerRequest(NetworkMessageInfo info)
    {
        int lowestAvailableValue = 0;   //find the lowest available localPlayerNumber of the players on the machine asking to add a player. 
        int i = 0;
        while(i < players.Count)
        {
            if (players[i].networkPlayer == info.sender)
            {
                if (players[i].localPlayerNumber == lowestAvailableValue)
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
			networkView.RPC("AddNewPlayer", RPCMode.Others, info.sender, lowestAvailableValue);
        	if (Network.peerType == NetworkPeerType.Server) this.AddNewPlayer(info.sender, lowestAvailableValue);	
		}
    }
	
	//called on the client machines to tell them to add a new player to the game
    [RPC]
    void AddNewPlayer(NetworkPlayer computer, int numOnComputer)
    {
        GameObject newPlayer = Instantiate(playerModel, Vector3.zero, Quaternion.identity) as GameObject;
        newPlayer.name = "player " + computer.ToString() + "-" + numOnComputer;
        newPlayer.transform.Find("Doll").GetComponent<NetPlayer>().networkPlayer = computer;
        newPlayer.transform.Find("Doll").GetComponent<NetPlayer>().localPlayerNumber = numOnComputer;
        newPlayer.transform.parent = GameObject.Find("/World/RootTeam").transform;
		
		newPlayer.transform.position = GameObject.Find("Spawn").transform.position;
		

        if (numOnComputer == 0)
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
            localPlayers.Add(newPlayer.transform.Find("Doll").GetComponent<NetPlayer>());
            UpdateCameraSplit();
        }
        newPlayer.transform.Find("Camera").camera.enabled = false;
        
		HidePlayers();
    }
	
	
	
    [RPC]
    public void RemovePlayerRequest(int numOnComputer, NetworkMessageInfo info)
    {
        networkView.RPC("RemovePlayer", RPCMode.Others, info.sender, numOnComputer);
        if (Network.peerType == NetworkPeerType.Server) this.RemovePlayer(info.sender, numOnComputer);
    }

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
	
	public void RevealPlayers()
	{
		foreach (NetPlayer player in players)
		{
			player.transform.Find ("Model").gameObject.active = true;
			player.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
	public void HidePlayers()
	{
		foreach (NetPlayer player in players)
		{
			player.transform.Find ("Model").gameObject.active = false;
			player.GetComponent<Rigidbody>().isKinematic = true;
		}
	}
	
	public void setToWorldCamera()
	{
		foreach(NetPlayer player in localPlayers)
		{
			player.playerCamera.camera.enabled = false;
		}
		world.camera.enabled = true;
	}
	public void setToLocalCameras()
	{
		foreach(NetPlayer player in localPlayers)
		{
			player.playerCamera.camera.enabled = true;
		}
		world.camera.enabled = false;
	}
	
	
	public void UpdateCameraSplit()
	{
		if(localPlayers.Count == 1)
		{
			
	       	localPlayers[0].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0, 0, 1, 1);
		}
		else if(localPlayers.Count == 2)
		{
			localPlayers[0].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0, 0, 0.5f, 1);
			localPlayers[1].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0.5f, 0, 0.5f, 1);
		}
		else if(localPlayers.Count == 3)
		{
			localPlayers[0].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0   , 0.5f   , 0.5f, 0.5f);
			localPlayers[1].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0.5f, 0.5f   , 0.5f, 0.5f);
			localPlayers[2].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0   , 0f     , 1   , 0.5f);
		}
		else if(localPlayers.Count == 4)
		{
			localPlayers[0].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0,    0.5f, 0.5f, 0.5f);
			localPlayers[1].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
			
			localPlayers[2].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0,    0f, 0.5f, 0.5f);
			localPlayers[3].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
		}
		else if(localPlayers.Count == 5)
		{
			localPlayers[0].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0,    2f/3f, 0.5f, 1f/3f);
			localPlayers[1].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0.5f, 2f/3f, 0.5f, 1f/3f);
			
			localPlayers[2].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0,    1f/3f, 0.5f, 1f/3f);
			localPlayers[3].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0.5f, 1f/3f, 0.5f, 1f/3f);
			
			localPlayers[4].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(0,    0, 1, 1f/3f);
		}
		
	}
	
	
	/*
    public void UpdateCameraSplit()
    {
        //figure out screen ratios
        List<int> rows = new List<int>();
        int startingRow = 0;
        for (int i = 0; i <= localPlayers.Count; i++)
        {
            bool addIntoARow = false;
            for (int k = 0; k < rows.Count; k++)
            {
                var currentRow = k + startingRow;
                if (currentRow >= rows.Count)
                {
                    currentRow = 0;
                }
                if (rows[currentRow] < rows.Count)
                {
                    rows[currentRow]++;
                    addIntoARow = true;
                    break;
                }
            }
            if (!addIntoARow)
            {
                rows.Add(0);
                for (int j = 0; j < rows.Count; j++)
                {
                    rows[j] = 0;
                }
                i = 0;
                startingRow = rows.Count - 1;
            }
            startingRow++;
            if(startingRow == rows.Count)
            {
                startingRow = 0;
            }


        }
        int v = 0;
        while (v < rows.Count)
        {
            if (rows[v] == 0)
            {
                rows.Remove(0);
            }
            else
            {
                v++;
            }
        }


        //layout screens based on calculations
        bool favorHorizontal = false;
        if (Screen.width > Screen.height)
        {
            favorHorizontal = true;
        }
        int pos = 0;
        for (int i = 0; i < rows.Count; i++)
        {
            for (int j = 0; j < rows[i]; j++)
            {

                float width = 1 / (float)rows[i]; 
                
                float height = 1 / (float)rows.Count;




                if (favorHorizontal)
                {
                    localPlayers[pos].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(j * width, i * height, width, height);
                }
                else
                {
                    localPlayers[pos].GetComponent<NetPlayer>().playerCamera.camera.rect = new Rect(i * height, j * width, height, width);
                }
                pos++;
            }
        }
    }*/
}