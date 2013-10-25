using UnityEngine;
using System.Collections;

//This bundles the players stats. Fairly self evident. Note that it is a monobehavior.
public class PlayerStats : MonoBehaviour
{
	public float speed = 500;
	public float turnSpeed = 40;
	public float jump = 500 * 25;
	public int score = 0;
	public float health = 100;
	public bool FiredSinceMouseDown = false;
}
