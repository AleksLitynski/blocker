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
	//[HideInInspector]
    //public GameManager gameManager;
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
		//gameManager = GameObject.Find("World").GetComponent<GameManager>() as GameManager;
		mapManager = GameObject.Find("World").GetComponent<MapManager>() as MapManager;
		menuManager = GameObject.Find("World").GetComponent<MenuManager>() as MenuManager;
		world = GameObject.Find("World");
	}
	
}
