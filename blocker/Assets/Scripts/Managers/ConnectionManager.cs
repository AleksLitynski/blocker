using UnityEngine;
using System.Collections.Generic;

public class ConnectionManager : BlockerObject 
{
	//public string serverAddress = "bigwhite.student.rit.edu";
	string gameName = "Blocker v 1.0.134523576";

    void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            GUILayout.BeginHorizontal();
				ServerGUI();
				ClientGUI();
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
			Network.Disconnect();
		}
		
	}
	
	
	string hostedGameName = "Default Game Name";
	string hostedGameDescription = "Default Description";
	void ServerGUI()
	{
		Debug.Log("Is Called");
		
		GUILayout.BeginVertical();
			hostedGameName = GUILayout.TextField(hostedGameName, GUILayout.MinWidth (100));
			hostedGameDescription = GUILayout.TextField(hostedGameDescription, GUILayout.MinWidth(200));
			if(GUILayout.Button ("Host", GUILayout.MinWidth(50)))
			{
				createServer(hostedGameName, hostedGameDescription);
			}
		GUILayout.EndVertical();
		
	}

 	void createServer(string hostName, string description)
	{
		Network.InitializeServer(32, Random.Range(2000,40000), !Network.HavePublicAddress());
		MasterServer.RegisterHost(gameName, hostName, description);
		
	}
	
	void ClientGUI()
	{
		MasterServer.RequestHostList(gameName);
		GUILayout.BeginVertical();
		HostData[] availableHosts = MasterServer.PollHostList();
			GUILayout.Box("Available Servers", GUILayout.MinWidth(Screen.width));
			foreach (HostData host in availableHosts)
			{
				connectionRow(host);
			}
		GUILayout.EndVertical();
		
	}
	
	void connectionRow(HostData host)
	{
		GUILayout.BeginHorizontal();
			GUILayout.Label(host.gameName + " (" + host.connectedPlayers + " / " + host.playerLimit + ")");
			GUILayout.Label(host.comment);
			if (GUILayout.Button("Connect", GUILayout.MaxWidth(Screen.width/10)))
			{
				Network.Connect(host);			
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



