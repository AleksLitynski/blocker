var windowSize:Rect;


function Start()
{
	ResolutionChanged(Screen.width, Screen.height);
}

function OnGUI()
{
	if(gameObject.GetComponent("PauseGame").paused)
	{
		GUI.skin = GetComponent("GUIProperties").mySkin;
		GUI.BeginGroup(windowSize);
			GUI.DrawTexture(new Rect(0,0, windowSize.width, windowSize.height), GetComponent("GUIProperties").transBox);
			
			printPlayers();
			
		GUI.EndGroup();
	}
}

function printPlayers()
{
	//print all players
	GUI.DrawTexture (new Rect(0,0, Screen.width, 20), GetComponent("GUIProperties").transBox);
	if(GUI.Button(new Rect(0, 0, windowSize.width, 20), "GAME STATS"))
	{
		toggleSize();
	}
	var offset = 30;
	var players = GameObject.FindGameObjectsWithTag("Player");
	
	GUI.Label(new Rect(10, offset, windowSize.width/3, 20), "| Name");
	GUI.Label(new Rect(10 + windowSize.width*1/3, offset, windowSize.width/3, 20), "| WEAPON");
	GUI.Label(new Rect(10 + windowSize.width*2/3, offset, windowSize.width/3, 20), "| SCORE");
	offset += 20;
	GUI.Label(new Rect(10, offset, windowSize.width - 20, 20), "------------------------------------------------------------------");
	offset += 20;
	for(var player in players)
	{
		try{
			var gunName = "None";
			
			if(player.GetComponent("PlayerStats").curGun != null)
				gunName = player.GetComponent("PlayerStats").curGun.name;
			
			var score = player.GetComponent("PlayerStats").score;
				
			GUI.Label(new Rect(10, offset, windowSize.width/3, 20), "| " + player.name);
			GUI.Label(new Rect(10 + windowSize.width*1/3, offset, windowSize.width/3, 20), "| " + gunName);
			GUI.Label(new Rect(10 + windowSize.width*2/3, offset, windowSize.width/3, 20), "| " + score);
			offset+= 25;
		}catch(err){}
	}
}

private var prevHeight:float;
function toggleSize()
{
	var temp = prevHeight;
	prevHeight = windowSize.height;
	windowSize.height = temp;
}


function ResolutionChanged(width:int, height:int)
{
	var borderScale = 10;
	if(windowSize != null && prevHeight < windowSize.height)
	{
		windowSize = new Rect(width/2 + width/borderScale,
			height/borderScale/5,
			width/2 - width/(borderScale/2), 
			height - height/(borderScale/2));
		prevHeight = 20;
	}
	else
	{
		windowSize = new Rect(width/2 + width/borderScale,
			height/borderScale/5,
			width/2 - width/(borderScale/2), 
			20);
		prevHeight = height - height/(borderScale/2);
	}
}