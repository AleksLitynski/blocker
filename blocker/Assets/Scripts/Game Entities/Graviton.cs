using UnityEngine;
using System.Collections;

public class Graviton : NetObject 
{
	public bool pointGravity = true;
	public bool plateGravity = true;
	public float bigG = 0.001f;
	
	public void Awake()
	{
		//freezes graviton. Could be removed later?
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		rigidbody.drag = 0;
		rigidbody.angularDrag = 0;
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		rigidbody.mass = 300;
		
		if(collider)
		{
			plateGravity = !collider.isTrigger; //if the player can walk into it, its a point gravity
			pointGravity = !plateGravity;
		}
	}
	
	
	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		collisionDispatch(hit.controller.gameObject, hit.normal);
	}
	
	public void OnTriggerStay (Collider other) 
	{
		collisionDispatch(other.gameObject, Vector3.zero);
	}
	
	public void collisionDispatch(GameObject other, Vector3 hitNormal)
	{
		if(other.tag == "Player" && other.name == "player " + Network.player.ToString())
		{
			if(pointGravity)
			{
				pointHit(other);
			}
			if(plateGravity)
			{
				surfaceHit(other, hitNormal);
			}
		}
	}
	
	public void pointHit(GameObject player)
	{	
		
		var top = rigidbody.mass * player.rigidbody.mass;
		var bottom = Mathf.Sqrt(Vector3.Distance(transform.position, player.transform.position));
		var magnitude = bigG * player.rigidbody.mass * (top/bottom);
		
		var direction = (player.transform.position - transform.position).normalized;
		
		
		objectStats.grav = -magnitude * direction;
	}
	
	public void surfaceHit(GameObject player, Vector3 surfaceNormal)
	{
		player.GetComponent<NetPlayer>().netObject.objectStats.grav = -(surfaceNormal * bigG * rigidbody.mass);
	}
	
	
}