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
				
				networkView.RPC("setPlayerTransform", RPCMode.Others, player.transform.position, player.transform.rotation.eulerAngles, player.playerArms.rotation, player.name);
				
				/*
				if(f1)
				{
					string name = "rolliepolieolie" + Random.Range(0,1000000);
					networkView.RPC("spawnObject", RPCMode.All, player.transform.position, player.transform.rotation.eulerAngles, name, "rolliepolieolie", "world/map");
					networkView.RPC ("setBulletVelocity", RPCMode.All, player.rigidbody.velocity * 1000, "world/map/"+name);
				}*/
				
                break;
            }
        }
    }
	
	public void AddInput(float f, float s, float tR, float tU, bool j, bool f1, bool f2, bool sp, bool c, int localNumber)
    {
		AddInput(f, s, tR, tU, j, f1, f2, sp, c, localNumber, new NetworkMessageInfo());	
    }
	
	[RPC]
	public void setPlayerTransform(Vector3 pos, Vector3 rot, Quaternion armRot, string playerName)
	{
		foreach (NetPlayer player in playerManager.players)
        {
            if (player.name == playerName)
            {
				player.setTransform(pos, rot);
				player.playerArms.rotation = armRot;
                break;
            }
        }
	}
	[RPC]
	public void setTransform(Vector3 pos, Vector3 rot, string objName)
	{
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("netObject"))
        {
            if (obj.name == objName)
            {
				obj.GetComponent<NetObject>().setTransform(pos, rot);
                break;
            }
        }
	}
}
