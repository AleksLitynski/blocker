using UnityEngine;
using System.Collections;

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
