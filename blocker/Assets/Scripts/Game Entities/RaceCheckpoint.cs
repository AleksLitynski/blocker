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
	public string hitby;
	
	// physics variables
	public CollisionType collisionType = CollisionType.Box;
	public Vector3 scale;
	public Collider myCollider;
	
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
		// update the scale of the collision object.
		switch(collisionType)
		{
		case CollisionType.Box:
			if (scale != (myCollider as BoxCollider).size) (myCollider as BoxCollider).size = scale;
			break;
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
			myCollider = this.gameObject.AddComponent<BoxCollider>();
			myCollider.isTrigger = true;
			break;
		case CollisionType.Cylinder:	
			myCollider = this.gameObject.AddComponent<CapsuleCollider>();
			myCollider.isTrigger = true;
			// kill the endcaps so the collider is actually a cylinder.
			//cc.radius = 0; //doesnt do what you want it to
			break;
		case CollisionType.Sphere:		
			myCollider = this.gameObject.AddComponent<SphereCollider>();
			myCollider.isTrigger = true;
			break;
		}
		
		unpack = true;	
	}
	
	void CollideWithPlayer(string name)
	{
		hitby = name;
	}
}
