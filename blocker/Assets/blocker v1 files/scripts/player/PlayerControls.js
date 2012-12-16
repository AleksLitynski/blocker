var pausedGame;
var playerCamera;
var playerStats;
var playerMove;

function Start()
{
	if(gameObject.name == "player " + Network.player.ToString())
	{
		pausedGame = GameObject.Find("World").GetComponent("PauseGame");
		playerCamera = transform.FindChild("PlayerCamera");
		playerStats = GetComponent("PlayerStats");
		playerMove = GetComponent("PlayerMove");
	}
}

function OnNetworkInstantiate (info : NetworkMessageInfo) 
{
	if(gameObject.name == "player " + Network.player.ToString())
	{
		pausedGame = GameObject.Find("World").GetComponent("PauseGame");
		playerCamera = transform.FindChild("PlayerCamera");
	}
		
}

function Update ()
{
	//don't try to update players who aren't yours to control. Also, don't do jack shit if paused (including falling)
	if(gameObject.name == "player " + Network.player.ToString())
	{
		//fire gun
		if(playerStats.curGun != null)
		{
			var key = "";
			if(Input.GetAxis("Fire1"))
			{
				key = "left";
			}
			if(Input.GetAxis("Fire2"))
			{
				key = "right";
			}
			if(key != "")
			{
				playerStats.curGun.SendMessage("Fire", key);
			}
			
		}
		//change gun
		for(var i = 0; i < 9; i++)
		{
			var j = i+1;
			if(j == 10) {j = 0;}
			if(Input.GetKeyDown(j+""))
			{
				networkView.RPC("setGun", RPCMode.AllBuffered, i);
			}
		}
	
	
	
		//player motion
		InputMotion = new Vector3(Input.GetAxis("Straff"), 0, Input.GetAxis("Forward"));
		Jump = Input.GetAxis("Jump")==1;
		
		//look up and down
		xRot = 0 + (Input.GetAxis("Turn Y"))*playerStats.turnSpeed/10;
		yRot = 0 + (Input.GetAxis("Turn X"))*playerStats.turnSpeed/10;
		
	}
}

var InputMotion:Vector3 = new Vector3(0,0,0);
var Jump:boolean = false;
var xRot:float = 0.0;
var yRot:float = 0.0;
function clearInputs()
{
	InputMotion = new Vector3(0,0,0);
	Jump = false;
	xRot = 0.0;
	yRot = 0.0;
}

function FixedUpdate()
{
	if(gameObject.name == "player " + Network.player.ToString())
	{
		playerMove.calcMotion(InputMotion, Jump);
		playerMove.calcRotation(xRot, yRot);
		clearInputs();
	}
}
