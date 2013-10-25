using UnityEngine;
using System.Collections;

/* This is an un-used class. It was meant to generic-isize guns, but
 * there is only one type of gun in the game.
 * 
 */
public class Gun : BlockerObject {

	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	public void fireGun()
	{
		if(Network.peerType == NetworkPeerType.Server)
		{
			
		}
	}
}
