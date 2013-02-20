using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerStats))]
public class NetPlayer : NetObject 
{
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
	

    bool _keyboardPlayer = false;
    bool _mobilePlayer = false;
    int _controllerNumber = -1;
	public float lookPitch = 0;
	public bool laserOn = false;
	
	public bool jumpWasUp = true;
	
	GameManager gameManager;
	
	public override void Start()
	{
		base.Start();
	}
	
	public void Awake()
	{
		player = transform.parent;
		playerArms = transform.FindChild("Model/Arms");
		playerCamera = player.FindChild("Camera");
		playerArrow = playerCamera.FindChild("Compass/arrow");
		playerCompass = playerCamera.FindChild("Compass");
		playerStats = gameObject.GetComponent<PlayerStats>();
		
		
	}
	
	bool compassHasBeenInited = false;
	public void initCompassLayer()
	{
		int newLayer = 21 + localPlayerNumber;
		//set the compass to the local player's level
		if(Network.player == networkPlayer)//if it's a local player 
		{
			playerCompass.gameObject.layer = newLayer;
			for(int i = 0; i < playerCompass.GetChildCount(); i++)
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
		
		
		
		if(!compassHasBeenInited)
		{
			initCompassLayer();
		}
		
		
		if (menuManager.bgMap != null)
		{
			gameManager = menuManager.bgMap.GetComponent<GameManager>();
		}
		
		
		GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
		Vector3 target = Vector3.one * 1000000;
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
		playerArrow.LookAt(target);
		
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
	//	Debug.Log((21+localPlayerNumber) + ": " + toLog);
		playerCamera.camera.cullingMask = LayerMaskHelper.EverythingBut(toIgnore.ToArray()); 
		
		drawLaserPointer(laserOn);
	}

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
		if(col.jump)
		{
			if(Physics.Raycast(transform.position, -transform.up, ((collider.bounds.size.x + collider.bounds.size.y + collider.bounds.size.z)/3)  * 1.1f))
			{
				playerMotion += playerStats.jump * Vector3.up;//objectStats.unitOppGrav; //JUMP IS NOT RELATIVE TO PLAYER
			}
		//	var toRotateLine = Quaternion.FromToRotation(objectStats.unitOppGrav, objectStats.normalOfLastHit);
		//	playerMotion = toRotateLine * playerMotion;
		}
		move(playerMotion, Quaternion.Euler(0,col.turnRight,0));
	}
	
	void OnTriggerEnter(Collider c)
	{
		if (Network.peerType == NetworkPeerType.Server)
		{
			// Tell Dog I just died!
			c.gameObject.SendMessage("PlayerEnter", player.name, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void OnTriggerExit(Collider c)
	{
		if (Network.peerType == NetworkPeerType.Server)
		{
			// Tell Dog I just died!
			c.gameObject.SendMessage("PlayerExit", player.name, SendMessageOptions.DontRequireReceiver);
		}
	}
	
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