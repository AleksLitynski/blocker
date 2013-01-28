using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ObjectStats))]
public class NetObject : BlockerObject {
	
	[HideInInspector]
    public NetObject netObject;
	[HideInInspector]
	public ObjectStats objectStats;
	
	
	public override void Start ()
    {
		gameObject.tag = "netObject";
        netObject = this;
		objectStats = gameObject.GetComponent<ObjectStats>();
		
		base.Start();
	}
	
	public void setTransform(Vector3 pos, Vector3 rot)
	{
		rigidbody.isKinematic = true;
		transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
		transform.rotation = Quaternion.Euler(rot);
		
	}
	
	public virtual void FixedUpdate()
	{
		if(Network.peerType == NetworkPeerType.Server)
		{
			if(objectStats.grav != Vector3.zero)
			{
				objectStats.unitOppGrav = -1 * objectStats.grav.normalized;
			}
			
			if(!Physics.Raycast(transform.position, -transform.up, ((collider.bounds.size.x + collider.bounds.size.y + collider.bounds.size.z)/3)  * 1.1f)) //if not on the ground
			{
				rigidbody.AddForce((objectStats.grav * Time.deltaTime), ForceMode.Impulse);//rigidbody.velocity + 
			}
			world.networkView.RPC("setTransform", RPCMode.Others, transform.position, transform.rotation.eulerAngles, this.name);
               
		}
		
	}
	
	public void move(Vector3 disp, Quaternion forcedRotation)
	{
		if(objectStats != null)
		{
			Quaternion rotate = Quaternion.LookRotation(Vector3.Cross(transform.right, objectStats.unitOppGrav), objectStats.unitOppGrav);
			rotate = Quaternion.RotateTowards(transform.rotation, rotate, objectStats.maxGravRoll);
			rotate = rotate * forcedRotation;
			rigidbody.rotation = rotate;
			
		}
		
		
	    rigidbody.AddRelativeForce(disp * Time.deltaTime);
		
		Debug.DrawRay(transform.position, transform.forward, Color.red, 10);
		Debug.DrawLine(transform.position, transform.position + rigidbody.velocity);
		
	}
	

}
