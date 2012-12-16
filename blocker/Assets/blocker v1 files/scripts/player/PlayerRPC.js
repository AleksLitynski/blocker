var playerStats;

function Awake()
{
	playerStats = GetComponent("PlayerStats");
}

function OnNetworkInstantiate (info : NetworkMessageInfo) 
{
	playerStats = GetComponent("PlayerStats");
}

@RPC
function setGun(wepNumber:int)
{	
	if(wepNumber >= 0 && wepNumber < playerStats.guns.Length)
	{
		if(playerStats.curGun != null)
		{
			Destroy(playerStats.curGun);
		}
		var hand = transform.FindChild("Arms/Hand").transform;
		playerStats.curGun = Instantiate (playerStats.guns[wepNumber], hand.position, hand.rotation);
		playerStats.curGun.transform.parent = hand;
		playerStats.curGun.name = playerStats.guns[wepNumber].name;
	}
}

@RPC
function addGun(wepNumber:int, toAddPath:String)
{
	if(wepNumber >= 0 && wepNumber < playerStats.maxGuns)
	{
		if(toAddPath == null)
		{
			playerStats.guns[wepNumber] = null;
		}
		playerStats.guns[wepNumber] = Resources.Load(toAddPath);
		
	}
}

@RPC
function takeDamage(damage:float, targetPlayer:String)
{
	GameObject.Find(targetPlayer).GetComponent("PlayerStats").health -= damage;
}

@RPC
function setScore(newScore:int, targetPlayer:String)
{
	GameObject.Find(targetPlayer).GetComponent("PlayerStats").score   = newScore;
}