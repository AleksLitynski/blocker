using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ObjectStats))]
public class NetObject : BlockerObject {
	
	[HideInInspector]
    public NetObject netObject;
	[HideInInspector]
	public ObjectStats objectStats;
	
	private Vector3 currentGrav = Vector3.zero;
	
	private Vector3 prevVelo = Vector3.zero;
	private float maxDiff = 0.1f;
	
	
	public override void Start ()
    {
		//rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		gameObject.tag = "netObject";
        netObject = this;
		objectStats = gameObject.GetComponent<ObjectStats>();
		
		gameObject.layer = LayerMask.NameToLayer("netObject");
		
		base.Start();
		
		
	}
	
	public void setTransform(Vector3 pos, Vector3 rot)
	{
		//rigidbody.isKinematic = true;
		transform.position = pos;
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
				currentGrav += (objectStats.grav * Time.deltaTime);
				rigidbody.AddForce(currentGrav, ForceMode.Impulse);//rigidbody.velocity + 
			}
			else
			{
				currentGrav = Vector3.zero;
			}
			world.networkView.RPC("setTransform", RPCMode.Others, transform.position, transform.rotation.eulerAngles, this.name);
			
			if(Mathf.Abs(prevVelo.x - rigidbody.velocity.x) > maxDiff || Mathf.Abs(prevVelo.y - rigidbody.velocity.y) > maxDiff || Mathf.Abs(prevVelo.z - rigidbody.velocity.z) > maxDiff)
			{
				world.networkView.RPC("setTransform", RPCMode.Others, transform.position, transform.rotation.eulerAngles, name);
			}
			if(Time.time % 1 == 0)
			{
				prevVelo = rigidbody.velocity;	
			}
			
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
