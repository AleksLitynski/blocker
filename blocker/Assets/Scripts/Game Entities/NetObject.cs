using UnityEngine;
using System.Collections;

/* Any in-game (physical, on the map) we're synchronizing over the network inherits from NetObject
 * 
 */

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
	public float creationTime = 0;
	
	//Tags iself and generates a reference to its stats. 
	public override void Start ()
    {
		//rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		gameObject.tag = "netObject";
        netObject = this;
		objectStats = gameObject.GetComponent<ObjectStats>();
		
		gameObject.layer = LayerMask.NameToLayer("netObject");
		
		base.Start();
		
		
	}
	
	//Sets the position of the object. This function was once more complex
	public void setTransform(Vector3 pos, Vector3 rot)
	{
		//rigidbody.isKinematic = true;
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
		
	}
		
	//Physics is updated on the fixed update. This keeps the game running at a smoother frame-rate
	public virtual void FixedUpdate()
	{
		//The server is the only one who updates objects. Very dumb terminals. The internet is quite fast,though.
		//The else statement would be the proper place for some client side prediction, if it existsed.
		if(Network.peerType == NetworkPeerType.Server)
		{
			//Settin the "up" direction; the direction opposite gravity
			if(objectStats.grav != Vector3.zero)
			{
				objectStats.unitOppGrav = -1 * objectStats.grav.normalized;
			}
			RaycastHit hit;


			bool hitSomething = Physics.Raycast(new Ray(transform.position, -transform.up), out hit, 1.0f);

			if(!hitSomething ) //if not on the ground, fall downwards
			{
				currentGrav += (objectStats.grav * Time.deltaTime);
				GetComponent<Rigidbody>().AddForce(currentGrav, ForceMode.Impulse);//rigidbody.velocity + 
			}
			else //Otherwise, apply the normal force. This was a big issue, as downward forces cumulated to quite high numbers, causing collision issues
			{
				if(hit.collider.isTrigger != true)
				{
					currentGrav = Vector3.zero;
				}
				else
				{
					currentGrav += (objectStats.grav * Time.deltaTime);
					GetComponent<Rigidbody>().AddForce(currentGrav, ForceMode.Impulse);//rigidbody.velocity + 	
				}
			}
			//Move the object for everybody. 
			//world.networkView is the primary RPC channel used by the game. This helps to consolidate network traffic and keep the code manageable.
			world.GetComponent<NetworkView>().RPC("setTransform", RPCMode.Others, transform.position, transform.rotation.eulerAngles, this.name);
			
			if(Mathf.Abs(prevVelo.x - GetComponent<Rigidbody>().velocity.x) > maxDiff || Mathf.Abs(prevVelo.y - GetComponent<Rigidbody>().velocity.y) > maxDiff || Mathf.Abs(prevVelo.z - GetComponent<Rigidbody>().velocity.z) > maxDiff)
			{
				world.GetComponent<NetworkView>().RPC("setTransform", RPCMode.Others, transform.position, transform.rotation.eulerAngles, name);
			}
			if(Time.time % 1 == 0)
			{
				prevVelo = GetComponent<Rigidbody>().velocity;	
			}
			
		}
		
	}
	
	//This moves the object based on a distance and a direction.
	public void move(Vector3 disp, Quaternion forcedRotation)
	{
		if(objectStats != null) //If there is an object to update...
		{
			Quaternion rotate = Quaternion.LookRotation(Vector3.Cross(transform.right, objectStats.unitOppGrav), objectStats.unitOppGrav);
			rotate = Quaternion.RotateTowards(transform.rotation, rotate, objectStats.maxGravRoll);
			rotate = rotate * forcedRotation;
			GetComponent<Rigidbody>().rotation = rotate;
			
		}
		
		//Applies the force over time.
	    GetComponent<Rigidbody>().AddRelativeForce(disp * Time.deltaTime);
		
		
	}
	
	

}
