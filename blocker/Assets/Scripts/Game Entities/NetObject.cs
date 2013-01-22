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
	
	void Update()
	{
		if(objectStats.grav != Vector3.zero)
		{
			objectStats.unitOppGrav = -1 * objectStats.grav.normalized;
		}
		
		if(!Physics.Raycast(transform.position, -transform.up, collider.bounds.size.y/1.8f))
		{
			objectStats.gravVelo += objectStats.grav * Time.deltaTime;
			rigidbody.AddForce(objectStats.gravVelo);
		}
		else
		{
			objectStats.gravVelo = Vector3.zero;
		}
		
	}
	
	public void move(Vector3 disp, Quaternion rotate, Quaternion forcedRotation)
	{
		
		
		rotate = Quaternion.RotateTowards(transform.rotation, rotate, objectStats.maxGravRoll);
		rotate = rotate * forcedRotation;
		rigidbody.rotation = rotate;
		
		
	    rigidbody.AddForce(disp * Time.deltaTime);
	}
	

}
