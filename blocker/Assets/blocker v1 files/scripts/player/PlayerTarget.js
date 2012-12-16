var playerStats;
var hand;
var playerCamera;
var cameraComp;

function Start()
{
	playerStats = GetComponent("PlayerStats");
	hand = transform.FindChild("Arms/Hand");
	playerCamera = gameObject.Find("PlayerCamera").transform;
	cameraComp = gameObject.Find("camera").GetComponent("Camera");
	
	updateTarget();
}

private var scopeIn = true;
function Update()
{
	if(scopeIn)
	{
		playerStats.scopedRange = playerStats.scopedRange+0.75;
		if(playerStats.scopedRange >= 75)
		{
			scopeIn = false;
		}
	}
	else 
	{
		playerStats.scopedRange = playerStats.scopedRange-1;
		if(playerStats.scopedRange <= 5)
		{
			scopeIn = true;
		}
	}
	
	
	if(gameObject.name == "player " + Network.player.ToString())
	{
		updateTarget();
		var loc = cameraComp.WorldToScreenPoint(new Ray(playerStats.target.position, playerStats.target.forward).GetPoint(playerStats.scopedRange));
		playerStats.crossHairLoc.x = loc.x;
		playerStats.crossHairLoc.y = Screen.height - loc.y;
	}
	var target = playerStats.target;
	
	
	/*Debug.DrawRay( target.position, 
						target.forward * 10, 
						Color.red, 
						0, 
						false);*/
						
	
}

function updateTarget()
{
	playerStats.target.position = hand.position;
	playerStats.target.forward = hand.forward;
	
	
}