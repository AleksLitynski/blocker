using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkView))]
public class NetObject : MonoBehaviour {

    public NetObject netObject;
	
	void Start () 
    {
        netObject = this;
	}
}

/*
 * Things that will be synchronized:
 * Position <-
 * Rotation <-
 * Cascaded Values
 * Is Firing Gun
 * Initial Map
 * Create new objects
 */
