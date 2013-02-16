using UnityEngine;
using System.Collections;

public class FollowCamera : BlockerObject 
{
	
	public Transform targetLocation;
	public Vector3 lookAtTarget;
//	public Vec cameraLookTowards;
	public float speed;
	public float forwardDistance = 100;
	
	public Vector3 offset;
	
	public Vector3 targetPosition;
	public Quaternion targetRotation;
	
	public override void Start () 
	{
		base.Start();
		targetLocation = transform.parent.Find("Doll");

		offset = new Vector3(0, 5, -10);
		speed = 0.03f;
		
		calculateTarget();
		
		transform.position = targetPosition;
		transform.rotation = targetRotation;
	}
	
	void LateUpdate ()
	{
		float compSpeed = speed;
		if(Input.GetKey(KeyCode.Q))
		{
			offset = new Vector3(-3f, 1f, -2f);
			speed = 0.06f;
		}
		else
		{
			offset = new Vector3(0, 5, -10);
			speed = 0.03f;	
			compSpeed *= (Vector3.Distance(targetPosition, transform.position)/5);
		}
		
		calculateTarget();
		
		
		transform.position = Vector3.Lerp(transform.position, targetPosition, compSpeed);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, compSpeed * 2);
		
		
		Debug.DrawLine(targetPosition, lookAtTarget, Color.blue);
		
	}
	
	
	void calculateTarget()
	{
		targetPosition = targetLocation.position;
		targetPosition += targetLocation.forward * offset.z;
		targetPosition += targetLocation.right * offset.x;
		targetPosition += targetLocation.up * offset.y;
		
		Transform targetLookAt = targetLocation.Find("Model/Arms");
		lookAtTarget = targetLookAt.position + targetLookAt.forward * forwardDistance;
		
		targetRotation = Quaternion.identity;
		targetRotation.SetLookRotation( lookAtTarget - targetPosition, targetLocation.up);//targetLocation.position
	}
	
}
