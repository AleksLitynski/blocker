using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * After harsh critism of the static camera, a loosy goosy follow camera is here to save the day.
 * This is the class that makes some people sea-sick and some people go "wow".
 * 
 * The camera targets a location and turns to look at it, sometimes slowly, with a locked max distance off.
 * 
 */
public class FollowCamera : BlockerObject 
{
	public bool lockedCamera = false;
	public Transform targetLocation;
	private Vector3 oldLocation;
	public float speed = 0.03f;
	
	public Vector3 offset = new Vector3(0, 5, -10);
	public Vector3 lockedOffset = new Vector3(0, 1f, -2);
	
	private Vector3 targetPosition;
	private Quaternion targetRotation;
	
	bool isLocked = false;
	float timeSinceLockRequested = 0f;
	
	public override void Start () 
	{
		base.Start();
		//The camera follows a line projected outwards from the players arms.
		//This is an unplesently un-object oriented hack.
		targetLocation = transform.parent.Find("Doll/Model/Arms");
		updateOldLocation();
		calculateTarget(offset);
		
		transform.position = targetPosition;
		transform.rotation = targetRotation;
	}
	
	
	//Late update guarentees the camera isn't turning to look at an object that has already moved on.
	void LateUpdate ()
	{
		if(targetLocation)
		{
			//This drags the camera in rapidly when the player clicks the right mouse btn.
			//If it takes too long to zoom in (flying through space at 1000000mph), it will speed up
			if(lockedCamera)
			{
				calculateTarget(lockedOffset);
				if(timeSinceLockRequested == 0)//Makes the camera not jump when the target location changes abruptly
				{
					updateOldLocation();
				}
				timeSinceLockRequested += Time.deltaTime;
				if(Vector3.Distance(targetPosition, transform.position) > 0.5f && !isLocked)
				{
					float compSpeed = speed * Mathf.Pow(timeSinceLockRequested * 10, 2);
					
					transform.position = Vector3.Lerp(transform.position - (targetPosition - oldLocation), targetPosition, compSpeed) ;//+ (targetPosition - oldLocation);
					transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, compSpeed * 2);
					
				}
				else
				{				
					transform.position = targetPosition; //If its close enough, just snap into space (and lock in place. If we're moving faster than 0.5f / frame it'll jigger)
					transform.rotation = targetRotation;
					isLocked = true;
				}
				
			}
			else //do normal target following
			{
				calculateTarget(offset); //figure out what to look at
				var compSpeed = speed;
				compSpeed *= (Vector3.SqrMagnitude(targetPosition - transform.position)/25); //turn slowly towards the target
				
			
				//Lerp and slerp at the given speed
				transform.position = Vector3.Lerp(transform.position, targetPosition, compSpeed);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, compSpeed); //*2
				isLocked = false; //If we're not locked, remove the lock toggle
				timeSinceLockRequested = 0f; 
			}
			
			
			updateOldLocation();
		}
				
	}
	
	//Given a target position, set the target rotation
	void calculateTarget(Vector3 inputOffset)
	{
		targetPosition = targetLocation.position;
		targetPosition += targetLocation.forward * inputOffset.z;
		targetPosition += targetLocation.right * inputOffset.x;
		targetPosition += targetLocation.up * inputOffset.y;
		
		targetRotation = Quaternion.identity;
		if((targetLocation.position - targetPosition) != Vector3.zero)
		{
			targetRotation.SetLookRotation( targetLocation.position - targetPosition, targetLocation.up);
		}
	}
	
	// Store the old camera target location
	void updateOldLocation()
	{
		oldLocation = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
	}
	
	
	int averageOver = 1000;
	Queue<float> pastSpeeds = new Queue<float>();
	
	//queus a speed and (maybe) removes an old speed
	float addSpeed(float speed)
	{
		pastSpeeds.Enqueue(speed);
		if(pastSpeeds.Count > averageOver)
		{
			pastSpeeds.Dequeue();	
		}
		return getAverageSpeed();
	}
	
	//sums up the average spped over a queue of 1000 previous speeds
	float getAverageSpeed()
	{
		float sum = 0;
		foreach(float speed in pastSpeeds)
		{
			sum += speed;
		}
		return sum / pastSpeeds.Count;
	}
	
	
}
