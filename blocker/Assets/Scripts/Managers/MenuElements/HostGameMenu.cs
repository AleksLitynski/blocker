using UnityEngine;
using System.Collections;

public class HostGameMenu
{
	public static void generateGUI(MenuManager menuManager)
	{
		
		GUILayout.BeginArea(new Rect(Screen.width/2-100, Screen.height/2, 200,400));
				GUILayout.BeginVertical();
				
				// get name and description from text fields.
				menuManager.hostedGameName = GUILayout.TextField(menuManager.hostedGameName, GUILayout.MaxWidth (200));
				menuManager.hostedGameDescription = GUILayout.TextField(menuManager.hostedGameDescription, GUILayout.MaxWidth(200));
				
				// initialize the server and register it with unity's master server.
				if(GUILayout.Button ("Host", GUILayout.MinWidth(50)))
				{
					Network.InitializeServer(32, Random.Range(2000,40000), !Network.HavePublicAddress());
					MasterServer.RegisterHost(menuManager.gameName, menuManager.hostedGameName, menuManager.hostedGameDescription);
					
					menuManager.ChangeState(MenuManager.GameState.Lobby);
				}
				
				if (GUILayout.Button("Back to Main Menu", GUILayout.MinWidth(50)))
				{
					menuManager.ChangeState(MenuManager.GameState.MainMenu);
				}
				
				GUILayout.EndVertical();
				GUILayout.EndArea();
	}
}
