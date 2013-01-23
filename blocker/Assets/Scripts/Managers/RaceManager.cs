using UnityEngine;
using System.Collections;

public class RaceManager: BlockerObject 
{
	// enums
	public enum SameOrderRule {AllowSameValue, UseUniqueValue};
	public SameOrderRule sameOrderRule = SameOrderRule.UseUniqueValue;
	
	// game variables
	public GameObject[] checkpoints;
	public int index;
	public int maxIndex;
	public GameObject indicator;
	
	// utility variables
	bool unpack = false;
	NetworkView networkView;
	
	// Use this for initialization
	void Start () 
	{
		init();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!unpack) init();
		
		if (Network.peerType == NetworkPeerType.Server)
		{
			// handle a player colliding with a checkpoint
			for (int i = 0; i < checkpoints.Length; i++)
			{
				RaceCheckpoint raceCheckpoint = checkpoints[i].GetComponent<RaceCheckpoint>();
				
				// put the indicator over the current checkpoint
				if (raceCheckpoint.orderInRace == index)
				{
					indicator.transform.position = new Vector3(checkpoints[i].transform.position.x, indicator.transform.position.y, checkpoints[i].transform.position.z);
				}
				
				if (raceCheckpoint.hitby != null)
				{
					// check if this is the next checkpoint. if so, advance the checkpoint
					// toward its maxPoints (default starting at 0 going to 1), give player points,
					// and advance the index to the next checkpoint.
					if (raceCheckpoint.orderInRace == index)
					{
						if (raceCheckpoint.currentPoints < raceCheckpoint.maxPoints)
						{
							raceCheckpoint.currentPoints++;
							// find the player, get their netplayer component, and give em some points
							GameObject.Find(raceCheckpoint.hitby).GetComponent<NetPlayer>().playerStats.score += raceCheckpoint.scoreReward;
							Debug.Log(GameObject.Find(raceCheckpoint.hitby).GetComponent<NetPlayer>().playerStats.score);
						}
						else //if (raceCheckpoint.currentPoints == raceCheckpoint.maxPoints)
						{
							// reset currentPoints and advance the index
							raceCheckpoint.currentPoints = 0;
							advanceIndex();
							
							// tell everyone about the new checkpoint.
							networkView.RPC ("setCheckpoint", RPCMode.Others, index);
							
							// reset the hit detection
							raceCheckpoint.hitby = null;
						}
					}
				}
			}
		}
	}
	
	void init()
	{
		// create a temp array to hold all the checkpoint objects
		try
		{
			checkpoints = GameObject.FindGameObjectsWithTag("RaceCheckpoint");
		}
		finally {}
		RaceCheckpoint[] temp = new RaceCheckpoint[checkpoints.Length];
		
		// loop through checkpoints and grab their scripts
		for(int i = 0; i < temp.Length; i++)
		{
			temp[i] = (RaceCheckpoint)checkpoints[i].GetComponent("RaceCheckpoint");
		}
		
		switch(sameOrderRule)
		{
		case SameOrderRule.AllowSameValue:
			// do something here? ? idk
			break;
		case SameOrderRule.UseUniqueValue:
			// loop through scripts to find same order values
			for (int i = 0; i < temp.Length - 1; i++)
			{
				for (int j = i + 1; j < temp.Length; j++)
				{
					// if a matching value is found, change a value and 
					// reset the search
					if (temp[i].orderInRace == temp[j].orderInRace)
					{
						temp[j].orderInRace++;
						i = 0;
					}
				}
			}
			break;
		}
		
		// initialize indices
		index = 0;
		maxIndex = 0;
		
		for (int i = 0; i < temp.Length; i++)
		{
			// find the greatest order in race.
			if (temp[i].orderInRace > maxIndex) maxIndex = temp[i].orderInRace;
			if (temp[i].orderInRace < index) index = temp[i].orderInRace;
		}
		
		networkView = this.gameObject.GetComponent<NetworkView>();
		
		unpack = true;	
		
	}
	
	void advanceIndex()
	{
		RaceCheckpoint[] temp = new RaceCheckpoint[checkpoints.Length];
		
		// loop through checkpoints and grab their scripts
		for(int i = 0; i < temp.Length; i++)
		{
			temp[i] = checkpoints[i].GetComponent<RaceCheckpoint>();
		}
		// easy case
		if (index == maxIndex)
		{
			// find  the least index among checkpoints.
			for (int i = 0; i < temp.Length; i++)
			{
				if (temp[i].orderInRace < index) index = temp[i].orderInRace;
			}
			return;
		}
		
		// ugh
		bool whilebreak = false;
		
		// increase index
		while (true)
		{
			// increment that shit til its something
			index++;
			
			for (int i = 0; i < temp.Length; i++)
			{
				// no lazy. gotta make a damn variable to break from the while loop #wtf
				//if (temp[i].orderInRace == index) break;
				if (temp[i].orderInRace == index) whilebreak = true;
			}
			if (whilebreak) break;
		}
	}
	
	[RPC]
	void setCheckpoint(int newIndex)
	{
		index = newIndex;
	}
}
