using UnityEngine;
using System.Collections;

public class RaceCheckpoint : MonoBehaviour 
{
	public enum CollisionType {Cylinder, Box, Sphere};
	
	// game variables
	public int orderInRace;
	public int currentPoints = 0;
	public int maxPoints = 1;
	public int scoreReward = 1;
	public int hitby;
	
	// physics variables
	public CollisionType collisionType;
	
	// utility variables
	private bool unpack = false;
	public bool hit = false;
	
	void Start()
	{
		init();
	}
	
	void Update () 
	{
		if (!unpack)
		{
			init();
		}
	}
	
	// relying on Start() is weird sometimes, so i safeguard it with a method that might
	// get run on the first update.
	void init()
	{
		// add a collision component based on the CollisionType enum. you can change the
		// CollisionType in the editor.
		switch(collisionType)
		{
		case CollisionType.Box:			
			this.gameObject.AddComponent<BoxCollider>();
			BoxCollider bc = this.gameObject.GetComponent<BoxCollider>();
			bc.isTrigger = true;
			break;
		case CollisionType.Cylinder:	
			this.gameObject.AddComponent<CapsuleCollider>();
			CapsuleCollider cc = this.gameObject.GetComponent<CapsuleCollider>();
			cc.isTrigger = true;
			// kill the endcaps so the collider is actually a cylinder.
			//cc.radius = 0; //doesnt do what you want it to
			break;
		case CollisionType.Sphere:		
			this.gameObject.AddComponent<SphereCollider>();
			SphereCollider sc = this.gameObject.GetComponent<SphereCollider>();
			sc.isTrigger = true;
			break;
		}
		
		unpack = true;	
	}
}
