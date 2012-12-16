using UnityEngine;
using System.Collections;

public class BlockerObject : MonoBehaviour {

    public ConnectionManager connectionManager;
    public PlayerManager     playerManager;
    public NetPlayer netPlayer;
    public InputDispatcher inputDispatcher;
    public InputReceiver inputReceiver;

    void Start()
    {
        connectionManager = GameObject.Find("World").GetComponent("ConnectionManager") as ConnectionManager;
        playerManager     = GameObject.Find("World").GetComponent("PlayerManager") as PlayerManager;
        netPlayer = GameObject.Find("World").GetComponent("NetPlayer") as NetPlayer;
        inputDispatcher = GameObject.Find("World").GetComponent("InputDispatcher") as InputDispatcher;
        inputReceiver = GameObject.Find("World").GetComponent("InputReceiver") as InputReceiver;
        
    }
	
}
