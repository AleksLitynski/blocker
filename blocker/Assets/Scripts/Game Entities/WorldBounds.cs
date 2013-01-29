using UnityEngine;
using System.Collections;

public class WorldBounds : MonoBehaviour 
{
	// enums
	enum CollisionType {Sphere, Box};
	CollisionType collisionType;
	
	// references
	Collider collScript;
	
	// vars
	Vector3 scale;
	
	void Awake()
	{
		// initialization
		collisionType = CollisionType.Box;
		scale = new Vector3();
		
		setCollisionType(collisionType);
	}
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void PlayerEnter(string playerName)
	{
		// uhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh
	}
	
	void PlayerExit(string playerName)
	{
		// respawn the player if they fall out of the world bounds.
		GameObject player = GameObject.Find(playerName);
		
		player.rigidbody.velocity = new Vector3();
		player.transform.position = GameObject.Find ("Spawn").transform.position;
	}
	
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
			setScale(500);
			break;
		case CollisionType.Box:
			collScript = this.gameObject.AddComponent<BoxCollider>();
			collScript.isTrigger = true;
			setScale (1000,1000,1000);
			break;
		}
	}
	
	void setScale(float value1, float value2 = 0, float value3 = 0)
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
