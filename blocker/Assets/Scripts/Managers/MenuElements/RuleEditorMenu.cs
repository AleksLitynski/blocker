using UnityEngine;
using System.Collections;

/*
 * This class is misnamed. It should be the CONTROLS MENU
 * 
 * Oh well. Sorry for the confusion.
 * 
 * Game rule editing is coming later.
 * 
 * 
 */
public class RuleEditorMenu 
{

	public static void generateGUI(MenuManager menuManager)
	{
		GUILayout.BeginArea(new Rect(Screen.width*1/5,Screen.height*1/5, Screen.width*3/5, Screen.height*3/5));
			//GUILayout.Box ("",GUILayout.MinWidth(Screen.width*3/5), GUILayout.MinHeight(Screen.height*3/5));
			GUILayout.BeginHorizontal("box");
				GUILayout.BeginVertical(GUILayout.MinWidth(Screen.width*1/5));
					GUILayout.Label ("Controls");
					GUILayout.Label ("");
					GUILayout.Label ("Movement");
					GUILayout.Label ("Aim");
					GUILayout.Label ("Jump");
					GUILayout.Label ("Ironsights");
					GUILayout.Label ("Fire");
				GUILayout.EndVertical();
				GUILayout.BeginVertical(GUILayout.MinWidth(Screen.width*1/5));
					GUILayout.Label ("Keyboard");
					GUILayout.Label ("");
					GUILayout.Label ("WASD");
					GUILayout.Label ("Mouse");
					GUILayout.Label ("Space bar");
					GUILayout.Label ("Right Click");
					GUILayout.Label ("Left Click");
					GUILayout.Space(50f);
				GUILayout.EndVertical();
				GUILayout.BeginVertical(GUILayout.MinWidth(Screen.width*1/5));
					GUILayout.Label ("Controller");
					GUILayout.Label ("");
					GUILayout.Label ("Left Analog Stick");
					GUILayout.Label ("Right Analog Stick");
					GUILayout.Label ("A button");
					GUILayout.Label ("Left Bumper");
					GUILayout.Label ("Right Bumper");
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
		if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height - 100,200,20), "Back to Main Menu")){menuManager.ChangeState(MenuManager.GameState.MainMenu);}	
	}
}
