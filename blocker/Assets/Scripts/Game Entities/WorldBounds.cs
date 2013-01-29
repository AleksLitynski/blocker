using UnityEngine;
using System.Collections;

public class WorldBounds : MonoBehaviour 
{
	// enums
	enum CollisionType {Sphere, Box};
	CollisionType collisionType;
	
	// references
	Collider collScript;
	
	void Awake()
	{
		switch(collisionType)
		{
		case CollisionType.Sphere:
			collScript = this.gameObject.AddComponent<SphereCollider>();
			collScript.isTrigger = true;
			break;
		case CollisionType.Box:
			collScript = this.gameObject.AddComponent<BoxCollider>();
			collScript.isTrigger = true;
			break;
		}
		
		collScript.transform.localScale = new Vector3(100,100,100);
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
		
	}
	
	void PlayerExit(string playerName)
	{
		// respawn the player if they fall out of the world bounds.
		GameObject player = GameObject.Find(playerName);
		
		player.rigidbody.velocity = new Vector3();
		player.transform.position = GameObject.Find ("Spawn").transform.position;
		
	}
}
