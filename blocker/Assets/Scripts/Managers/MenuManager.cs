using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour 
{
	// enums
	public enum GameState {MainMenu, Options, RuleEditor, MapEditor, HostGame, JoinGame, Lobby, Game, PostGame};
	public GameState gameState;
	
	// Use this for initialization
	void Start () 
	{
		gameState = GameState.MainMenu;
	}
	
	// Update is called once per frame
	void OnGUI () 
	{
		// do different updates based on the state of the game.
		switch(gameState)
		{
		case GameState.MainMenu:
			// menu header
			GUI.Label(new Rect (Screen.width/2 - 100,Screen.height/2 - 150,200,300), "Blocker", "box");
			
			// main menu options
			if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height/2 - 80,160,20), "Host Game")){gameState = GameState.HostGame;}
			if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height/2 - 55,160,20), "Join Game")){gameState = GameState.JoinGame;}
			if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height/2 - 30,160,20), "Rule Editor")){gameState = GameState.RuleEditor;}
			if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height/2 - 5,160,20), "Map Editor")){gameState = GameState.MapEditor;}
			if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height/2 + 20,160,20), "Options")){gameState = GameState.Options;}
			break;
		case GameState.Options:
			
			// return to main menu button
			if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height - 100,160,20), "Back to Main Menu")){gameState = GameState.MainMenu;}
			break;
		case GameState.RuleEditor:
			
			break;
		case GameState.MapEditor:
			
			break;
		case GameState.HostGame:
			// move connectionmanager crap here
			break;
		case GameState.JoinGame:
			// move connectionmanager crap here
			break;
		case GameState.Lobby:
			
			break;
		case GameState.Game:
			// manage a menu for each local player (placement and settings)
			break;
		case GameState.PostGame:
			
			break;
		}
	}
}
