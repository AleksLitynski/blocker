using UnityEngine;
using System.Collections;

public class bullet : NetObject {

	public override void Start () 
	{
		base.Start();
	}
	
	public Vector3 velocity;
	
	void Update () 
	{
		rigidbody.velocity += velocity;
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.GetComponent<WorldBounds>() != null)
		{
			Destroy(this.gameObject);
		}
	}
}
