using UnityEngine;
using System.Collections;


/* This object represents our two types of alterantive gravity. (only one plateGravity is used)
 * Plate Gravity pulls an object towards the surface its normal constitutes,
 * Point gravity pulls an object towards a given point in space.
 * 
 */
[RequireComponent(typeof(Rigidbody))]
public class Graviton : BlockerObject 
{
	//The Type of gravity and its strength
	public bool pointGravity = true;
	public bool plateGravity = true;
	public float bigG = 1;
	
	public void Awake()
	{
		//Prevent the graviton from moving in any way. This restricition could be lifted for hillarious effect
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
	
	public override void Start()
	{
		base.Start ();	//Propagate the start method up.
	}
	
	
	//This indicates the object is on the surface of the graviton. IE: It is colliding.
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
	
	//This indicates the player is within the object, ie: it is a point gravity
	public void OnTriggerStay (Collider other) 
	{
		if(pointGravity)
		{
			pointHit(other.gameObject);
		}
	}
	
	//Applies gravity towards a point
	public void pointHit(GameObject other)
	{	
		
		//Calculates the magnitude of the force
		var top = rigidbody.mass * other.rigidbody.mass;
		var bottom = Mathf.Sqrt(Vector3.Distance(transform.position, other.transform.position));
		var magnitude = bigG * other.rigidbody.mass * (top/bottom);
		
		var direction = (other.transform.position - transform.position).normalized; //Direction of the force
		
		if(other.GetComponent<ObjectStats>())
		{
			other.GetComponent<ObjectStats>().grav = -magnitude * direction; //applies force in direction
		}
	}
	
	//Applies gravity relative to the normal.
	public void surfaceHit(GameObject other, Vector3 surfaceNormal)
	{
		//Debug.Log(other.GetComponent<NetObject>().objectStats.grav + " " + surfaceNormal);
		
		other.GetComponent<ObjectStats>().grav = surfaceNormal * bigG * other.rigidbody.mass;
	}
	
	//This is clearly obsolete
	public void CollideWithPlayer(string a)
	{
		
	}
	
}