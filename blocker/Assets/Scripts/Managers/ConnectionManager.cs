using UnityEngine;
using System.Collections.Generic;

/* This controls the games connection to the server.
 * 
 * It doesn't worry about how many players are on board, just throwing across the innital connection
 * 
 * The connection code is embedded in the GUI code. Not great, but fairly clean.
 */

public class ConnectionManager : BlockerObject 
{
	//public string serverAddress = "bigwhite.student.rit.edu";
	string gameName = "Blocker v 0.1";

    void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            GUILayout.BeginHorizontal();
				//ServerGUI();
				//ClientGUI();
			GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
				DisconnectGUI();
			GUILayout.EndHorizontal();
        }
	}
	
	void DisconnectGUI()
	{
		
		if(GUI.Button(new Rect(0, 0, 120, 20), "Return to Lobby"))
		{
			Network.Disconnect(); //Disconnects!!
			Application.LoadLevel(Application.loadedLevel); //Loads a background map
		}
		
	}
	
	
	//Presents a "HOST SERVER" button. Creates a server on request
	string hostedGameName = "Default Game Name";
	string hostedGameDescription = "Default Description";
	void ServerGUI()
	{
		
		GUILayout.BeginVertical();
			hostedGameName = GUILayout.TextField(hostedGameName, GUILayout.MaxWidth (200));
			hostedGameDescription = GUILayout.TextField(hostedGameDescription, GUILayout.MaxWidth(200));
			if(GUILayout.Button ("Host", GUILayout.MinWidth(50)))
			{
				createServer(hostedGameName, hostedGameDescription);
			}
		GUILayout.EndVertical();
		
	}

 	void createServer(string hostName, string description)
	{
		//Registers the master server. 
		//Uses a random ID to prevents collision between version while debugging
		Network.InitializeServer(32, Random.Range(2000,40000), !Network.HavePublicAddress()); 
		MasterServer.RegisterHost(gameName, hostName, description);
		
	}
	
	void ClientGUI()
	{
		MasterServer.RequestHostList(gameName);
		GUILayout.BeginVertical();
		HostData[] availableHosts = MasterServer.PollHostList();
			GUILayout.Box("Available Servers", GUILayout.MinWidth(Screen.width - 200));
			foreach (HostData host in availableHosts) //Itterate all available servers and present them.
			{
				connectionRow(host);
			}
		GUILayout.EndVertical();
		
	}
	
	//Shows a single row of valid server.
	void connectionRow(HostData host)
	{
		GUILayout.BeginHorizontal();
			GUILayout.Label(host.gameName + " (" + host.connectedPlayers + " / " + host.playerLimit + ")");
			GUILayout.Label(host.comment);
			if (GUILayout.Button("Connect", GUILayout.MaxWidth(Screen.width/10)))
			{
				Network.Connect(host);	//Uses unitys server connection protocal		
			}				
		GUILayout.EndHorizontal();
	}
	
	/*void drawWindow(int winID)
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            serverAddress = GUILayout.TextField(serverAddress, 25);
            if (GUILayout.Button("start"))
            {
                Network.InitializeSecurity();
                Network.InitializeServer(32, 44344, false);
            }

            if (GUILayout.Button("join"))
            {
                Network.Connect(serverAddress, 44344);
            }
        }
        else
        {
            if (GUILayout.Button("disconnect"))
            {
                Network.Disconnect();
            }
        }
    }*/
	


    
}



