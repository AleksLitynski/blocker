var windowSize:Rect;

var paused = false;
var prevPauseInput;

function Awake()
{
	ResolutionChanged(Screen.width, Screen.height);
	prevPauseInput = Input.GetAxis("Pause");
}

function Update () {

	if((Input.GetAxis("Pause") != prevPauseInput) && Input.GetAxis("Pause") == 1)
	{
		paused = !paused;
	}
	if(paused)
	{
		if(Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
		if(Cursor.visable   != true)                 Cursor.visible = true;
	}
	else
	{
		if(gameObject.GetComponent("NetworkSetup").connected)
		{
			if(gameObject.GetComponent("GameOptions").mouseLock){
				if(Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked;
				if(Cursor.visable   != false)                 Cursor.visible = false;
			} else {
				if(Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
				if(Cursor.visable   != true)                 Cursor.visible = true;
			}
		}
	}
	if(!gameObject.GetComponent("NetworkSetup").connected)
	{
		paused = false;
		
		if(Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
		if(Cursor.visable   != true)                 Cursor.visible = true;
	}
	
	prevPauseInput = Input.GetAxis("Pause");
}

function OnGUI()
{
	GUI.skin = GetComponent("GUIProperties").mySkin;
	GUI.BeginGroup(windowSize);
		if(paused)
		{
			if(GUI.Button(new Rect(0,0,windowSize.width, windowSize.height), "PAUSED"))
			{
				paused = !paused;
			}
		}
	GUI.EndGroup();
}

function ResolutionChanged(width:int, height:int)
{
	windowSize = new Rect(width/2 - 40, height-20,100, 20);
}