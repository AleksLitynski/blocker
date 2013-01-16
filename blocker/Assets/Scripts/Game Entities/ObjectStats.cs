using UnityEngine;
using System.Collections;

public class ObjectStats
{
	public Vector3 grav = new Vector3(0, -50.0f, 0);
	public float maxGravRoll = 5;
	public Vector3 unitOppGrav;
	public Vector3 normalOfLastHit = new Vector3(0,0,0);
	public Vector3 gravVelo = new Vector3(0,0,0);
}
