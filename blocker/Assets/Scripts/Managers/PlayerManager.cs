using UnityEngine;
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
        (newPlayer.GetComponent("NetPlayer") as NetPlayer).networkPlayer = computer;
        (newPlayer.GetComponent("NetPlayer") as NetPlayer).localPlayerNumber = numOnComputer;
        newPlayer.transform.parent = GameObject.Find("/World/RootTeam").transform;
		
		newPlayer.transform.position = GameObject.Find("Spawn").transform.position;
		

        if (numOnComputer == 0)
        {
            (newPlayer.GetComponent("NetPlayer") as NetPlayer).KeyboardPlayer = true;
        }
        else
        {
            (newPlayer.GetComponent("NetPlayer") as NetPlayer).ControllerNumber = numOnComputer-1;
        }

        players.Add((newPlayer.GetComponent("NetPlayer") as NetPlayer));
        if (computer == Network.player)
        {
            localPlayers.Add((newPlayer.GetComponent("NetPlayer") as NetPlayer));
            UpdateCameraSplit();
        }
        GameObject.Find(newPlayer.name + "/Arms/Camera").camera.enabled = false;
        
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
			player.GetComponent<MeshRenderer>().enabled = true;
			player.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
	public void HidePlayers()
	{
		foreach (NetPlayer player in players)
		{
			player.GetComponent<MeshRenderer>().enabled = false;
			player.GetComponent<Rigidbody>().isKinematic = true;
		}
	}
	
	public void setToWorldCamera()
	{
		foreach(NetPlayer player in localPlayers)
		{
			player.transform.FindChild("Arms/Camera").camera.enabled = false;
		}
		world.camera.enabled = true;
	}
	public void setToLocalCameras()
	{
		foreach(NetPlayer player in localPlayers)
		{
			player.transform.FindChild("Arms/Camera").camera.enabled = true;
		}
		world.camera.enabled = false;
	}
	
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
                    (GameObject.Find(localPlayers[pos].name + "Arms/Camera").camera as Camera).rect = new Rect(j * width, i * height, width, height);
                }
                else
                {
                    (GameObject.Find(localPlayers[pos].name + "Arms/Camera").camera as Camera).rect = new Rect(i * height, j * width, height, width);
                }
                pos++;
            }
        }
    }
}