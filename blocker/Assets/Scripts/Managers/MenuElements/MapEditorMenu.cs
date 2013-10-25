using UnityEngine;
using System.Collections;

/* Menu item. Static function used to draw the map menu.
 * 
 * Called from MenuManager.
 * 
 */
public class MapEditorMenu 
{

	public static void generateGUI(MenuManager menuManager)
	{
		if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height - 100,200,20), "Back to Main Menu")){menuManager.ChangeState(MenuManager.GameState.MainMenu);}	
	}
}
