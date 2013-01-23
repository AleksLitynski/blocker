using UnityEngine;
using System.Collections;

public class InputReceiver : BlockerObject 
{
	
	[RPC]
    public void AddInput(float f, float s, float tR, float tU, bool j, bool f1, bool f2, bool sp, bool c, int localNumber, NetworkMessageInfo incomingInfo)
    {
        NetworkMessageInfoLocalWrapper info = new NetworkMessageInfoLocalWrapper(incomingInfo);
		
        foreach (NetPlayer player in playerManager.players)
        {
            if (player.networkPlayer == info.sender && player.localPlayerNumber == localNumber)
            {
                player.move(new InputCollection(player, f, s, tR, tU, j, f1, f2, sp, c));
				
				networkView.RPC("setPlayerTransform", RPCMode.Others, player.transform.position, player.transform.rotation.eulerAngles, player.playerArms.rotation.eulerAngles.x, player.name);
                break;
            }
        }
    }
	
	public void AddInput(float f, float s, float tR, float tU, bool j, bool f1, bool f2, bool sp, bool c, int localNumber)
    {
		AddInput(f, s, tR, tU, j, f1, f2, sp, c, localNumber, new NetworkMessageInfo());	
    }
	
	[RPC]
	public void setPlayerTransform(Vector3 pos, Vector3 rot, float armRot, string playerName)
	{
		foreach (NetPlayer player in playerManager.players)
        {
            if (player.name == playerName)
            {
				player.setTransform(pos, rot);
				player.playerArms.rotation = Quaternion.Euler(armRot, player.playerArms.rotation.eulerAngles.y, player.playerArms.rotation.eulerAngles.z);
                break;
            }
        }
	}
	[RPC]
	public void setTransform(Vector3 pos, Vector3 rot, string objName)
	{
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("gameObject"))
        {
            if (obj.name == objName)
            {
				obj.GetComponent<NetPlayer>().netObject.setTransform(pos, rot);
                break;
            }
        }
	}
}
