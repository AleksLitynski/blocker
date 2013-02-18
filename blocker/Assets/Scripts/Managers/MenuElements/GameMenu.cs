using UnityEngine;
using System.Collections;

public class GameMenu
{

	public static void generateGUI(MenuManager menuManager)
	{
		// manage a menu for each local player (placement and settings)
			// local players FUUUUUUUUUUUCKKKKKKKKK (fuck local players)
			foreach(NetPlayer player in menuManager.playerManager.localPlayers)
			{
				Rect cameraZone = player.playerCamera.camera.rect;
				Rect displayWindow = new Rect(  (Screen.width * cameraZone.x),
												Screen.height * cameraZone.height - (Screen.height * cameraZone.y),
												Screen.width * cameraZone.width,
												Screen.height * cameraZone.height);
			//GUI.Box(displayWindow, player.player.name);
			//Debug.Log(player.player.name + ": " + displayWindow);
			
				GUILayout.BeginArea(displayWindow);
					GUILayout.Label("score: " + player.playerStats.score);
					GUILayout.Label("Name: " + player.player.name);
				GUILayout.EndArea();
			}
			
	}
}
