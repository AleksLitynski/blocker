var playerStats;
var playerArms;
var map;

var normalOfLastHit = new Vector3(0,0,0);
var unitOppGrav:Vector3 = new Vector3(0,1,0);

function Start()
{
	playerStats = GetComponent("PlayerStats");
	playerArms =  transform.FindChild("Arms");
	map = gameObject.Find("Map");
}

function Update()
{
	if(gameObject.name == "player " + Network.player.ToString())
	{
		if(playerStats != null)
		{
			if(playerStats.grav != Vector3.zero)
			{
				unitOppGrav = -1 * playerStats.grav.normalized;
		    }
		}
		
		if(Physics.Raycast(transform.position, -transform.up, GetComponent.<Collider>().bounds.size.y + GetComponent.<Collider>().bounds.size.y/5, LayerMask.NameToLayer("Player")))
		{
			playerStats.isGrounded = true;
		}
		else
		{
			playerStats.isGrounded = false;
		}
	}
}

function OnNetworkInstantiate (info : NetworkMessageInfo) 
{
	playerStats = GetComponent("PlayerStats");
	playerArms =  transform.FindChild("Arms");
	map = gameObject.Find("Map");
}

function calcRotation(x:float, y:float)
{
	if(playerArms != null)
	{
		var armRot:float = playerArms.localRotation.eulerAngles.x - x;
		if(!((armRot <= 60 && armRot >= -5) || (armRot >= 270 && armRot <= 365)))
		{
			x = 0.0;
		}
	}
	
	var rotation:Quaternion = Quaternion.LookRotation(Vector3.Cross(transform.right, unitOppGrav), unitOppGrav);
	rotation = Quaternion.RotateTowards(transform.rotation, rotation, playerStats.maxGravRoll);
	rotation = rotation * Quaternion.Euler(0,y,0);
	
	try{
		playerArms.Rotate(new Vector3(-x,0,0));
	}catch(err){}

	transform.rotation = rotation;
}

function calcMotion(inputMotion:Vector3, jump:boolean)
{
	var playerMotion:Vector3 = transform.TransformDirection(inputMotion) * playerStats.speed;//inial player speed
	
	var controller : CharacterController = GetComponent(CharacterController);
	if(controller.collisionFlags != CollisionFlags.None || playerStats.isGrounded)
	{
		playerStats.velo *= playerStats.fric; //apply friction if on surface
		if(jump)
		{
			playerStats.velo.x = playerStats.jump * unitOppGrav.x;
			playerStats.velo.y = playerStats.jump * unitOppGrav.y;
			playerStats.velo.z = playerStats.jump * unitOppGrav.z;
		}
		
		var toRotateLine = Quaternion.FromToRotation(unitOppGrav, normalOfLastHit);
		playerMotion = toRotateLine * playerMotion;
	}
	else
	{
		playerStats.velo += playerStats.grav; //dont apply gravity when on the ground. Perfect normal force
	}
	
	playerStats.velo += playerMotion; //apply movment relative to model
	
    GetComponent(CharacterController).Move(playerStats.velo * Time.deltaTime);
}



//grav += (G((playerMass*mass)/r^2))/playerMass
function OnControllerColliderHit(hit:ControllerColliderHit)
{
	normalOfLastHit = hit.normal;
	hit.gameObject.SendMessage("OnControllerColliderHit", hit, SendMessageOptions.DontRequireReceiver); //dispatch this message to the other object, because it deserves to know.
}

var gravHasBeenZeroed:boolean = false;
function zeroGravityOnce()
{
	if(!gravHasBeenZeroed)
	{
		gravHasBeenZeroed = true;
		playerStats.grav = Vector3.zero;
	}
}

function FixedUpdate()
{
	gravHasBeenZeroed = false;
}


