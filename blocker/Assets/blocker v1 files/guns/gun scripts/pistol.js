var playerStats;

function Start()
{
	playerStats = gameObject.Find("player " + Network.player.ToString()).GetComponent("PlayerStats");
}

function Fire(type:String)
{
	if(type == "left")
	{
		target = playerStats.target;
		
		var hit:RaycastHit;
		var hitSomething = Physics.Raycast (target.position, target.forward, hit, 100);
		if(hitSomething)
		{
			var hitObj = hit.transform.gameObject;
			if(hitObj.tag == "Player")
			{
				playerStats.gameObject.networkView.RPC ("takeDamage", RPCMode.All, 1.0, hitObj.name);
				playerStats.gameObject.networkView.RPC ("setScore", RPCMode.All, playerStats.score + 1, playerStats.gameObject.name);
			}
		}
	}
	if(type == "right")
	{
		target = playerStats.target;
	}
}