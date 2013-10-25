using UnityEngine;
using System.Collections;

/* When in split screen mode, the microphone must be moved to the average location of all players.
 * 
 * You can't have multiple mics, becasue putting one mic in the middle has the same effect. 
 * 
 * I find that hard to wrap my head around, but it works.
 * 
 * 
 */
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
