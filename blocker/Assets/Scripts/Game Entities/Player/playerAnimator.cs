using UnityEngine;
using System.Collections;

//This code plays the animations on the player model. They animate based on the direction the given player is moving.
public class playerAnimator : BlockerObject
{
	GameObject model;
	public override void Start () 
	{
		base.Start();
		
		model = transform.Find("Model").gameObject;
	}
	
	void Update () 
	{
		
		Vector3 localMotion = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
		
		
		
		//localMotion y position animation handlers
		if(localMotion.y > 5.0)
		{
			
			model.GetComponent<Animation>().CrossFade("jump",0.4f);
			model.GetComponent<Animation>().wrapMode = WrapMode.Once;
			//Debug.Log("motion is jumping");
			//Debug.Log(localMotion.y);
			
		}
		
		
		//localMotion idle animation handlers
		if(localMotion.z < 0.1 && localMotion.z >= 0.0)
		{
			
			model.GetComponent<Animation>().CrossFade("idle",0.5f);
			model.GetComponent<Animation>().wrapMode = WrapMode.Once;
			//Debug.Log("idle is running");
			//Debug.Log(localMotion.z);
			
			if(localMotion.y > 5.0)
			{
				
				model.GetComponent<Animation>().Blend("jump",1.0f,0.1f);
				model.GetComponent<Animation>().wrapMode = WrapMode.Once;
				//Debug.Log("motion is jumping");
				//Debug.Log(localMotion.y);
				
			}
			
		}
		
		
		//localMotion z position animation handlers
		if(localMotion.z > 0.1)
		{
			
			model.GetComponent<Animation>().CrossFade("run",0.4f);
			model.GetComponent<Animation>().wrapMode = WrapMode.Once;
			//Debug.Log("motion is running");
			//Debug.Log(localMotion.z);
			
			if(localMotion.y > 5.0)
			{
				
				model.GetComponent<Animation>().Blend("jump",1.0f,0.1f);
				model.GetComponent<Animation>().wrapMode = WrapMode.Once;
				//Debug.Log("motion is jumping");
				//Debug.Log(localMotion.y);
				
			}
			
		}
		
		if(localMotion.z < -0.1)
		{
			
			model.GetComponent<Animation>().CrossFade("run",0.4f);
			model.GetComponent<Animation>().wrapMode = WrapMode.Once;
			//Debug.Log("motion is running");
			//Debug.Log(localMotion.z);
			
			if(localMotion.y > 5.0)
			{
				
				model.GetComponent<Animation>().Blend("jump",1.0f,0.1f);
				model.GetComponent<Animation>().wrapMode = WrapMode.Once;
				//Debug.Log("motion is jumping");
				//Debug.Log(localMotion.y);
				
			}
			
		}

		
		//localMotion x position animation handlers
		if(localMotion.x > 0.1)
		{
			
			model.GetComponent<Animation>().CrossFade("run",0.4f);
			model.GetComponent<Animation>().wrapMode = WrapMode.Once;
			//Debug.Log("motion is running");
			//Debug.Log(localMotion.z);
			
		}
		
		if(localMotion.x < -0.1)
		{
			
			model.GetComponent<Animation>().CrossFade("run",0.4f);
			model.GetComponent<Animation>().wrapMode = WrapMode.Once;
			//Debug.Log("motion is running");
			//Debug.Log(localMotion.z);
			
		}
		
	
	}
}

