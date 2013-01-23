using UnityEngine;
using System.Collections;

public class Graviton : BlockerObject 
{
	public bool pointGravity = true;
	public bool plateGravity = true;
	public float bigG = 1;
	
	public void Awake()
	{
		//freezes graviton. Could be removed later?
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		rigidbody.drag = 0;
		rigidbody.angularDrag = 0;
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		rigidbody.mass = 10;
		
		if(collider)
		{
			plateGravity = !collider.isTrigger; //if the player can walk into it, its a point gravity
			pointGravity = !plateGravity;
		}
		
	}
	
	
	public void OnCollisionStay(Collision collisionInfo)
	{
		if(plateGravity)
		{
			Vector3 normal = Vector3.zero;
			foreach(var contact in collisionInfo.contacts)
			{
				normal += contact.normal;
			}
			normal = normal / collisionInfo.contacts.Length;
		//	normal = new Vector3(Mathf.Round(normal.x), Mathf.Round(normal.y), Mathf.Round(normal.z));
			surfaceHit(collisionInfo.gameObject, normal);
		}
	}
	/*
	public void OnTriggerStay (Collider other) 
	{
		if(pointGravity)
		{
			pointHit(other.gameObject);
		}
	}
	
	public void pointHit(GameObject other)
	{	
		
		var top = rigidbody.mass * other.rigidbody.mass;
		var bottom = Mathf.Sqrt(Vector3.Distance(transform.position, other.transform.position));
		var magnitude = bigG * other.rigidbody.mass * (top/bottom);
		
		var direction = (other.transform.position - transform.position).normalized;
		
		
		other.GetComponent<NetObject>().objectStats.grav = magnitude * direction;
	}*/
	
	public void surfaceHit(GameObject other, Vector3 surfaceNormal)
	{
		//Debug.Log(other.GetComponent<NetObject>().objectStats.grav + " " + surfaceNormal);
		
		other.GetComponent<ObjectStats>().grav = surfaceNormal * bigG * rigidbody.mass;
	}
	
	
}