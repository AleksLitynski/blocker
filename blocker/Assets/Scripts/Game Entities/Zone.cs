using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone : MonoBehaviour 
{
	public enum CollisionType {Cylinder, Box, Sphere};
	
	// game variables
	public int orderInRace;
	public int currentPoints = 0;
	public int maxPoints = 1;
	public int scoreReward = 1;
	//public string hitby;
	public List<string> hitList;
	
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
		
		// update the scale of the collision object.
		switch(collisionType)
		{
		case CollisionType.Sphere:
			scale = transform.FindChild("Sphere").localScale;
			(myCollider as SphereCollider).center = Vector3.zero;
			(myCollider as SphereCollider).radius = scale.x/2;
			/*var haloComponent = GetComponent("Halo");
			Debug.Log(haloComponent.GetType().GetProperty("size"));
			var  haloSizeProperty = System.Reflection.Assembly.GetExecutingAssembly(). GetType("MyType");//.GetType().GetProperty("size");
			haloSizeProperty.SetValue(haloComponent, 100, null);*/
			break;
		case CollisionType.Box:
			Debug.Log ("Warning: This type of Checkpoint collider does not function properly (See CollisionType.Sphere)");
			if (scale != (myCollider as BoxCollider).size) (myCollider as BoxCollider).size = scale;
			break;
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
	
	// collision handling
	void PlayerEnter(string name)
	{
		//hitby = name;
		hitList.Add(name);
	}
	void PlayerExit(string name)
	{
		//hitby = null;
		hitList.RemoveAt(hitList.IndexOf(name));
	}
	
	public void toggleHalo(bool tf)
	{
		(gameObject.GetComponent("Halo") as Behaviour).enabled = tf;
	}
}
