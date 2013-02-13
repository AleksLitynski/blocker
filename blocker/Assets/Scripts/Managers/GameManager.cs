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
	public enum WinRule {MostPointsOverTime, MostPointsByFinish, FirstToFinish, FirstToLimit};
	public WinRule winRule = WinRule.FirstToLimit;
	
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
	public int numWinners;				
	public int currWinners;
	public bool matchOver;
	public float elapsedTime;
	public float maxTime;
	
	// utility variables
	bool unpack = false;
	public bool initState = false;				// if false, uninitialize. otherwise, initialize.
	public string gameDescription;
	
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
		
		// only elapse time if the Game state is active.
		if (GameObject.Find ("World").GetComponent<MenuManager>().gameState == MenuManager.GameState.Game)
		{
			elapsedTime += Time.deltaTime;
		}
		
		if (Network.peerType == NetworkPeerType.Server)
		{
			// handle a player colliding with a checkpoint
			for (int i = 0; i < checkpoints.Count; i++)
			{
				RaceCheckpoint raceCheckpoint = checkpoints[i].GetComponent<RaceCheckpoint>();
				
				
				if (raceCheckpoint.hitby != null && raceCheckpoint.hitby != "")
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
							Debug.Log (raceCheckpoint.hitby+ " " + i);
							Debug.Log(checkpoints.Count);
							world.networkView.RPC ("givePoints", RPCMode.All, i, raceCheckpoint.hitby);
							//GameObject.Find(raceCheckpoint.hitby).GetComponent<NetPlayer>().playerStats.score += raceCheckpoint.scoreReward;
						}
						else //if (raceCheckpoint.currentPoints == raceCheckpoint.maxPoints)
						{
							// reset currentPoints and advance the index
							raceCheckpoint.currentPoints = 0;
							advanceIndex();
							
							// tell everyone about the new checkpoint.
							networkView.RPC ("setCheckpoint", RPCMode.All, index);
							
							// reset the hit detection
							raceCheckpoint.hitby = null;
						}
					}
					else
					{
						// if you are not the index,
						// fuck you,
						// goodbye,
						raceCheckpoint.hitby = null;
					}
				}
			}
			// this is the winning. you win here.
			switch(winRule)
			{
			// the player with the most points when the final checkpoint is reached.
			case WinRule.MostPointsByFinish:
				Debug.Log ("Warning: This WinRule is unimplemented.");
				break;
			// the player with the most points after a time interval.
			case WinRule.MostPointsOverTime:
				if (elapsedTime >= maxTime)
				{
					world.networkView.RPC ("ChangeState", RPCMode.All, MenuManager.LobbyCode);
				}
				break;
			// the first player to reach the last checkpoint in the race.
			case WinRule.FirstToFinish:
				Debug.Log ("Warning: This WinRule is unimplemented.");
				break;
			// the first player to reach a certain score.
			case WinRule.FirstToLimit:
				foreach(NetPlayer player in playerManager.players)
				{
					if (player.playerStats.score >= scoreToWin)
					{
						world.networkView.RPC ("ChangeState", RPCMode.All, MenuManager.LobbyCode);	
					}
				}
				break;
			}
			
		}
	}
	
	public void init()
	{
		// create a temp array to hold all the checkpoint objects
		checkpoints.Clear();
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("RaceCheckpoint"))
		{
			/*if(obj.transform.parent == GameObject.Find ("World").GetComponent<MapManager>().loadedMap.transform)
			{
				checkpoints.Add(obj);
			}*/
			if (obj.transform.parent == this.gameObject.transform)
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
		
		gameObject.AddComponent<NetworkView>();
		
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
		
		elapsedTime = 0;
		maxTime = 60;
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
		switch(nextNodeRule)
		{
		case NextNodeRule.OrderedNodes:
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
			break;
		case NextNodeRule.RandomNode:
			// get the index from a random checkpoint in the map.
			index = temp[Random.Range (0,temp.Length)].orderInRace;
			break;
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
	
	public void ToggleAllCheckpoints(bool tf)
	{
		foreach(GameObject rc in checkpoints)
		{
			rc.GetComponent<RaceCheckpoint>().AlterLifeState(tf);
		}
	}
	
	[RPC]
	void setCheckpoint(int newIndex)
	{
		index = newIndex;
		foreach(GameObject rc in checkpoints)
		{
			rc.GetComponent<RaceCheckpoint>().awake = false;
		}
		Debug.Log(checkpoints[index].GetComponent<RaceCheckpoint>().awake);
		checkpoints[index].GetComponent<RaceCheckpoint>().awake = true;
		Debug.Log(checkpoints[index].GetComponent<RaceCheckpoint>().awake);
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
		/*if(checkpoints.Count < i)
		{
			(checkpoints[i].GetComponent("Halo") as Behaviour).enabled = tf;	
		}*/
		Debug.Log(checkpoints.Count + "in");
		(checkpoints[i].GetComponent("Halo") as Behaviour).enabled = tf;
		checkpoints[i].active = tf;
	}
}
