using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
	public float speed = 500;
	public float turnSpeed = 40;
	public float jump = 500 * 25;
	public int score = 0;
	public float health = 100;
	public bool FiredSinceMouseDown = false;
}
