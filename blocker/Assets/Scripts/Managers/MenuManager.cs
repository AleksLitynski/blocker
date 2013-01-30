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
	void Update () 
	{
		// do different updates based on the state of the game.
		switch(gameState)
		{
		case GameState.MainMenu:
				break;
		}
	}
}
