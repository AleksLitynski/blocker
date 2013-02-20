using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class GameMenu
{

	public static void generateGUI(MenuManager menuManager)
	{
		
		if(Network.peerType == NetworkPeerType.Server)
		{
			if(GUI.Button(new Rect(Screen.width - 100,0, 100, 20),"X All"))
			{
				menuManager.ChangeState (MenuManager.LobbyCode);
				menuManager.networkView.RPC("changeState", RPCMode.Others, MenuManager.LobbyCode);
			}
		}
		else
		{
			if(GUI.Button(new Rect(Screen.width - 50,0, 50, 20),"X"))
			{
				Network.Disconnect();
				menuManager.ChangeState (MenuManager.GameState.MainMenu);
			}			
		}
		
		foreach(NetPlayer player in menuManager.playerManager.localPlayers.OrderByDescending(p => p.playerStats.score))
		{
			Rect cameraZone = player.playerCamera.camera.rect;
			if(menuManager.playerManager.localPlayers.Count == 1)
			{
		       	cameraZone = new Rect(0, 0, 1, 1);
			}
			else if(menuManager.playerManager.localPlayers.Count == 2)
			{
				if(player.localPlayerNumber == 0) cameraZone = new Rect(0, 0, 0.5f, 1);
				if(player.localPlayerNumber == 1) cameraZone = new Rect(0.5f, 0, 0.5f, 1);
			}
			else if(menuManager.playerManager.localPlayers.Count == 3)
			{
				if(player.localPlayerNumber == 0) cameraZone = new Rect(0   , 0f   , 0.5f, 0.5f);
				if(player.localPlayerNumber == 1) cameraZone = new Rect(0.5f, 0f   , 0.5f, 0.5f);
				if(player.localPlayerNumber == 2) cameraZone = new Rect(0   , 0.5f     , 1   , 0.5f);
			}
			else if(menuManager.playerManager.localPlayers.Count == 4)
			{
				if(player.localPlayerNumber == 0) cameraZone = new Rect(0,    0f, 0.5f, 0.5f);
				if(player.localPlayerNumber == 1) cameraZone = new Rect(0.5f, 0f, 0.5f, 0.5f);
				
				if(player.localPlayerNumber == 2) cameraZone = new Rect(0,    0.5f, 0.5f, 0.5f);
				if(player.localPlayerNumber == 3) cameraZone = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
			}
			else if(menuManager.playerManager.localPlayers.Count == 5)
			{
				if(player.localPlayerNumber == 0) cameraZone = new Rect(0,    0, 0.5f, 1f/3f);
				if(player.localPlayerNumber == 1) cameraZone = new Rect(0.5f, 0, 0.5f, 1f/3f);
			
				if(player.localPlayerNumber == 2) cameraZone = new Rect(0,    1f/3f, 0.5f, 1f/3f);
				if(player.localPlayerNumber == 3) cameraZone = new Rect(0.5f, 1f/3f, 0.5f, 1f/3f);
			
				if(player.localPlayerNumber == 4) cameraZone = new Rect(0,    2f/3f, 1, 1f/3f);
			}
			//	GUI.Box(displayWindow, player.player.name);
			//	Debug.Log(player.player.name + ": " + cameraZone.y);
			
			cameraZone = new Rect(cameraZone.x * Screen.width, cameraZone.y * Screen.height, cameraZone.width * Screen.width, cameraZone.height * Screen.height);
			
			GUILayout.BeginArea(cameraZone);
						layoutPlayer(player.player.name, player.playerStats.score);
			GUILayout.EndArea();
		}
			
	}
	
	public static void layoutPlayer(string name, int score)//, int place1, string name1, int score1, int place2, string name2, int score2)
	{
	//	GUILayout.Space(Screen.height - 25);
		GUILayout.Box(name + ": " + score, GUILayout.ExpandWidth(false), GUILayout.MinWidth(Screen.width/4));
	}
}
