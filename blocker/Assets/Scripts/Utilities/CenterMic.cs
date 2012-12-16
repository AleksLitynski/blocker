using UnityEngine;
using System.Collections;

public class CenterMic : BlockerObject 
{
	// Update is called once per frame
	void Update () 
    {
	    transform.position = Vector3.zero;
        if (playerManager.players.Count > 0)
        {
            for (int i = 0; i < playerManager.players.Count; i++)
            {
                transform.position += playerManager.players[i].transform.position;
            }
            transform.position = transform.position / playerManager.players.Count;
        }
	}
}
