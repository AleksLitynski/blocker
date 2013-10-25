using UnityEngine;
using System.Collections;

/* Bundles up the valid user input.
 * Unity has a hard time with input customization.
 * See "Edit -> Project Setting -> Input" to see what I mean. I had to enter all those manually.
 */
public class InputCollection {
	
	//valid player inputs
    public float forward;
    public float straff;
    public float turnRight;
    public float turnUp;
    public bool jump;
    public bool fireOne;
    public bool fireTwo;
    public bool sprint;
    public bool crouch;
    public NetPlayer netPlayer;

	
	//Creates a collection based on the player
    public InputCollection(){}
	public InputCollection(NetPlayer player)
	{
        netPlayer = player;
	}
	
	//Creates a collection based on player and all options
    public InputCollection(NetPlayer player, float f, float s, float tR, float tU, bool j, bool f1, bool f2, bool sp, bool c)
    {
        forward = f;
        straff = s;
        turnRight = tR;
        turnUp = tU;
        jump = j;
        fireOne = f1;
        fireTwo = f2;
        sprint = sp;
        crouch = c;
        netPlayer = player;
    }
	
	//Sends the inputs to the server to be processed
    public void sendToServerVia(NetworkView view, InputReceiver receiver)
    {
        if(Network.peerType == NetworkPeerType.Client) 
        {
            view.RPC("AddInput", RPCMode.Server, forward, straff, turnRight, turnUp, jump, fireOne, fireTwo, sprint, crouch, netPlayer.localPlayerNumber);
        }
        if (Network.peerType == NetworkPeerType.Server)
        {
            receiver.AddInput(forward, straff, turnRight, turnUp, jump, fireOne, fireTwo, sprint, crouch, netPlayer.localPlayerNumber);
        }
    }
}
