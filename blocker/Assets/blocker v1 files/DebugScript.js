var playerStats;

function Awake()
{
	playerStats = GetComponent("PlayerStats");
}

function OnNetworkInstantiate (info : NetworkMessageInfo) 
{
	playerStats = GetComponent("PlayerStats");
}

//lets you control gravity with sliders. For debugging, basically
function OnGUI()
{
	if(gameObject.name == "player " + Network.player.ToString())
	{
		GUI.Label(new Rect(450,10, 60,30), "X: " + Mathf.Round(playerStats.grav.x));
		playerStats.grav.x = GUI.HorizontalScrollbar (new Rect(500,10, 300,30), playerStats.grav.x, 1, -10, 10);
		if(GUI.Button(new Rect(810,10,30,30),"0")) { playerStats.grav.x = 0; }
		
		GUI.Label(new Rect(450,50, 60,30), "Y: " + Mathf.Round(playerStats.grav.y));
		playerStats.grav.y = GUI.HorizontalScrollbar (new Rect(500,50, 300,30), playerStats.grav.y, 1, -10, 10);
		if(GUI.Button(new Rect(810,50,30,30),"0")) { playerStats.grav.y = 0; }
		
		GUI.Label(new Rect(450,90, 60,30), "Z: " + Mathf.Round(playerStats.grav.z));
		playerStats.grav.z = GUI.HorizontalScrollbar (new Rect(500,90, 300,30), playerStats.grav.z, 1, -10, 10);
		if(GUI.Button(new Rect(810,90,30,30),"0")) { playerStats.grav.z = 0; }
	}
}

