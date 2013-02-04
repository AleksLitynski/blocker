using UnityEngine;
using System.Collections;

public class BlockerObject : MonoBehaviour {
	
	[HideInInspector]
    public ConnectionManager connectionManager;
	[HideInInspector]
    public PlayerManager playerManager;
	[HideInInspector]
    public NetPlayer netPlayer;
	[HideInInspector]
    public InputDispatcher inputDispatcher;
	[HideInInspector]
    public InputReceiver inputReceiver;
	[HideInInspector]
    public RaceManager raceManager;
	[HideInInspector]
    public GameObject world;
	[HideInInspector]
    public MapManager mapManager;
	[HideInInspector]
    public MenuManager menuManager;

  	public virtual void Start()
    {
        connectionManager = GameObject.Find("World").GetComponent("ConnectionManager") as ConnectionManager;
        playerManager     = GameObject.Find("World").GetComponent("PlayerManager") as PlayerManager;
        netPlayer = GameObject.Find("World").GetComponent("NetPlayer") as NetPlayer;
        inputDispatcher = GameObject.Find("World").GetComponent("InputDispatcher") as InputDispatcher;
        inputReceiver = GameObject.Find("World").GetComponent("InputReceiver") as InputReceiver;
		raceManager = GameObject.Find("World").GetComponent<RaceManager>() as RaceManager;
		mapManager = GameObject.Find("World").GetComponent<MapManager>() as MapManager;
		menuManager = GameObject.Find("World").GetComponent<MenuManager>() as MenuManager;
		world = GameObject.Find("World");
		
    }
	
	/*
	public void PlayerEnter(string he)
	{
		
	}
	public void PlayerExit(string he)
	{
		
	}*/
	
}
