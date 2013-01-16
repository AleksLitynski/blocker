using UnityEngine;
using System.Collections;

public class RaceManager: MonoBehaviour 
{
	public enum SameOrderRule {AllowSameValue, UseUniqueValue};
	public SameOrderRule sameOrderRule = SameOrderRule.UseUniqueValue;
	
	public GameObject[] checkpoints;
	
	bool unpack = false;
	
	// Use this for initialization
	void Start () 
	{
		init();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!unpack) init();
	}
	
	void init()
	{
		// create a temp array to hold all the checkpoint objects
		checkpoints = GameObject.FindGameObjectsWithTag("RaceCheckpoint");
		
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
		unpack = true;	
	}
}
