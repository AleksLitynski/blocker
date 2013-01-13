using UnityEngine;
using System.Collections;

public class InputCollection {

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


    public InputCollection(){}
	public InputCollection(NetPlayer player)
	{
        netPlayer = player;
	}
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
	
	public void move()
	{
		netPlayer.move(this);
	}
}
