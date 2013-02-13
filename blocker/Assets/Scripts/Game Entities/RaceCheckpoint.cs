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
	public bool alive;
	public bool awake;
	
	// physics variables
	public CollisionType collisionType = CollisionType.Sphere;
	public Vector3 scale = new Vector3(1,1,1);
	public Collider myCollider;
	
	// utility variables
	private bool unpack = false;
	
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
		
		if (alive)
		{
			// update the scale of the collision object.
			switch(collisionType)
			{
			case CollisionType.Sphere:
				scale = transform.FindChild("Sphere").localScale;
				(myCollider as SphereCollider).center = Vector3.zero;
				(myCollider as SphereCollider).radius = scale.x/2;
				
				(myCollider as SphereCollider).enabled = awake;
				transform.FindChild("Sphere").GetComponent<MeshRenderer>().enabled = awake;
				(gameObject.GetComponent("Halo") as Behaviour).enabled = !awake;
				break;
			case CollisionType.Box:
				Debug.Log ("Warning: This type of Checkpoint collider does not function properly (See CollisionType.Sphere)");
				if (scale != (myCollider as BoxCollider).size) (myCollider as BoxCollider).size = scale;
				break;
			}
		}
		else
		{
			switch(collisionType)
			{
			case CollisionType.Sphere:
				(myCollider as SphereCollider).enabled = false;
				break;
			case CollisionType.Box:
				(myCollider as BoxCollider).enabled = false;
				break;
			}
			transform.FindChild("Sphere").GetComponent<MeshRenderer>().enabled = false;
			(gameObject.GetComponent("Halo") as Behaviour).enabled = false;
		}
	}
	
	// relying on Start() is weird sometimes, so i safeguard it with a method that might
	// get run on the first update.
	void init()
	{
		scale = transform.FindChild("Sphere").localScale;
		// add a collision component based on the CollisionType enum. you can change the
		// CollisionType in the editor.
		switch(collisionType)
		{
		case CollisionType.Box:			
			myCollider = this.gameObject.AddComponent<BoxCollider>();
			myCollider.isTrigger = true;
			(myCollider as BoxCollider).enabled = false;
			break;
		case CollisionType.Cylinder:	
			myCollider = this.gameObject.AddComponent<CapsuleCollider>();
			myCollider.isTrigger = true;
			(myCollider as CapsuleCollider).enabled = false;
			// kill the endcaps so the collider is actually a cylinder.
			//cc.radius = 0; //doesnt do what you want it to
			break;
		case CollisionType.Sphere:		
			myCollider = this.gameObject.AddComponent<SphereCollider>();
			myCollider.isTrigger = true;
			(myCollider as SphereCollider).enabled = false;
			break;
		}
		
		//awake = true;
		alive = true;
		
		unpack = true;
	}
	
	public void AlterLifeState(bool tf)
	{
		alive = tf;
	}
	
	// collision handling
	void PlayerEnter(string name)
	{
		hitby = name;
	}
	void PlayerExit(string name)
	{
		hitby = null;	
	}
}
