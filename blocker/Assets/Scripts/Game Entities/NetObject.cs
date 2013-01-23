using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class NetObject : MonoBehaviour {
	
	[HideInInspector]
    public NetObject netObject;
	[HideInInspector]
	public ObjectStats objectStats = new ObjectStats();
	
	void Start ()
    {
        netObject = this;
	}
	
	public void setTransform(Vector3 pos, Vector3 rot)
	{
		rigidbody.isKinematic = true;
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
		
	}
	
	void FixedUpdate()
	{
		//if(objectStats.grav != Vector3.zero)
		//{
			objectStats.unitOppGrav = -1 * objectStats.grav.normalized;
		//}
		
		if(!Physics.Raycast(transform.position, -transform.up, collider.bounds.size.y/1.8f)) //if not on the ground
		{
			rigidbody.AddForce(objectStats.grav * Time.deltaTime);
		}
		
	}
	
	public void move(Vector3 disp, Quaternion forcedRotation)
	{
		
		
		Quaternion rotate = Quaternion.LookRotation(Vector3.Cross(transform.right, objectStats.unitOppGrav), objectStats.unitOppGrav);
		rotate = Quaternion.RotateTowards(transform.rotation, rotate, objectStats.maxGravRoll);
		rotate = rotate * forcedRotation;
		rigidbody.rotation = rotate;
		
		
	    rigidbody.AddRelativeForce(disp * Time.deltaTime);
		
		
		Debug.DrawLine(transform.position, transform.position + rigidbody.velocity);
		
	}
	

}
