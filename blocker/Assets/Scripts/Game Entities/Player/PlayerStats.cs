using UnityEngine;
using System.Collections;

public class PlayerStats 
{

	
	public float speed = 1;
	public float turnSpeed = 40;
	public float jump = 75;
	public Vector3 grav = new Vector3(0, -3, 0);
	public float fric = 0.90f;
	public Vector3 velo = new Vector3(0, 0, 0);
	public float maxGravRoll = 5;
	public float mass = 10f;
	public bool isGrounded = false;
	public Vector3 unitOppGrav;
	public Vector3 normalOfLastHit = new Vector3(0,0,0);
	
	public int score = 0;
	public float health = 100;


	public override string ToString()
	{
		return "Speed: " + speed + 
		" || Turn Speed: " + turnSpeed + 
		" || Jump: " + jump + 
		" || grav: " + grav + 
		" || grounded: " + isGrounded + 
		" || fric: " + fric + 
		" || velo: " + velo + 
		" || Max Grav Roll: " + maxGravRoll +
		" || Score: " + score + 
		" || Health: " + health; 
	}
}
