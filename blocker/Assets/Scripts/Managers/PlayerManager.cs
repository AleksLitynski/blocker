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
    void AddNewPlayerRequest(NetworkMessageInfo incomingInfo)
    {
        NetworkMessageInfoLocalWrapper info = new NetworkMessageInfoLocalWrapper(incomingInfo);
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
		networkView.RPC("AddNewPlayer", RPCMode.Others, info.sender, lowestAvailableValue);
        if (Network.peerType == NetworkPeerType.Server) this.AddNewPlayer(info.sender, lowestAvailableValue);
    }
	void AddNewPlayerRequest()
	{
		AddNewPlayerRequest(new NetworkMessageInfo());
	}

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
        else
        {
            GameObject.Find(newPlayer.name + "/Arms/Camera").active = false;
        }
		
		if(localPlayers.Count > 0)
		{
			gameObject.camera.enabled = false;
		}
    }
	
	
	
    [RPC]
    public void RemovePlayerRequest(int numOnComputer, NetworkMessageInfo incomingInfo)
    {
        NetworkMessageInfoLocalWrapper info = new NetworkMessageInfoLocalWrapper(incomingInfo);
        networkView.RPC("RemovePlayer", RPCMode.Others, info.sender, numOnComputer);
        if (Network.peerType == NetworkPeerType.Server) this.RemovePlayer(info.sender, numOnComputer);
    }
	public void RemovePlayerRequest(int numOnComputer)
    {
        RemovePlayerRequest(numOnComputer, new NetworkMessageInfo());
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
			gameObject.camera.enabled = true;	
		}
    }

    void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Server || Network.peerType == NetworkPeerType.Client)
        {
            GUILayout.Window(1, new Rect(Screen.width - 200, 0, 200, localPlayers.Count * 35 + 35), drawWindow2, "Add/Remove Player");
            if (players.Count > 0)
            {
                GUILayout.Window(3, new Rect(Screen.width - 300, 0, 100, localPlayers.Count * 20 + 20), drawWindow3, "Players");
            }
        }
        
    }

    void drawWindow3(int a)
    {
        for (int i = 0; i < players.Count; i++)
        {
			GUILayout.BeginHorizontal();
            	GUILayout.Label(players[i].gameObject.name);
				GUILayout.Label(""+players[i].playerStats.score);
			GUILayout.EndHorizontal();
        }
    }
    void drawWindow2(int windowID)
    {
        if (GUILayout.Button("New Player"))
        {
            if (Network.peerType == NetworkPeerType.Client) networkView.RPC("AddNewPlayerRequest", RPCMode.Server);
            if (Network.peerType == NetworkPeerType.Server) this.AddNewPlayerRequest();
        }
        for (int i = 0; i < localPlayers.Count; i++)
        {
			GUILayout.BeginHorizontal();
	            if (GUILayout.Button("Drop Player " + localPlayers[i].localPlayerNumber))
	            {
	                networkView.RPC("RemovePlayerRequest", RPCMode.Server, localPlayers[i].localPlayerNumber);
	                if (Network.peerType == NetworkPeerType.Server) this.RemovePlayerRequest(localPlayers[i].localPlayerNumber);
	            }
				if(GUILayout.Button("KB " + localPlayers[i].localPlayerNumber))
				{
					foreach(NetPlayer player in localPlayers)
					{
						player.KeyboardPlayer = false;
					}
					localPlayers[i].KeyboardPlayer = true;
				}
			GUILayout.EndHorizontal();
        }
    }

    void UpdateCameraSplit()
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

    //1) if all rows have rows == cols
        //add new row 
}