using UnityEngine;
using System.Collections;

/* Static menu method. There were very ugly when they were in a single class.
 * 
 * All called from MenuManager.
 */
public class OptionsMenu
{
	//There is nothing in the "options screen", just a back button
	public static void generateGUI(MenuManager menuManager)
	{
		if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height - 100,200,20), "Back to Main Menu")){menuManager.ChangeState(MenuManager.GameState.MainMenu);}	
	}
}
