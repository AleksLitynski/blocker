using UnityEngine;
using System.Collections;

/* Static menu method. There were very ugly when they were in a single class.
 * 
 * All called from MenuManager.
 */
public class MainMenu
{

	public static void generateGUI(MenuManager menuManager)
	{
		// menu header
			GUI.Label(new Rect (Screen.width/2 - 125,Screen.height/2 - 150,250,300), "Blocker", "box");
			
			// main menu options
			GUILayout.BeginArea(new Rect(Screen.width/2-100, Screen.height*2/5, 200,400));
			GUILayout.BeginVertical();
			
			if (GUILayout.Button ("Join Game", GUILayout.MaxWidth(200))){menuManager.ChangeState(MenuManager.GameState.JoinGame);}
			if (GUILayout.Button ("Host Game", GUILayout.MaxWidth(200))){menuManager.ChangeState(MenuManager.GameState.HostGame);}
			if (GUILayout.Button ("Instructions", GUILayout.MaxWidth(200))){menuManager.ChangeState(MenuManager.GameState.RuleEditor);}
			/*if (GUILayout.Button ("Map Editor", GUILayout.MaxWidth(200))){menuManager.ChangeState(MenuManager.GameState.MapEditor);}
			if (GUILayout.Button ("Options", GUILayout.MaxWidth(200))){menuManager.ChangeState(MenuManager.GameState.Options);}*/
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
		
	}
}
