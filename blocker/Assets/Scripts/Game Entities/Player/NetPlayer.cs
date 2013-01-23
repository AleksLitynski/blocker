using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStats))]
public class NetPlayer : NetObject 
{
	[HideInInspector]
    public int localPlayerNumber;
	[HideInInspector]
    public NetworkPlayer networkPlayer;
<<<<<<< HEAD
	//[HideInInspector]
	public PlayerStats playerStats = new PlayerStats();
=======
	[HideInInspector]
	public PlayerStats playerStats;
>>>>>>> Made playerStats and objectStats components.
	
	[HideInInspector]
	public Transform playerArms;

    bool _keyboardPlayer = false;
    bool _mobilePlayer = false;
    int _controllerNumber = -1;
	
	void Awake()
	{
		playerArms = transform.FindChild("Arms");
		playerStats = gameObject.GetComponent<PlayerStats>();
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
				playerMotion += playerStats.jump * transform.up;//objectStats.unitOppGrav;
			}
			Debug.DrawLine(transform.position, transform.position + playerStats.jump * transform.up, Color.green);
			
		//	var toRotateLine = Quaternion.FromToRotation(objectStats.unitOppGrav, objectStats.normalOfLastHit);
		//	playerMotion = toRotateLine * playerMotion;
		}
		Debug.DrawLine(transform.position, transform.position - transform.up * (((collider.bounds.size.x + collider.bounds.size.y + collider.bounds.size.z)/3)  * 1.1f), Color.red);
		
		move(playerMotion, Quaternion.Euler(0,col.turnRight,0));
	}
	
	
	
	

	void OnTriggerEnter(Collider c)
	{
		// tell the checkpoint it was hit
		if (Network.peerType == NetworkPeerType.Server)
		{
			// Tell Dog I just died!
			c.gameObject.SendMessage("CollideWithPlayer", this.gameObject.name);
		}
	}
}