using UnityEngine;
using System.Collections;

public class InputReceiver : BlockerObject 
{
	
	
	[RPC]
    public void AddInput(float f, float s, float tR, float tU, bool j, bool f1, bool f2, bool sp, bool c, int localNumber, NetworkMessageInfo incomingInfo)
    {
        NetworkMessageInfoLocalWrapper info = new NetworkMessageInfoLocalWrapper(incomingInfo);
		
		//build the inputCollection backup on the recieving end
        InputCollection sentCollection = new InputCollection();
        foreach (NetPlayer player in playerManager.players)
        {
            if (player.networkPlayer == info.sender && player.localPlayerNumber == localNumber)
            {
                sentCollection = new InputCollection(player, f, s, tR, tU, j, f1, f2, sp, c);
                break;
            }
        }
		
		sentCollection.move();
		
		networkView.RPC("setPlayerPos", RPCMode.Others, sentCollection.netPlayer.transform.position, sentCollection.netPlayer.name);
		networkView.RPC("setPlayerRot", RPCMode.Others, sentCollection.netPlayer.transform.rotation.eulerAngles, sentCollection.netPlayer.name);	
    }
	public void AddInput(float f, float s, float tR, float tU, bool j, bool f1, bool f2, bool sp, bool c, int localNumber)
    {
		AddInput(f, s, tR, tU, j, f1, f2, sp, c, localNumber, new NetworkMessageInfo());	
    }
	
	[RPC]
	void setPlayerPos(Vector3 newPos, string playerName)
	{
		foreach (NetPlayer player in playerManager.players)
        {
            if (player.name == playerName)
            {
                player.rigidbody.position = newPos;
                break;
            }
        }
	}
	[RPC]
	void setPlayerRot(Vector3 newRot, string playerName)
	{
		foreach (NetPlayer player in playerManager.players)
        {
            if (player.name == playerName)
            {
                player.rigidbody.rotation = Quaternion.Euler(newRot);
                break;
            }
        }
	}
    
}
