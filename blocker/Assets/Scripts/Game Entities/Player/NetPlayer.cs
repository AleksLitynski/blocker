using UnityEngine;
using System.Collections;

public class NetPlayer : NetObject 
{
    public int localPlayerNumber;
    public NetworkPlayer networkPlayer;
	public PlayerStats playerStats = new PlayerStats();
	public Transform playerArms;

    bool _keyboardPlayer = false;
    bool _mobilePlayer = false;
    int _controllerNumber = -1;
	
	void Start()
	{
		playerArms = transform.FindChild("Arms");
	}

    public bool KeyboardPlayer
    {
        get
        {
            return _keyboardPlayer;
        }
        set
        {
            _keyboardPlayer = true;
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
            _mobilePlayer = true;
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
	
	void Update()
	{
		if(playerStats.grav != Vector3.zero)
		{
			playerStats.unitOppGrav = -1 * playerStats.grav.normalized;
	    }
		
		if(Physics.Raycast(transform.position, -transform.up, collider.bounds.size.y + collider.bounds.size.y/5, LayerMask.NameToLayer("Player")))
		{
			playerStats.isGrounded = true;
		}
		else
		{
			playerStats.isGrounded = false;
		}
	}
	
	public void move(InputCollection col)
	{
		
		rigidbody.velocity = new Vector3(rigidbody.velocity.x + col.straff, rigidbody.velocity.y, rigidbody.velocity.z + col.forward);
		rigidbody.angularVelocity = new Vector3(rigidbody.angularVelocity.x/2 + 0.0f, rigidbody.angularVelocity.y/2 + col.turnRight, rigidbody.angularVelocity.z/2 + col.turnUp);
		
		
		
		
		
		
		
		
	}
	
	Vector3 calcRotation(float x, float y)
	{
		if(playerArms != null)
		{
			float armRot = playerArms.localRotation.eulerAngles.x - x;
			if(!((armRot <= 60 && armRot >= -5) || (armRot >= 270 && armRot <= 365)))
			{
				x = 0.0f;
			}
		}
		
		Quaternion rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, playerStats.unitOppGrav), playerStats.unitOppGrav);
		rotation = Quaternion.RotateTowards(transform.rotation, rotation, playerStats.maxGravRoll);
		rotation = rotation * Quaternion.Euler(0,y,0);
		
		playerArms.Rotate(new Vector3(-x,0,0));
	
		return rotation.eulerAngles;
	}
	
	Vector3 calcMotion(Vector3 inputMotion, bool jump)
	{
		Vector3 playerMotion = transform.TransformDirection(inputMotion) * playerStats.speed;//inial player speed
		
		if(playerStats.isGrounded) //check if you are touching walls too!!!!!!!!!!!!!!! <TODOTODOTODOTODO>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
		{
			playerStats.velo *= playerStats.fric; //apply friction if on surface
			if(jump)
			{
				playerStats.velo.x = playerStats.jump * playerStats.unitOppGrav.x;
				playerStats.velo.y = playerStats.jump * playerStats.unitOppGrav.y;
				playerStats.velo.z = playerStats.jump * playerStats.unitOppGrav.z;
			}
			
			Quaternion toRotateLine = Quaternion.FromToRotation(playerStats.unitOppGrav, playerStats.normalOfLastHit);// <What is normal of last hit??????>
			playerMotion = toRotateLine * playerMotion;
		}
		else
		{
			playerStats.velo += playerStats.grav; //dont apply gravity when on the ground. Perfect normal force
		}
		
		playerStats.velo += playerMotion; //apply movment relative to model
		
	    return playerStats.velo * Time.deltaTime;
	}
	
	[RPC]
	void setPlayerPos(Vector3 newPos)
	{
    	rigidbody.position = newPos;
	}
	
	[RPC]
	void setPlayerRot(Vector3 newRot)
	{
    	rigidbody.rotation = Quaternion.Euler(newRot);
	}

}