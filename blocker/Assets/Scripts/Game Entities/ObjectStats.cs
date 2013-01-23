using UnityEngine;
using System.Collections;

public class ObjectStats : MonoBehaviour
{
	public Vector3 grav = new Vector3(0, -9.81f, 0);
	public float maxGravRoll = 10f;
	public Vector3 unitOppGrav = new Vector3(0, 9.81f, 0);
	//public Vector3 normalOfLastHit = new Vector3(0,0,0);
}
