using UnityEngine;
using System.Collections;

public class InputReceiver : BlockerObject 
{
	
	int maxBullets = 10;
	
	[RPC]
    public void AddInput(float f, float s, float tR, float tU, bool j, bool f1, bool f2, bool sp, bool c, int localNumber, NetworkMessageInfo info)
    {
		if(menuManager.gameState == MenuManager.GameState.Game)//you can only move/shoot/etc durring the game
		{
	        foreach (NetPlayer player in playerManager.players)
	        {
	            if (player.networkPlayer == info.sender && player.localPlayerNumber == localNumber)
	            {
	                player.move(new InputCollection(player, f, s, tR, tU, j, f1, f2, sp, c));
					
					networkView.RPC("setPlayerTransform", RPCMode.Others, player.transform.position, player.transform.rotation.eulerAngles, player.playerArms.rotation, player.player.name);
					
					// fire bullets
					if(f1 && !player.GetComponent<PlayerStats>().FiredSinceMouseDown && menuManager.gameState == MenuManager.GameState.Game) //ummmm? Maybe this does something??????
					{
						Screen.lockCursor = true;
						string name = "testBullet" + Random.Range(0,1000000);
						networkView.RPC("spawnObject", RPCMode.All, player.transform.position + player.playerArms.forward * 1.5f , player.transform.rotation.eulerAngles, name, "testBullet", "World/Bullets");
						networkView.RPC ("setBulletVelocity", RPCMode.All, player.playerArms.forward * 250000, "World/Bullets/"+name);
						networkView.RPC ("setObjectGravity", RPCMode.All, player.objectStats.grav, "World/Bullets/"+name);
						player.GetComponent<PlayerStats>().FiredSinceMouseDown = true;
						
						Transform bullets = world.transform.Find("Bullets");
						if(bullets.childCount > maxBullets)
						{
							bullet oldestBullet = bullets.GetChild(0).gameObject.GetComponent<bullet>();
							for(int i = 0; i < bullets.childCount; i++)
							{
								bullet current = bullets.GetChild(i).GetComponent<bullet>();
								if(current.creationTime < oldestBullet.creationTime)
								{
									oldestBullet = current;	
								}
							}
							networkView.RPC("removeObject", RPCMode.All, "World/Bullets/" + oldestBullet.name);
						}
						
					}
					if(!f1)
					{
						player.GetComponent<PlayerStats>().FiredSinceMouseDown = false;
					}
					if(f2)
					{
						networkView.RPC ("setScopedIn", RPCMode.All, true, player.player.name);
					}
					else
					{
						networkView.RPC ("setScopedIn", RPCMode.All, false, player.player.name);
					}
					
	                break;
	            }
	        }
		}
    }
	
	[RPC]
	public void setScopedIn(bool scopedIn, string name)
	{
		foreach(NetPlayer player in playerManager.players)
		{
			if(player.player.name == name)
			{
				if(playerManager.localPlayers.IndexOf(player) != -1)
				{
					player.playerCamera.GetComponent<FollowCamera>().lockedCamera = scopedIn;	
				}
				player.laserOn = scopedIn;
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
            if (player.player.name == playerName)
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
