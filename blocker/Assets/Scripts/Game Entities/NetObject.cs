using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class NetObject : MonoBehaviour {

    public NetObject netObject;
	public ObjectStats objectStats = new ObjectStats();
	
	void Start () 
    {
        netObject = this;
	}
	
	public void setTransform(Vector3 pos, Vector3 rot)
	{
		Debug.Log(gameObject.name);
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
		
		if(!Physics.Raycast(transform.position, -transform.up, collider.bounds.size.y/1.8f)) //, LayerMask.NameToLayer("Player")
		{
			objectStats.gravVelo += objectStats.grav * Time.deltaTime;
			rigidbody.AddForce(objectStats.gravVelo);
		}
		else
		{
			objectStats.gravVelo = Vector3.zero;
		}
		
	}
	

}
