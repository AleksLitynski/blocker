using UnityEngine;
using System.Collections;

/* Generic physics stats we added to every object. 
 * Our gravity isn't the same for every object, so using unitys built in gravity is out of the question
 * 
 */
public class ObjectStats : MonoBehaviour
{
	public Vector3 grav = new Vector3(0, -9.81f, 0);
	
	//Characters/objects will straighten themselves by this much every frame.
	//This allows them to look upright no matter which sureface they are on.
	public float maxGravRoll = 10f;
	public Vector3 unitOppGrav = new Vector3(0, 9.81f, 0);
	//public Vector3 normalOfLastHit = new Vector3(0,0,0);
}
