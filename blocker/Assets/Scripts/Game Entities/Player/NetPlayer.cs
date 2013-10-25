using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
NetPlayer contains most of the code controlling a given player character.
This includes:
player id
movement controls
compass and laser pointer UI overlays
indicators of the current control scheme

*/
[RequireComponent(typeof(PlayerStats))]
public class NetPlayer : NetObject 
{
	//Public accessors for common functionality.
	[HideInInspector]
    public int localPlayerNumber;
	[HideInInspector]
    public NetworkPlayer networkPlayer;
	[HideInInspector]
	public PlayerStats playerStats;
	[HideInInspector]
	public Transform playerArms;
	[HideInInspector]
	public Transform playerArrow;
	[HideInInspector]
	public Transform playerCamera;
	[HideInInspector]
	public Transform player;
	[HideInInspector]
	public Transform playerCompass;
	
	//private toggles for control type
    bool _keyboardPlayer = false;
    bool _mobilePlayer = false;
    int _controllerNumber = -1;
	
	//camera options
	public float lookPitch = 0;
	public bool laserOn = false;
	
	public bool jumpWasUp = true;
	
	GameManager gameManager;
	
	public override void Start()
	{
		base.Start();
	}
	
	//Awake is called after network instantiation
	public void Awake()
	{
		//rig up helpful accessors for parts of the player. 
		//This consolidates findChilds in case the structure of the player is modified
		player = transform.parent;
		playerArms = transform.FindChild("Model/Arms");
		playerCamera = player.FindChild("Camera");
		playerArrow = playerCamera.FindChild("Compass/arrow"); 
		playerCompass = playerCamera.FindChild("Compass");
		playerStats = gameObject.GetComponent<PlayerStats>(); //Note that this was required at the head of the file
		
		
	}
	
	//Creates the compass that points towards the next objective.
	//Each camera has a mask to hide all other players compasses/lasers.
	//Otherwise, there would be a big ball floating over every players sholder. 
	bool compassHasBeenInited = false;
	public void initCompassLayer()
	{
		int newLayer = 21 + localPlayerNumber;
		//set the compass to the local player's level.
		if(Network.player == networkPlayer)//if it's a local player 
		{
			playerCompass.gameObject.layer = newLayer;
			for(int i = 0; i < playerCompass.GetChildCount(); i++) //move the compass, and its child the arrow to the proper layer
			{
				playerCompass.GetChild(i).gameObject.layer = newLayer;
			}
			for(int i = 0; i < playerArrow.GetChildCount(); i++)
			{
				playerArrow.GetChild(i).gameObject.layer = newLayer;
			}
		}
		else
		{
			playerCompass.gameObject.layer = 20;
			for(int i = 0; i < playerCompass.GetChildCount(); i++)
			{
				playerCompass.GetChild(i).gameObject.layer = 20;
			}
			for(int i = 0; i < playerArrow.GetChildCount(); i++)
			{
				playerArrow.GetChild(i).gameObject.layer = 20;
			}
		}
		compassHasBeenInited = true;
		
		float x = (((Screen.width * playerCamera.camera.rect.width)/1.1f) + (Screen.width * playerCamera.camera.rect.x));
		float y = (((Screen.height * playerCamera.camera.rect.height)/9) + (Screen.height * playerCamera.camera.rect.y));
		
		
		Ray ray = playerCamera.camera.ScreenPointToRay(new Vector3(x,y,0));//tweak this to position compass
		playerCompass.position = ray.GetPoint(5);
		
		
	}
	
	void Update()
	{
		
		
		
		if(!compassHasBeenInited) //This is a costly way to check if the compass has been created, but it "got the job done"
		{
			initCompassLayer();
		}
		
		
		if (menuManager.bgMap != null)
		{
			gameManager = menuManager.bgMap.GetComponent<GameManager>();
		}
		
		
		GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
		Vector3 target = Vector3.one * 1000000;
		
		//Go through all checkpoints and find out which, amoung those that are next, is closest. 
		foreach(GameObject checkpoint in checkpoints) 
		{
			if(checkpoint.GetComponent<Zone>().orderInRace == gameManager.index)
			{
				if(Vector3.SqrMagnitude(transform.position - checkpoint.transform.position) < Vector3.SqrMagnitude(transform.position - target))
				{
					target = new Vector3(checkpoint.transform.position.x, checkpoint.transform.position.y, checkpoint.transform.position.z);
				}
			}
		}
		playerArrow.LookAt(target); //Aim the arrow  at the closest one.
		
		//Sets up the culling mask so the player doesn't see the other compasses/lasers
		string toLog = "20, ";
		List<int> toIgnore = new List<int>();
		toIgnore.Add(20);
		foreach(NetPlayer player in playerManager.localPlayers)
		{
			if(player.localPlayerNumber != localPlayerNumber)
			{
				toIgnore.Add(21 + player.localPlayerNumber);	
				toLog += "" + (21 + player.localPlayerNumber) + ", ";	
			}
		}
		//applies the mask
		playerCamera.camera.cullingMask = LayerMaskHelper.EverythingBut(toIgnore.ToArray()); 
		
		drawLaserPointer(laserOn); //draws the laser, if enabled. Really, fairly self documenting
	}
	
	//When activating keyboard/mobile/controller, disable the other control types. The game doesn't actually support mobile (yet)
    public bool KeyboardPlayer
    {
        get
        {
            return _keyboardPlayer;
        }
        set
        {
            _keyboardPlayer = value;
            _mobilePlayer = false;
            _controllerNumber = -1;
			
        }
    }
    public bool MobilePlayer
    {
        get
        {
            return _mobilePlayer;
        }
        set
        {
            _keyboardPlayer = false;
            _mobilePlayer = value;
            _controllerNumber = -1;
        }
    }
    public int ControllerNumber
    {
        get
        {
            return _controllerNumber;
        }
        set
        {
            _keyboardPlayer = false;
            _mobilePlayer = false;
            _controllerNumber = value;
        }
    }

		//Prints: "Local Number: ##. Controlled With ________"
    public override string ToString()
    {
        string toReturn = networkPlayer.ToString();
        toReturn += ". Local Number: " + localPlayerNumber;
        if (KeyboardPlayer)
        {
            toReturn += ". Controlled With keyboard";
        }
        if (MobilePlayer)
        {
            toReturn += ". Controlled With mobile";
        }
        if (ControllerNumber != -1)
        {
            toReturn += ". Controlled With joystick #" + ControllerNumber;
        }
        return toReturn;
    }
	
	//This takes care of moving the player
	public void move(InputCollection col)
	{
		//rotate player arms
		if(playerArms != null)
		{
			float armRot = playerArms.localRotation.eulerAngles.x - col.turnUp;
			if(!((armRot <= 60 && armRot >= -5) || (armRot >= 270 && armRot <= 365)))
			{
				col.turnUp = 0.0f;
			}
			playerArms.Rotate(new Vector3(-col.turnUp,0,0));
		}
		
		Vector3 playerMotion = new Vector3(col.straff, 0, col.forward) * playerStats.speed;//transform.TransformDirection(new Vector3(col.straff, 0, col.forward)) * playerStats.speed;//inial player speed
		
		//Gravity isn't always down. We cannot trust unitys built in "grounded" function, instead we use its "raycast" function to see what is below us.
		if(col.jump)
		{
			if(Physics.Raycast(transform.position, -transform.up, ((collider.bounds.size.x + collider.bounds.size.y + collider.bounds.size.z)/3)  * 1.1f))
			{
				playerMotion += playerStats.jump * Vector3.up;//objectStats.unitOppGrav; //JUMP IS NOT RELATIVE TO PLAYER
			}
		//	var toRotateLine = Quaternion.FromToRotation(objectStats.unitOppGrav, objectStats.normalOfLastHit);
		//	playerMotion = toRotateLine * playerMotion;
		}
		
		//Moves the player according to the input
		move(playerMotion, Quaternion.Euler(0,col.turnRight,0));
	}
	
	//This kicks the "trigger enter" message back to the object the player has entered. That object deals with the collision.
	//This is a missing feature that was fixed in unity 4.
	void OnTriggerEnter(Collider c)
	{
		if (Network.peerType == NetworkPeerType.Server)
		{
			// Tell Dog I just died!
			//Tells the object the player it it was hit by a player (Rigidbody hear about collision events, Colliders don't).
			c.gameObject.SendMessage("PlayerEnter", player.name, SendMessageOptions.DontRequireReceiver); //We don't case if anyone gets the message.
		}
	}
	
	//Same deal as the "OnTriggerEnter" function above.
	void OnTriggerExit(Collider c)
	{
		if (Network.peerType == NetworkPeerType.Server)
		{
			// Tell Dog I just died!
			c.gameObject.SendMessage("PlayerExit", player.name, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	
	//This is the line that is drawn when the player right clicks. 
	void drawLaserPointer(bool active)
	{
		
		LineRenderer lineRenderer  = GetComponent<LineRenderer>();
		
		if(active)
		{
			Transform hand = playerArms.Find("Hand");
			lineRenderer.useWorldSpace = true;
		    lineRenderer.SetVertexCount(2);
			lineRenderer.SetPosition(0,hand.position);
		    RaycastHit hit;
		    Physics.Raycast(hand.position, hand.forward, out hit);
			//Draw the line 500 units long, or until it hits something. 
			//The line will draw over everything, so unless you stop the line, it looks a bit silly.
		    if(hit.collider && !hit.collider.isTrigger) 
			{
		    	lineRenderer.SetPosition(1, hand.position + hand.forward * hit.distance);
		    }
		    else
			{
		        lineRenderer.SetPosition(1, hand.position + hand.forward * 500);
		    }
		}
		else
		{
			lineRenderer.SetPosition(0,new Vector3(0,0,0));
			lineRenderer.SetPosition(1,new Vector3(0,0,0));
		}
	}
}