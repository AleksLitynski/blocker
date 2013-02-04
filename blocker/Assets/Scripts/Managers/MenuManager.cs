using UnityEngine;
using System.Collections;

public class MenuManager : BlockerObject 
{
	// enums
	public enum GameState {MainMenu, Options, RuleEditor, MapEditor, HostGame, JoinGame, Lobby, Game, PostGame};
	public GameState gameState;
	public GameState prevState;
	
	// vars
	string gameName = "Blocker v 0.1";
	string hostedGameName = "Default Game Name";
	string hostedGameDescription = "Default Description";
	const int GameCode = 1;
	const int JoinGameCode = 2;
	
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
			// menu header
			GUI.Label(new Rect (Screen.width/2 - 100,Screen.height/2 - 150,200,300), "Blocker", "box");
			
			// main menu options
			GUILayout.BeginArea(new Rect(Screen.width/2-100, Screen.height*2/5, 200,400));
			GUILayout.BeginVertical();
			
			if (GUILayout.Button ("Join Game", GUILayout.MaxWidth(200))){ChangeState(GameState.JoinGame);}
			if (GUILayout.Button ("Host Game", GUILayout.MaxWidth(200))){ChangeState(GameState.HostGame);}
			if (GUILayout.Button ("Rule Editor", GUILayout.MaxWidth(200))){ChangeState(GameState.RuleEditor);}
			if (GUILayout.Button ("Map Editor", GUILayout.MaxWidth(200))){ChangeState(GameState.MapEditor);}
			if (GUILayout.Button ("Options", GUILayout.MaxWidth(200))){ChangeState(GameState.Options);}
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
			break;
		case GameState.Options:
			
			// return to main menu button
			if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height - 100,160,20), "Back to Main Menu")){ChangeState(GameState.MainMenu);}
			break;
		case GameState.RuleEditor:
			if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height - 100,160,20), "Back to Main Menu")){ChangeState(GameState.MainMenu);}
			break;
		case GameState.MapEditor:
			if (GUI.Button (new Rect (Screen.width/2 - 80,Screen.height - 100,160,20), "Back to Main Menu")){ChangeState(GameState.MainMenu);}
			break;
		case GameState.HostGame:
			GUILayout.BeginArea(new Rect(Screen.width/2-100, Screen.height/2, 200,400));
			GUILayout.BeginVertical();
			
			// get name and description from text fields.
			hostedGameName = GUILayout.TextField(hostedGameName, GUILayout.MaxWidth (200));
			hostedGameDescription = GUILayout.TextField(hostedGameDescription, GUILayout.MaxWidth(200));
			
			// initialize the server and register it with unity's master server.
			if(GUILayout.Button ("Host", GUILayout.MinWidth(50)))
			{
				Network.InitializeServer(32, Random.Range(2000,40000), !Network.HavePublicAddress());
				MasterServer.RegisterHost(gameName, hostedGameName, hostedGameDescription);
				
				ChangeState(GameState.Lobby);
			}
			
			if (GUILayout.Button("Back to Main Menu", GUILayout.MinWidth(50)))
			{
				ChangeState(GameState.MainMenu);
			}
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
			break;
		case GameState.JoinGame:
			// get games from the master server.
			MasterServer.RequestHostList(gameName);
			
			GUILayout.BeginArea(new Rect(100, Screen.height/8, Screen.width -200, Screen.height/2));
				GUILayout.BeginVertical();
				
					// actually get games from the master server.
					HostData[] availableHosts = MasterServer.PollHostList();
					
					// actually list games from the master server.
					GUILayout.Box("Available Servers", GUILayout.MinWidth(Screen.width - 200));
						foreach (HostData host in availableHosts)
						{
							GUILayout.BeginHorizontal();
								GUILayout.Label(host.gameName + " (" + host.connectedPlayers + " / " + host.playerLimit + ") - " + host.comment);
								if (GUILayout.Button("Connect", GUILayout.MaxWidth(Screen.width/10)))
								{
									Network.Connect(host);
									ChangeState(GameState.Lobby);
								}	
							GUILayout.EndHorizontal();
						}
				GUILayout.EndVertical();
				
				// buttons at the bottom (refresh and back)
				GUILayout.BeginHorizontal();
					if (GUILayout.Button("Refresh List",GUILayout.MaxWidth(200))){MasterServer.RequestHostList(gameName);}
					GUILayout.Label("",GUILayout.MaxWidth(Screen.width-600));
					if (GUILayout.Button("Back to Main Menu",GUILayout.MaxWidth(200))){ChangeState(GameState.MainMenu);}
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
			break;
		case GameState.Lobby:
			GUILayout.BeginArea(new Rect(Screen.width*1/6, Screen.height*1/6, Screen.width*2/3, Screen.height*2/3));
				GUILayout.BeginVertical();	
					// list all players in the lobby.
					foreach(NetPlayer player in playerManager.players)
					{
						GUILayout.Label(player.gameObject.name);
					}
				GUILayout.EndVertical();
				GUILayout.BeginHorizontal();
					if (Network.peerType == NetworkPeerType.Server)
					{
						// actually initialize the game state.
						if (GUILayout.Button("Start", GUILayout.MaxWidth(200)))
						{
							// buffer an RPC telling everyone the game has started (join in progress)
							networkView.RPC("ChangeState", RPCMode.AllBuffered, GameCode);
							networkView.RPC("initializeGame", RPCMode.All);
						}
						GUILayout.Label("", GUILayout.MaxWidth(Screen.width*2/3-400));
						if (GUILayout.Button("Back to Main Menu", GUILayout.MaxWidth(200)))
						{
							// return yourself to the main menu and everyone else to the joingame menu.
							ChangeState (GameState.MainMenu);
							networkView.RPC("ChangeState", RPCMode.Others, JoinGameCode);
						}
					}
				GUILayout.EndHorizontal();
			
				
			GUILayout.EndArea();
				playerAddRemove();
			break;
		case GameState.Game:
			// manage a menu for each local player (placement and settings)
			// local players FUUUUUUUUUUUCKKKKKKKKK (fuck local players)
			break;
		case GameState.PostGame:
			
			break;
		}
	}
	
	void playerAddRemove()
	{
		
		/* this isthe score thing
		 * 
		 * 
		 * for (int i = 0; i < playerManager.players.Count; i++)
        {
			GUILayout.BeginHorizontal();
            	GUILayout.Label(playerManager.players[i].gameObject.name);
				GUILayout.Label(""+playerManager.players[i].playerStats.score);
			GUILayout.EndHorizontal();
        }*/
		
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
			Destroy(bgMap);
		}
		bgtoggle = tf;
	}
	
	[RPC]
	void ChangeState(int stateCode)
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
		}
		ChangeState (gs);
	}
	
	// ALWAYS USE THIS TO MAKE CHANGES TO GAMESTATE.
	// if you dont use this, you may get the background map in your actual map (AHHH)
	[RPC]
	void ChangeState(GameState gs)
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
