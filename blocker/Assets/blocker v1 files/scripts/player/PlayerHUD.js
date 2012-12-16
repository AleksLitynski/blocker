var windowSize:Rect;
private var world;

var crossHair:Texture2D;
var playerStats;
function Start()
{
	ResolutionChanged(Screen.width, Screen.height);
	world = gameObject.Find("World");
	playerStats = GetComponent("PlayerStats");
	GetComponent("PlayerStats").crossHairLoc = new Vector2(Screen.width/2, Screen.height/2);
}

function ResolutionChanged(width:int, height:int)
{
	windowSize = new Rect(0, 0,width, height);
}


function OnGUI()
{
	if(!world.GetComponent("PauseGame").paused && gameObject.name == "player " + Network.player.ToString())
	{
		GUI.BeginGroup(windowSize);
	
			GUI.DrawTexture(new Rect(0,0, windowSize.width, Screen.height/7), world.GetComponent("GUIProperties").transBox);
			
		
			var cLoc = GetComponent("PlayerStats").crossHairLoc;
			var cDepth = playerStats.scopedRange;
			cDepth = 600/cDepth;
			GUI.DrawTexture(new Rect(cLoc.x - cDepth/2,cLoc.y - cDepth/2, cDepth, cDepth), crossHair);
			
			GUI.Label(new Rect(20,20,windowSize.width, windowSize.height), playerStats.ToString());
			
		
		GUI.EndGroup();
	}
}