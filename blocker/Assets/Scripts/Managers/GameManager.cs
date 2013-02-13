using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager: BlockerObject 
{
	// enums
	// this rule determines if there are "optional paths" in the course.
	// if you use "useuniquevalue", racemanager will create an system of
	// ordered nodes for you. you wont have to change the order in the
	// inspector.
	public enum SameOrderRule {AllowSameValue, UseUniqueValue};
	public SameOrderRule sameOrderRule = SameOrderRule.UseUniqueValue;
	
	// this rule determines which player wins the game: by having the
	// most points by the end of a time limit, the most points by the
	// time someone finishes the race, or being the first to finish the
	// race.
	public enum WinRule {MostPointsOverTime, MostPointsByFinish, FirstToFinish};
	public WinRule winRule = WinRule.MostPointsByFinish;
	
	// this rule determines how the manager determines which node is 
	// next when a node is "completed" (its maxpoints have been
	// reached): choosing a random node, or choosing a node next by the
	// order.
	public enum NextNodeRule {RandomNode, OrderedNodes};
	public NextNodeRule nextNodeRule = NextNodeRule.OrderedNodes;
	
	// game variables
	public List<GameObject> checkpoints;
	public int index;
	public int maxIndex;
	public int scoreToWin;
	public int numWinners;				// make an overarching manager for this
	public int currWinners;
	public bool matchOver;
	
	// utility variables
	bool unpack = false;
	public bool initState = false;				// if false, uninitialize. otherwise, initialize.
	
	// Use this for initialization
	public override void Start () 
	{
		base.Start();
		init();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!unpack) init();
		
		if (Network.peerType == NetworkPeerType.Server)
		{
			// handle a player colliding with a checkpoint
			for (int i = 0; i < checkpoints.Count; i++)
			{
				RaceCheckpoint raceCheckpoint = checkpoints[i].GetComponent<RaceCheckpoint>();
				
				
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
							networkView.RPC ("givePoints", RPCMode.All, i, raceCheckpoint.hitby);
							//GameObject.Find(raceCheckpoint.hitby).GetComponent<NetPlayer>().playerStats.score += raceCheckpoint.scoreReward;
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
					else
					{
						raceCheckpoint.hitby = null;
					}
				}
			}
			
			foreach(NetPlayer player in playerManager.players)
			{
				if (player.playerStats.score >= scoreToWin)
				{
					networkView.RPC ("ChangeState", RPCMode.All, MenuManager.LobbyCode);	
				}
			}
		}
	}
	
	void OnGUI()
	{
		if (currWinners >= numWinners)
		{
			//GUI.
		}
	}
	
	public void init()
	{
		// create a temp array to hold all the checkpoint objects
		checkpoints.Clear();
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("RaceCheckpoint"))
		{
			if(obj.transform.parent == mapManager.loadedMap.transform)
			{
				checkpoints.Add(obj);
			}
		}
		
		
		
		RaceCheckpoint[] temp = new RaceCheckpoint[checkpoints.Count];
		
		foreach(GameObject point in checkpoints)
		{
			(point.GetComponent("Halo") as Behaviour).enabled = false;
		}
		
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
		
		matchOver = false;
		
		unpack = true;
		
		changeHalo(index, true);
	}
	
	void advanceIndex()
	{
		networkView.RPC ("changeHalo",RPCMode.All,index, false);
		RaceCheckpoint[] temp = new RaceCheckpoint[checkpoints.Count];
		
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
			networkView.RPC ("changeHalo", RPCMode.All, index, true);
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
		networkView.RPC ("changeHalo", RPCMode.All, index, true);
	}
	
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		networkView.RPC("changeHalo", player, index, true);//actives players first checkpoint
		
		foreach(NetPlayer p in playerManager.players)
		{
			networkView.RPC ("setScore", player, p.gameObject.GetComponent<PlayerStats>().score, p.name);
		}
	}
	
	
	
	[RPC]
	void setCheckpoint(int newIndex)
	{
		index = newIndex;
	}
	
	[RPC]
	void setScore(int score, string playerName)
	{
		GameObject playerToFind = GameObject.Find(playerName);
		
		if(playerToFind != null)
		{
			playerToFind.GetComponent<PlayerStats>().score = score;
		}
	}
	
	// this function uses the index of the loop to give points to the hitby of checkpoints[i]
	[RPC]
	void givePoints(int i, string hitby)
	{
		// get the component script
		RaceCheckpoint rc = checkpoints[i].GetComponent<RaceCheckpoint>();
		
		// find the mofo that hit this dude and give that sucka some dough
		GameObject playerToFind = GameObject.Find(hitby);
		
		if(playerToFind != null)
		{
			playerToFind.GetComponent<PlayerStats>().score += rc.scoreReward;
			
			// add this guy to the hall of fame.
			if (playerToFind.GetComponent<PlayerStats>().score > scoreToWin) currWinners++;
		}
	}
	
	// i is the index of the checkpoint to light
	[RPC]
	void changeHalo(int i, bool tf)
	{
		if(checkpoints.Count < i)
		{
			(checkpoints[i].GetComponent("Halo") as Behaviour).enabled = tf;	
		}
	}
}
