using UnityEngine;
using System.Collections;

/*
 * This used to be much larger. Now calls out to MenuElements folder items. 
 * 
 * A big state machine of all the menus in the game
 * 
 */
public class MenuManager : BlockerObject 
{
	// enums
	public enum GameState {MainMenu, Options, RuleEditor, MapEditor, HostGame, JoinGame, Lobby, Game, PostGame};
	public GameState gameState;
	public GameState prevState;
	
	// vars
	public string gameName = "Blocker v 0.1";
	public string hostedGameName = "Default Game Name";
	public string hostedGameDescription = "Default Description";
	public int GameCode = 1;
	public int JoinGameCode = 2;
	public const int LobbyCode = 3;
	
	public GUISkin guiSkin; //Mostly used to specify the Pixaled Text Texture.
	
	// accessibility
	Camera myCamera;
	//PlayerManager playerManager;
	//MapManager mapManager;
	
	// funsies
	public GameObject bgMap;
	Object[] maps;
	//bool shouldRotateMap;
	public Vector3 cameraPosition;
	public Vector3 lookAtPosition;
	Vector3 mapRotation = Vector3.zero; //the rotation of the map. Used to sync when maps are swapped so it looks less awkward.
	
	// Use this for initialization
	public override void Start () 
	{
		base.Start();
		gameState = GameState.MainMenu;
		myCamera = this.gameObject.GetComponent<Camera>();
		maps = Resources.LoadAll("Maps",typeof(GameObject));
		
		LoadRandomMap();
		clearBullets();
		toggleVision(false);
		
	}
	
	void Update()
	{
		if (gameState != GameState.Game)
		{
		/*	myCamera.transform.position = cameraPosition;
			myCamera.transform.LookAt(lookAtPosition);
			if(bgMap)
			{
				mapRotation = Quaternion.Euler(mapRotation.eulerAngles.x, mapRotation.eulerAngles.y + 0.05f, mapRotation.eulerAngles.z);
				bgMap.transform.rotation = mapRotation;
			}*/
			mapRotation += new Vector3(0,.05f,0);
			//bgMap.transform.rotation = Quaternion.Euler (mapRotation);
			myCamera.transform.RotateAround(bgMap.transform.position, Vector3.up, 5 * Time.deltaTime);
		}
	}
	
	// Update is called once per frame
	void OnGUI () 
	{
		
		GUI.skin = guiSkin;
		// do different updates based on the state of the game.
		switch(gameState)
		{
		case GameState.MainMenu:
			MainMenu.generateGUI(this);
				break;
		case GameState.Options:
			// return to main menu button
			OptionsMenu.generateGUI(this);
			break;
		case GameState.RuleEditor:
			RuleEditorMenu.generateGUI(this);
			break;
		case GameState.MapEditor:
			MapEditorMenu.generateGUI(this);
			break;
		case GameState.HostGame:
			HostGameMenu.generateGUI(this);
			break;
		case GameState.JoinGame:
			// get games from the master server.
			JoinGameMenu.generateGUI(this);
			break;
		case GameState.Lobby:
			LobbyMenu.generateGUI(this);
			break;
		case GameState.Game:
			GameMenu.generateGUI(this);
			break;
		case GameState.PostGame:
			// depreciated
			break;
		}
	}
	
	//Loads a random map to be used as a background image.
	void LoadRandomMap()
	{
		Destroy(bgMap);
		bgMap = maps[Random.Range(0, maps.Length)] as GameObject;
		bgMap = Instantiate(bgMap,new Vector3(),Quaternion.identity) as GameObject;
		
		float minx = 9999;
		float maxx = -9999;
		float miny = 9999;
		float maxy = -9999;
		float minz = 9999;
		float maxz = -9999;
		foreach(Transform child in bgMap.transform)
		{
			if (child.renderer != null)
			{
				if (child.renderer.bounds.min.x < minx) {minx = child.renderer.bounds.min.x;}
				if (child.renderer.bounds.max.x > maxx) {maxx = child.renderer.bounds.max.x;}
				if (child.renderer.bounds.min.y < miny) {miny = child.renderer.bounds.min.y;}
				if (child.renderer.bounds.max.y > maxy) {maxy = child.renderer.bounds.max.y;}
				if (child.renderer.bounds.min.z < minz) {minz = child.renderer.bounds.min.z;}
				if (child.renderer.bounds.max.z > maxz) {maxz = child.renderer.bounds.max.z;}
			}
		}
		cameraPosition = new Vector3(1.5f*(maxx-minx), (maxy-miny), maxz-minz);
		lookAtPosition = bgMap.transform.position;
			
		myCamera.transform.position = cameraPosition;
		myCamera.transform.LookAt(lookAtPosition);
	}
	
	//Odd place for it, but it deletes the lingering bullet objects.
	void clearBullets()
	{
		var bullets = world.transform.FindChild("Bullets");
		Destroy(bullets.gameObject);
		var newBullets = new GameObject();
		newBullets.transform.parent = world.transform;
		newBullets.name = "Bullets";
	}
	
	// toggle visibility of players (IE: hide them when we are in the menu)
	void toggleVision(bool showPlayers)
	{
		if (showPlayers)
		{
			// initialize the state for gameplay.
			playerManager.RevealPlayers();
			if (playerManager.localPlayers.Count != 0)
			{
				Screen.lockCursor = true;
				playerManager.setToLocalCameras();
				foreach(NetPlayer player in playerManager.players)
				{
					player.playerStats.score = 0;	//reset the score when we hide the players
				}
			}
			//shouldRotateMap = false;
		}
		else
		{
			
			// initialize the state for non gameplay-stuff.
			Screen.lockCursor = false;
			playerManager.setToWorldCamera();
			playerManager.HidePlayers();
			
			//shouldRotateMap = true;
		}
		//bgtoggle = tf;
	}
	
	//RPC's to all clients the state of the game
	[RPC]
	public void ChangeState(int stateCode)
	{
		GameState gs = new GameState();
		
		// all stateCodes are const ints established at the top of the file.
		switch(stateCode)
		{
		case 1: 
			gs = GameState.Game;
			break;
		case 2:
			gs = GameState.JoinGame;
			break;
		case 3:
			gs = GameState.Lobby;
			break;
		}
		ChangeState (gs);
	}
	
	// ALWAYS USE THIS TO MAKE CHANGES TO GAMESTATE.
	// if you dont use this, you may get the background map in your actual map (AHHH)
	[RPC]
	public void ChangeState(GameState gs)
	{
		prevState = gameState;
		gameState = gs;
		
		clearBullets();
		if(gameState == GameState.Game)
		{
			Debug.Log("1");
			toggleVision(true);
			if (prevState == GameState.Lobby)
			{
				Debug.Log("2");
				if (playerManager.localPlayers.Count == 0)
				{
					Debug.Log("3");
					myCamera.transform.position = cameraPosition;
					myCamera.transform.LookAt(lookAtPosition);
				}
			}
		}
		if(gameState == GameState.JoinGame   || 
			gameState == GameState.HostGame  || 
			gameState == GameState.MapEditor || 
			gameState == GameState.Options   || 
			gameState == GameState.RuleEditor)
		{
			toggleVision(false);		//Hide the map when in menu	
		}
		if(gameState == GameState.MainMenu )
		{
			LoadRandomMap();
		}
		if(gameState == GameState.Lobby)
		{
			toggleVision(false);
			//LoadRandomMap(); 
			if (prevState == GameState.Game)
			{
				myCamera.transform.position = cameraPosition;
				myCamera.transform.LookAt(lookAtPosition);
			}
		}
		
		
		// if you were previously in a state where the bgmap should not appear and are
		// now in a state that the bgmap should appear, turn it on.
		/*if ((prevState == GameState.Game || prevState == GameState.PostGame) 
			&& (gameState != GameState.Game || gameState != GameState.PostGame))
		{
			ToggleBGMap(true);
		}
		// otherwise, turn it off.
		else if ((prevState != GameState.Game || prevState != GameState.PostGame) 
				&& (gameState == GameState.Game || gameState == GameState.PostGame))
		{
			ToggleBGMap(false);	
		}
		if(gs == GameState.Lobby)
		{
			playerManager.setToWorldCamera();
			playerManager.HidePlayers();
		}*/
	}
}
