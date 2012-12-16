var windowSize:Rect;
function Start()
{
	ResolutionChanged(Screen.width, Screen.height);
}

var mouseLock = false;
private var newH;
private var newW;
private var wasPaused;
private var firstPerson = false;
function setScreenSize(width:int, height:int)
{
	if(width >= 300 && height >= 300)
	{
		Application.ExternalEval("document.getElementById('unityPlayer').style.height = '"+height+"px';	document.getElementById('unityPlayer').style.width = '"+width+"px';");

		Screen.SetResolution(width, height, Screen.fullScreen);
		
		for (var script : MonoBehaviour in gameObject.GetComponents(MonoBehaviour))
		{
			try
			{
				script.ResolutionChanged(width, height);
			} catch(e) {}
		}
	}
}

var face;
function toggleFirstPerson()
{
	firstPerson = !firstPerson;
	var playerName = "player " + Network.player.ToString();
	Debug.Log(playerName);
	if(firstPerson)
	{
		gameObject.Find(playerName + "/Arms/PlayerCamera").GetComponent("CameraStarter").FocusCameraOnObject(playerName + "/Arms", new Vector3(0.0, 0.3, 0.0), 0.0);
		face = gameObject.Find(playerName + "/Face");
		face.active = false;
		
		//gameObject.Find(playerName+"/Arms").transform.localScale = gameObject.Find(playerName+"/Arms").transform.localScale/2.0;
		
	}
	else
	{
		gameObject.Find(playerName + "/Arms/PlayerCamera").GetComponent("CameraStarter").FocusCameraOnObject(playerName + "/Arms", new Vector3(-1.5, 1.8, -3.0), 10.0);
		face.active = true;
		
		//gameObject.Find(playerName+"/Arms").transform.localScale = gameObject.Find(playerName+"/Arms").transform.localScale*2.0;
	}
}

function toggleFullscreen()
{
	Screen.fullscreen = !Screen.fullscreen;
}

function Update()
{
	if(gameObject.GetComponent("PauseGame").paused && !wasPaused)
	{
		newH = Screen.height;
		newW = Screen.width;
	}
	wasPaused = gameObject.GetComponent("PauseGame").paused;
}

function OnGUI()
{
	if(gameObject.GetComponent("PauseGame").paused)
	{
		GUI.skin = GetComponent("GUIProperties").mySkin;
		GUI.BeginGroup(windowSize);
			GUI.DrawTexture(new Rect(0,0, windowSize.width, windowSize.height), GetComponent("GUIProperties").transBox);
			if(GUI.Button(new Rect(0, 0, windowSize.width, 20), "GAME OPTIONS"))
			{
				toggleSize();
			}
			mouseLock = GUI.Toggle (Rect (10,30,100,20), mouseLock, "MouseLock");
			
			newW = parseInt(GUI.TextField(new Rect(10, 70, windowSize.width*3/5 - 20, 20), newW + ""));
			newH = parseInt(GUI.TextField(new Rect(10, 100, windowSize.width*3/5 - 20, 20), newH +""));
			if(GUI.Button(new Rect(windowSize.width*3/5, 70, windowSize.width*2/5 - 10, 50), "Set Size"))
			{
				setScreenSize(newW, newH);
			}
			if(GUI.Button(new Rect(10, 130, windowSize.width-20, 50), "Toggle First Person"))
			{
				toggleFirstPerson();
			}
		GUI.EndGroup();
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
	if(windowSize != null && prevHeight > windowSize.height)
	{
		windowSize = new Rect(width/borderScale,
			height/borderScale/5,
			width/2 - width/(borderScale/2), 
			20);
		prevHeight = height - height/(borderScale/2);
	}
	else
	{
		windowSize = new Rect(width/borderScale,
			height/borderScale/5,
			width/2 - width/(borderScale/2), 
			height - height/(borderScale/2));
		prevHeight = 20;
	}
}
