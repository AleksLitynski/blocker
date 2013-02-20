using UnityEngine;
using System.Collections;

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
		
		Vector3 localMotion = transform.InverseTransformDirection(rigidbody.velocity);
		
		
		
		//localMotion y position animation handlers
		if(localMotion.y > 5.0)
		{
			
			model.animation.CrossFade("jump",0.4f);
			model.animation.wrapMode = WrapMode.Once;
			//Debug.Log("motion is jumping");
			//Debug.Log(localMotion.y);
			
		}
		
		
		//localMotion idle animation handlers
		if(localMotion.z < 0.1 && localMotion.z >= 0.0)
		{
			
			model.animation.CrossFade("idle",0.5f);
			model.animation.wrapMode = WrapMode.Once;
			//Debug.Log("idle is running");
			//Debug.Log(localMotion.z);
			
			if(localMotion.y > 5.0)
			{
				
				model.animation.Blend("jump",1.0f,0.1f);
				model.animation.wrapMode = WrapMode.Once;
				//Debug.Log("motion is jumping");
				//Debug.Log(localMotion.y);
				
			}
			
		}
		
		
		//localMotion z position animation handlers
		if(localMotion.z > 0.1)
		{
			
			model.animation.CrossFade("run",0.4f);
			model.animation.wrapMode = WrapMode.Once;
			//Debug.Log("motion is running");
			//Debug.Log(localMotion.z);
			
			if(localMotion.y > 5.0)
			{
				
				model.animation.Blend("jump",1.0f,0.1f);
				model.animation.wrapMode = WrapMode.Once;
				//Debug.Log("motion is jumping");
				//Debug.Log(localMotion.y);
				
			}
			
		}
		
		if(localMotion.z < -0.1)
		{
			
			model.animation.CrossFade("run",0.4f);
			model.animation.wrapMode = WrapMode.Once;
			//Debug.Log("motion is running");
			//Debug.Log(localMotion.z);
			
			if(localMotion.y > 5.0)
			{
				
				model.animation.Blend("jump",1.0f,0.1f);
				model.animation.wrapMode = WrapMode.Once;
				//Debug.Log("motion is jumping");
				//Debug.Log(localMotion.y);
				
			}
			
		}

		
		//localMotion x position animation handlers
		if(localMotion.x > 0.1)
		{
			
			model.animation.CrossFade("run",0.4f);
			model.animation.wrapMode = WrapMode.Once;
			//Debug.Log("motion is running");
			//Debug.Log(localMotion.z);
			
		}
		
		if(localMotion.x < -0.1)
		{
			
			model.animation.CrossFade("run",0.4f);
			model.animation.wrapMode = WrapMode.Once;
			//Debug.Log("motion is running");
			//Debug.Log(localMotion.z);
			
		}
		
	
	}
}

