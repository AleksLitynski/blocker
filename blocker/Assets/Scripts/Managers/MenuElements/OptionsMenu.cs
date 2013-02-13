using UnityEngine;
using System.Collections;

public class OptionsMenu
{

	public static void generateGUI(MenuManager menuManager)
	{
		if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height - 100,160,20), "Back to Main Menu")){menuManager.ChangeState(MenuManager.GameState.MainMenu);}	
	}
}
