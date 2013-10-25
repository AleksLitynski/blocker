using UnityEngine;
using System.Collections;

/*
 * Players tend to fall off the map.
 * This is the container that respawns them when they fall too far.
 */
public class WorldBounds : BlockerObject 
{
	// enums
	enum CollisionType {Sphere, Box};
	CollisionType collisionType;
	
	// references
	Collider collScript;
	
	
	void Awake()
	{
		// initialization
		collisionType = CollisionType.Box;
		
		setCollisionType(collisionType);
	}
	
	// Use this for initialization
	public override void Start () 
	{
		base.Start();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void PlayerEnter(string playerName){}
	
	void PlayerExit(string playerName)
	{
		// respawn the player if they fall out of the world bounds.
		mapManager.respawnPlayer(playerName);
	}
	
	//This toggles if the world bounds are a sphere or a rectangle.
	void setCollisionType(CollisionType ct)
	{
		// mutate dat shit
		collisionType = ct;
		
		// remove any collider currently on the worldbounds.
		Destroy (gameObject.GetComponent<BoxCollider>());
		Destroy (gameObject.GetComponent<SphereCollider>());
		
		// based on the collisionType variable, add a new collider with some default
		// parameters.
		switch(collisionType)
		{
		case CollisionType.Sphere:
			collScript = this.gameObject.AddComponent<SphereCollider>();
			collScript.isTrigger = true;
			setScale(500, 0, 0);
			break;
		case CollisionType.Box:
			collScript = this.gameObject.AddComponent<BoxCollider>();
			collScript.isTrigger = true;
			setScale (1000,1000, 1000);
			break;
		}
	}
	
	//Changes the side of the collider.
	void setScale(float value1, float value2, float value3)
	{
		switch(collisionType)
		{
		case CollisionType.Sphere:
			// have to remember this is radius?? takes half the value for the same
			// width with the same value on a boxcollider. x width: 5 radius or
			// 10 x size
			// I THINK
			(collScript as SphereCollider).radius = value1;
			break;
		case CollisionType.Box:
			(collScript as BoxCollider).size = new Vector3(value1,value2,value3);
			break;
		}
	}
}
