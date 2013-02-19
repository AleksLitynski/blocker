using UnityEngine;
using System.Collections;

public class LobbyMenu : BlockerObject
{

	public static void generateGUI(MenuManager menuManager)
	{
			GUILayout.BeginArea(new Rect(0,0,Screen.width, Screen.height));
				GUILayout.BeginHorizontal();
					GUILayout.BeginArea(new Rect(Screen.width*1/20,Screen.height*1/20,Screen.width/6, Screen.height*15/20));
						if (GUILayout.Button("New Player", GUILayout.MinWidth(Screen.width/6)))
				        {
				            if (Network.peerType == NetworkPeerType.Client) 
							{
								//Debug.Log("you clicked it as a clinet");
								menuManager.networkView.RPC("AddNewPlayerRequest", RPCMode.Server);
							}
				            if (Network.peerType == NetworkPeerType.Server) menuManager.playerManager.AddNewPlayerRequest(new NetworkMessageInfo());
				        }
					GUILayout.EndArea();
					
					// provide add/drop ability
					GUILayout.BeginArea(new Rect(Screen.width*1/20,Screen.height*2/20,Screen.width*2/20, Screen.height*15/20));
						GUILayout.BeginVertical();
						
						foreach(NetPlayer player in menuManager.playerManager.players)
						{
							if(player.networkPlayer.ToString() == Network.player.ToString())
							{
								if (GUILayout.Button("Drop", GUILayout.MinWidth(Screen.width*2/20)))
					            {
					                menuManager.networkView.RPC("RemovePlayerRequest", RPCMode.Server, player.localPlayerNumber);
					                if (Network.peerType == NetworkPeerType.Server) menuManager.playerManager.RemovePlayerRequest(player.localPlayerNumber, new NetworkMessageInfo());
					            }
							}
							else
							{
								GUILayout.Label("");
							}
						}
						GUILayout.EndVertical();
					GUILayout.EndArea();
					// list player names
					GUILayout.BeginArea(new Rect(Screen.width*4/20,Screen.height*2/20,Screen.width*5/20, Screen.height*14/20));
						foreach(NetPlayer player in menuManager.playerManager.players)
						{
							GUILayout.Label(player.player.name);
						}
					GUILayout.EndArea();
					// list player scores
					GUILayout.BeginArea(new Rect(Screen.width*10/20,Screen.height*1.9f/20,Screen.width*3/20, Screen.height*14/20));
						foreach(NetPlayer player in menuManager.playerManager.players)
						{
							GUILayout.Label("" + player.playerStats.score);
						}
					GUILayout.EndArea();
					GUILayout.BeginArea(new Rect(Screen.width*13/20,Screen.height*1.9f/20,Screen.width*6/20, Screen.height*14/20));
		
						GUILayout.Label ("Description:");
						GUILayout.Label (menuManager.bgMap.GetComponent<GameManager>().gameDescription);
		
					GUILayout.EndArea();
				GUILayout.EndHorizontal();
				GUILayout.BeginArea(new Rect(Screen.width*(1.0f/6), Screen.height*17/20, Screen.width*2/3, Screen.height*2/20));
					GUILayout.BeginHorizontal();
						// actually initialize the game state.
						if (Network.peerType == NetworkPeerType.Server)
						{
							if (GUILayout.Button("Start", GUILayout.MaxWidth(200)))
							{							
								// buffer an RPC telling everyone the game has started (join in progress)
								menuManager.networkView.RPC("ChangeState", RPCMode.AllBuffered, menuManager.GameCode);
								menuManager.networkView.RPC("initializeGame", RPCMode.All, menuManager.bgMap.name);
							}
						}
						else
						{
							if (GUILayout.Button("Vote to Start", GUILayout.MaxWidth(200)))
							{
								//LOL JOKES ON YOU PLAYER!
							}
						}
						GUILayout.Label("", GUILayout.MaxWidth(Screen.width*2/3-400));
						if (GUILayout.Button("Back to Main Menu", GUILayout.MaxWidth(200)))
						{
							if(Network.peerType == NetworkPeerType.Server)
							{
								// return yourself to the main menu and everyone else to the joingame menu.
								menuManager.ChangeState (MenuManager.GameState.MainMenu);
								menuManager.networkView.RPC("ChangeState", RPCMode.Others, menuManager.JoinGameCode);
								MasterServer.UnregisterHost();
							}
							else
							{
								Network.Disconnect();
								menuManager.ChangeState (MenuManager.GameState.MainMenu);
							}
						}
					GUILayout.EndHorizontal();
				GUILayout.EndArea();
			GUILayout.EndArea();
	}
}
