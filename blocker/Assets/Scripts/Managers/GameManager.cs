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
	public enum NextNodeRule {RandomNode, Ascending, Descending};
	public NextNodeRule nextNodeRule = NextNodeRule.Ascending;
	
	public enum GameType {Race, KingOfTheHill};
	public GameType gameType;
	
	// game variables
	public int index;
	public int maxIndex;
	public int scoreToWin;
	public int numWinners;				
	public int currWinners;
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
			foreach (GameObject zone in GameObject.FindGameObjectsWithTag("Checkpoint"))
			{
				Zone zoneVals = zone.GetComponent<Zone>();
				
				if (zoneVals.hitby != null && zoneVals.hitby != "")
				{
					// check if this is the next checkpoint. if so, advance the checkpoint
					// toward its maxPoints (default starting at 0 going to 1), give player points,
					// and advance the index to the next checkpoint.
					if (zoneVals.orderInRace == index)
					{
						if (zoneVals.currentPoints < zoneVals.maxPoints && menuManager.gameState == MenuManager.GameState.Game)
						{
							zoneVals.currentPoints++;
							// find the player, get their netplayer component, and give em some points
							networkView.RPC ("givePoints", RPCMode.All, zone.name, zoneVals.hitby);
							//GameObject.Find(raceCheckpoint.hitby).GetComponent<NetPlayer>().playerStats.score += raceCheckpoint.scoreReward;
						}
						else //if (raceCheckpoint.currentPoints == raceCheckpoint.maxPoints)
						{
							// reset currentPoints and advance the index
							zoneVals.currentPoints = 0;
							advanceIndex();
							
							// tell everyone about the new checkpoint.
							networkView.RPC ("setCheckpoint", RPCMode.All, index);
							
							// reset the hit detection
							zoneVals.hitby = null;
						}
					}
					else
					{
						// if you are not the index,
						// fuck you,
						// goodbye,
						zoneVals.hitby = null;
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
					if (player.playerStats.score >= scoreToWin && menuManager.gameState == MenuManager.GameState.Game)
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
		// initialize indices
		index = 0;
		maxIndex = 0;
		elapsedTime = 0;
		maxTime = 60;
		int nameint = 0;
		// create a temp array to hold all the checkpoint objects
		foreach(GameObject zone in GameObject.FindGameObjectsWithTag("Checkpoint"))
		{
			(zone.GetComponent("Halo") as Behaviour).enabled = false;
			zone.transform.FindChild("Sphere").GetComponent<MeshRenderer>().enabled = false;
			
			if (zone.GetComponent<Zone>().orderInRace > maxIndex) 
				maxIndex = zone.GetComponent<Zone>().orderInRace;
			if (zone.GetComponent<Zone>().orderInRace < index) 
				index = zone.GetComponent<Zone>().orderInRace;
			zone.name = zone.name + nameint;
			nameint++;
		}
		
		switch(sameOrderRule)
		{
		case SameOrderRule.AllowSameValue:
			// do something here? ? idk
			break;
		case SameOrderRule.UseUniqueValue:
			// loop through scripts to find same order values
			foreach (GameObject zone1 in GameObject.FindGameObjectsWithTag("Checkpoint"))
			{
				foreach (GameObject zone2 in GameObject.FindGameObjectsWithTag("Checkpoint"))
				{
					// if a matching value is found, change a value and 
					// reset the search
					if (zone1.GetComponent<Zone>().orderInRace == zone2.GetComponent<Zone>().orderInRace)
					{
						maxIndex++;
						zone2.GetComponent<Zone>().orderInRace = maxIndex;
					}
				}
			}
			break;
		}
		
		index = maxIndex;
		advanceIndex();
		
		if(!gameObject.GetComponent<NetworkView>())
		{
			gameObject.AddComponent<NetworkView>();
			gameObject.GetComponent<NetworkView>().stateSynchronization = NetworkStateSynchronization.Off;
		}
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			networkView.RPC ("setCheckpoint", RPCMode.All, index);
		}
		
		unpack = true;
	}
	
	void advanceIndex()
	{
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			networkView.RPC ("changeHalo",RPCMode.All,index, false);
		}
		
		switch(nextNodeRule)
		{
		case NextNodeRule.Ascending:
			// easy case
			if (index == maxIndex)
			{
				// find  the least index among checkpoints.
				foreach (GameObject zone in GameObject.FindGameObjectsWithTag("Checkpoint"))
				{
					if (zone.GetComponent<Zone>().orderInRace <= index) index = zone.GetComponent<Zone>().orderInRace;
				}
				if(Network.peerType != NetworkPeerType.Disconnected)
				{
					networkView.RPC ("changeHalo", RPCMode.All, index, true);
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
				
				foreach (GameObject zone in GameObject.FindGameObjectsWithTag("Checkpoint"))
				{
					// no lazy. gotta make a damn variable to break from the while loop #wtf
					//if (temp[i].orderInRace == index) break;
					if (zone.GetComponent<Zone>().orderInRace == index) whilebreak = true;
				}
				
				if (whilebreak) break;
			}
			break;
		case NextNodeRule.RandomNode:
			// get the index from a random checkpoint in the map.
			index = GameObject.FindGameObjectsWithTag("Checkpoint")[Random.Range (0,GameObject.FindGameObjectsWithTag("Checkpoint").Length)].GetComponent<Zone>().orderInRace;
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
	
	[RPC]
	void setCheckpoint(int newIndex)
	{
		index = newIndex;
	}
	
	// made for late join 
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
	void givePoints(string zoneName, string hitby)
	{
		// find the mofo that hit this dude and give that sucka some dough
		
		GameObject playerToFind = GameObject.Find(hitby + "/Doll");
		
		if(playerToFind != null)
		{
			playerToFind.GetComponent<PlayerStats>().score += GameObject.Find(zoneName).GetComponent<Zone>().scoreReward;
			
			// add this guy to the hall of fame.
			if (playerToFind.GetComponent<PlayerStats>().score > scoreToWin) currWinners++;
		}
	}
	
	// i is the index of the checkpoint to light
	[RPC]
	void changeHalo(int ind, bool tf)
	{
		foreach(GameObject zone in GameObject.FindGameObjectsWithTag("Checkpoint"))
		{
			if(zone.GetComponent<Zone>().orderInRace == ind)
			{
				zone.GetComponent<Zone>().toggleHalo(tf);
				zone.transform.FindChild("Sphere").GetComponent<MeshRenderer>().enabled = tf;
			}
		}
	}
}
