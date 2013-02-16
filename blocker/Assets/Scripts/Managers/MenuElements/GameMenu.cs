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
				var cameraZone = player.playerCamera.camera.rect;
				cameraZone.x = Screen.width * cameraZone.x;
				cameraZone.y = Screen.height * cameraZone.y;
				cameraZone.width = Screen.width * cameraZone.width;
				cameraZone.height = Screen.height * cameraZone.height;
				
				GUILayout.BeginArea(cameraZone);
					GUILayout.Label("score: " + player.playerStats.score);
				GUILayout.EndArea();
			}
			
	}
}
