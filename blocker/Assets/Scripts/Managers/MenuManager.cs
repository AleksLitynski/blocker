using UnityEngine;
using System.Collections;

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
	
	// accessibility
	Camera myCamera;
	//PlayerManager playerManager;
	//MapManager mapManager;
	
	// funsies
	GameObject bgMap;
	Object[] maps;
	bool bgtoggle;
	Vector3 cameraPosition;
	Vector3 lookAtPosition;
	
	// Use this for initialization
	public override void Start () 
	{
		base.Start();
		gameState = GameState.MainMenu;
		
		myCamera = this.gameObject.GetComponent<Camera>();
		//playerManager = this.gameObject.GetComponent<PlayerManager>();
		//mapManager = this.gameObject.GetComponent<MapManager>();
		maps = Resources.LoadAll("Maps",typeof(GameObject));
		
		ToggleBGMap(true);
	}
	
	void Update()
	{
		if (bgtoggle)
		{
			myCamera.transform.position = cameraPosition;
			myCamera.transform.LookAt(lookAtPosition);
			bgMap.transform.Rotate(new Vector3(0, 0.05f, 0));
		}
	}
	
	// Update is called once per frame
	void OnGUI () 
	{
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
<<<<<<< HEAD
			GUILayout.BeginArea(new Rect(0,0,Screen.width, Screen.height));
				GUILayout.BeginHorizontal();
					// provide add/drop ability
					GUILayout.BeginArea(new Rect(Screen.width*1/20,Screen.height*1/20,Screen.width*2/20, Screen.height*15/20));
						GUILayout.BeginVertical();
						if (GUILayout.Button("New Player", GUILayout.MinWidth(Screen.width*2/20)))
				        {
				            if (Network.peerType == NetworkPeerType.Client) networkView.RPC("AddNewPlayerRequest", RPCMode.Server);
				            if (Network.peerType == NetworkPeerType.Server) playerManager.AddNewPlayerRequest(new NetworkMessageInfo());
				        }
						foreach(NetPlayer player in playerManager.players)
						{
							if(player.networkPlayer.ToString() == Network.player.ToString())
							{
								if (GUILayout.Button("Drop", GUILayout.MinWidth(Screen.width*2/20)))
					            {
					                networkView.RPC("RemovePlayerRequest", RPCMode.Server, player.localPlayerNumber);
					                if (Network.peerType == NetworkPeerType.Server) playerManager.RemovePlayerRequest(player.localPlayerNumber, new NetworkMessageInfo());
					            }
							}
							else
							{
								GUILayout.Label("");
							}
						}
						GUILayout.EndVertical();
					GUILayout.EndArea();
					// list player names
					GUILayout.BeginArea(new Rect(Screen.width*4/20,Screen.height*1.9f/20,Screen.width*5/20, Screen.height*14/20));
						foreach(NetPlayer player in playerManager.players)
						{
							GUILayout.Label(player.gameObject.name);
						}
					GUILayout.EndArea();
					// list player scores
					GUILayout.BeginArea(new Rect(Screen.width*10/20,Screen.height*1.9f/20,Screen.width*5/20, Screen.height*14/20));
						foreach(NetPlayer player in playerManager.players)
						{
							GUILayout.Label("" + player.playerStats.score);
						}
					GUILayout.EndArea();
				GUILayout.EndHorizontal();
				GUILayout.BeginArea(new Rect(Screen.width*(1.0f/6), Screen.height*17/20, Screen.width*2/3, Screen.height*2/20));
					GUILayout.BeginHorizontal();
						// actually initialize the game state.
						if (Network.peerType == NetworkPeerType.Server)
						{
							if (GUILayout.Button("Start", GUILayout.MaxWidth(200)))
							{							
								// buffer an RPC telling everyone the game has started (join in progress)
								networkView.RPC("ChangeState", RPCMode.AllBuffered, GameCode);
								networkView.RPC("initializeGame", RPCMode.All);
							}
						}
						else
						{
							if (GUILayout.Button("Vote to Start", GUILayout.MaxWidth(200)))
							{
							}
						}
						GUILayout.Label("", GUILayout.MaxWidth(Screen.width*2/3-400));
						if (GUILayout.Button("Back to Main Menu", GUILayout.MaxWidth(200)))
						{
							if(Network.peerType == NetworkPeerType.Server)
							{
								// return yourself to the main menu and everyone else to the joingame menu.
								ChangeState (GameState.MainMenu);
								networkView.RPC("ChangeState", RPCMode.Others, JoinGameCode);
								MasterServer.UnregisterHost();
							}
							else
							{
								Network.Disconnect();	
							}
						}
					GUILayout.EndHorizontal();
				GUILayout.EndArea();
			GUILayout.EndArea();
=======
			LobbyMenu.generateGUI(this);
>>>>>>> Broke menu manager into  a few small classes.
			break;
		case GameState.Game:
			GameMenu.generateGUI(this);
			break;
		case GameState.PostGame:
			// depreciated
			break;
		}
	}
	
	void playerAddRemove()
	{
		if (GUILayout.Button("New Player"))
        {
            if (Network.peerType == NetworkPeerType.Client) networkView.RPC("AddNewPlayerRequest", RPCMode.Server);
            if (Network.peerType == NetworkPeerType.Server) playerManager.AddNewPlayerRequest(new NetworkMessageInfo());
        }
        for (int i = 0; i < playerManager.localPlayers.Count; i++)
        {
			GUILayout.BeginHorizontal();
	            if (GUILayout.Button("Drop Player " + playerManager.localPlayers[i].localPlayerNumber))
	            {
	                networkView.RPC("RemovePlayerRequest", RPCMode.Server, playerManager.localPlayers[i].localPlayerNumber);
	                if (Network.peerType == NetworkPeerType.Server) playerManager.RemovePlayerRequest(playerManager.localPlayers[i].localPlayerNumber, new NetworkMessageInfo());
	            }
				if(GUILayout.Button("KB " + playerManager.localPlayers[i].localPlayerNumber))
				{
					foreach(NetPlayer player in playerManager.localPlayers)
					{
						player.KeyboardPlayer = false;
					}
					playerManager.localPlayers[i].KeyboardPlayer = true;
				}
			GUILayout.EndHorizontal();
        }
	}
	
	void ToggleBGMap(bool tf)
	{
		if (tf)
		{
			Screen.lockCursor = false;
			Destroy(mapManager.loadedMap);
			playerManager.setToWorldCamera();
			playerManager.HidePlayers();
			//raceManager.init();
			var bullets = world.transform.FindChild("Bullets");
			Destroy(bullets.gameObject);
			var newBullets = new GameObject();
			newBullets.transform.parent = world.transform;
			newBullets.name = "Bullets";
			
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
			lookAtPosition = new Vector3(0,.5f*(maxy-miny),0);
		}
		else
		{
			Screen.lockCursor = true;
			playerManager.RevealPlayers();
			playerManager.setToLocalCameras();
			foreach(NetPlayer player in playerManager.players)
			{
				player.playerStats.score = 0;	
			}
			Destroy(bgMap);
		}
		bgtoggle = tf;
	}
	
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
		
		// if you were previously in a state where the bgmap should not appear and are
		// now in a state that the bgmap should appear, turn it on.
		if ((prevState == GameState.Game || prevState == GameState.PostGame) 
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
	}
}
