//@script RequireComponent(Rigidbody) dont need to require it, as I add it by hand when this thing starts


var pointGravity:boolean = true; //as opposed to, adds to gravity
var plateGravity:boolean = true; //as opposed to, adds to gravity
var overridesGravity:boolean = false;
var mass:float = 300;
var bigG:float = 0.001;

static var ignoreRest:boolean = false;

function Awake()
{
	if(!gameObject.GetComponent(Rigidbody)){gameObject.AddComponent(Rigidbody);}
	var rigidBody = gameObject.GetComponent(Rigidbody);
	rigidBody.constraints = RigidbodyConstraints.FreezeAll;
	rigidBody.useGravity = false;
	rigidBody.mass = mass;
	rigidBody.drag = 0;
	rigidBody.angularDrag = 0;
	rigidBody.isKinematic = true;
	
	Physics.gravity = Vector3.zero;
	if(collider)
	{
		plateGravity = !collider.isTrigger; //if the player can walk into it, its a point gravity
		pointGravity = !plateGravity;
	}
}


function OnControllerColliderHit(hit:ControllerColliderHit)
{
	collisionDispatch(hit.controller.gameObject, hit.normal);
}

function OnTriggerStay (other : Collider) 
{
	collisionDispatch(other.gameObject, Vector3.zero);
}

function collisionDispatch(other: GameObject, hitNormal:Vector3)
{
	if(other.tag == "Player" && other.name == "player " + Network.player.ToString())
	{
		if(pointGravity)
		{
			pointHit(other);
		}
		if(plateGravity)
		{
			surfaceHit(other, hitNormal);
		}
	}
}

function pointHit(player: GameObject)
{	
	var playerStats = player.GetComponent("PlayerStats");
	if(playerStats != null)
	{
		var top = mass * playerStats.mass;
		var bottom = Mathf.Sqrt(Vector3.Distance(transform.position, player.transform.position));
		var magnitude = bigG * playerStats.mass * (top/bottom);
		
		var direction = (player.transform.position - transform.position).normalized;
		
		if(!ignoreRest)
		{
			if(overridesGravity)
			{
				playerStats.grav = -magnitude * direction;
				ignoreRest = true;
			}
			else
			{
				player.GetComponent("PlayerMove").zeroGravityOnce();
				playerStats.grav += -magnitude * direction;
			}
		}
		
	}
}

function surfaceHit(player: GameObject, surfaceNormal:Vector3)
{
	if(!ignoreRest)
	{
		if(overridesGravity)
		{
			player.GetComponent("PlayerStats").grav = -(surfaceNormal * bigG * mass);
			ignoreRest = true;
		}
		else
		{
			player.GetComponent("PlayerMove").zeroGravityOnce();
			player.GetComponent("PlayerStats").grav += -(surfaceNormal * bigG * mass);
		}
	}
}
function FixedUpdate()
{
	shouldClearIgnore = true;
}
var shouldClearIgnore:boolean = false;
function LateUpdate()
{
	if(shouldClearIgnore)
	{
		ignoreRest = false;
		shouldClearIgnore = false;
	}
}