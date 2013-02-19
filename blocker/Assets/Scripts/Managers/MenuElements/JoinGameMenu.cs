using UnityEngine;
using System.Collections;

public class JoinGameMenu
{

	public static void generateGUI(MenuManager menuManager)
	{
		MasterServer.RequestHostList(menuManager.gameName);
			
			GUILayout.BeginArea(new Rect(100, Screen.height/8, Screen.width -200, Screen.height/2));
				GUILayout.BeginVertical();
				
					// actually get games from the master server.
					HostData[] availableHosts = MasterServer.PollHostList();
					
					// actually list games from the master server.
					GUILayout.Box("Available Servers", GUILayout.MinWidth(Screen.width - 200));
						foreach (HostData host in availableHosts)
						{
							GUILayout.BeginHorizontal();
								GUILayout.Label(host.gameName + " (" + host.connectedPlayers + " / " + host.playerLimit + ") - " + host.comment);
								if (GUILayout.Button("Connect", GUILayout.MaxWidth(Screen.width/8)))
								{
									Network.Connect(host);
									menuManager.ChangeState(MenuManager.GameState.Lobby);
								}	
							GUILayout.EndHorizontal();
						}
				GUILayout.EndVertical();
				
				// buttons at the bottom (refresh and back)
				GUILayout.BeginHorizontal();
					if (GUILayout.Button("Refresh List",GUILayout.MaxWidth(200))){MasterServer.RequestHostList(menuManager.gameName);}
					GUILayout.Label("",GUILayout.MaxWidth(Screen.width-600));
					if (GUILayout.Button("Back to Main Menu",GUILayout.MaxWidth(200))){menuManager.ChangeState(MenuManager.GameState.MainMenu);}
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
	}
}
