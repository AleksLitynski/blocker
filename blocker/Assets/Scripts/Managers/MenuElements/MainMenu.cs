using UnityEngine;
using System.Collections;

public class MainMenu
{

	public static void generateGUI(MenuManager menuManager)
	{
		// menu header
			GUI.Label(new Rect (Screen.width/2 - 100,Screen.height/2 - 150,200,300), "Blocker", "box");
			
			// main menu options
			GUILayout.BeginArea(new Rect(Screen.width/2-100, Screen.height*2/5, 200,400));
			GUILayout.BeginVertical();
			
			if (GUILayout.Button ("Join Game", GUILayout.MaxWidth(200))){menuManager.ChangeState(MenuManager.GameState.JoinGame);}
			if (GUILayout.Button ("Host Game", GUILayout.MaxWidth(200))){menuManager.ChangeState(MenuManager.GameState.HostGame);}
			if (GUILayout.Button ("Rule Editor", GUILayout.MaxWidth(200))){menuManager.ChangeState(MenuManager.GameState.RuleEditor);}
			if (GUILayout.Button ("Map Editor", GUILayout.MaxWidth(200))){menuManager.ChangeState(MenuManager.GameState.MapEditor);}
			if (GUILayout.Button ("Options", GUILayout.MaxWidth(200))){menuManager.ChangeState(MenuManager.GameState.Options);}
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
		
	}
}
