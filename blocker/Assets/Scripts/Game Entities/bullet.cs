using UnityEngine;
using System.Collections;

//Bullets are farily mundane networked objects, but
//they move with a predetermied velocity, on top of their motion induced by physics
public class bullet : NetObject {

	public override void Start () 
	{
		base.Start();
	}
	
	public Vector3 velocity;
	
	void Update () 
	{
		//Just keep chugging along
		GetComponent<Rigidbody>().velocity += velocity;
	}
	
	//When the bullet exits the map, destory the bullet.
	void OnTriggerExit(Collider other)
	{
		if(other.GetComponent<WorldBounds>() != null)
		{
			Destroy(this.gameObject);
		}
	}
}
