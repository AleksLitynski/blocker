using UnityEngine;
using System.Collections;

public class NetPlayer : NetObject 
{
    public int localPlayerNumber;
    public NetworkPlayer networkPlayer;
	public PlayerStats playerStats = new PlayerStats();
	
	private Transform playerArms;

    bool _keyboardPlayer = false;
    bool _mobilePlayer = false;
    int _controllerNumber = -1;
	
	public int playerID = Random.Range (0,10000); // fix the shit out of this
	
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
		
		Quaternion rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, objectStats.unitOppGrav), objectStats.unitOppGrav);
		rotation = Quaternion.RotateTowards(transform.rotation, rotation, objectStats.maxGravRoll);
		rotation = rotation * Quaternion.Euler(0,col.turnRight,0);
	
		rigidbody.rotation = rotation;
		
		if(playerArms != null)
		{
			float armRot = playerArms.localRotation.eulerAngles.x - col.turnUp;
			if(!((armRot <= 60 && armRot >= -5) || (armRot >= 270 && armRot <= 365)))
			{
				col.turnUp = 0.0f;
			}
			playerArms.Rotate(new Vector3(-col.turnUp,0,0));
		//	networkView.RPC ("setArmRotation", RPCMode.Others, -col.turnUp);
		}
		
		
		
		
		Vector3 playerMotion = transform.TransformDirection(new Vector3(col.straff, 0, col.forward)) * playerStats.speed;//inial player speed
		
		
		if(col.jump)
		{
			if(Physics.Raycast(transform.position, -transform.up, collider.bounds.size.y/1.8f))
			{
				playerMotion += playerStats.jump * objectStats.unitOppGrav;
			}
			
			var toRotateLine = Quaternion.FromToRotation(objectStats.unitOppGrav, objectStats.normalOfLastHit);
			playerMotion = toRotateLine * playerMotion;
		}
		
	    rigidbody.AddForce(playerMotion * Time.deltaTime * 1000);
	}
	
	/*[RPC]
	void setArmRotation(float rotation)
	{
		playerArms.Rotate(new Vector3(rotation,0,0));
	}*/
	
	

	void OnTriggerEnter(Collider c)
	{
		// tell the checkpoint it was hit
		if (Network.peerType == NetworkPeerType.Server)
		{
			if (c.gameObject.name == "Checkpoint")
			{
				c.gameObject.GetComponent<RaceCheckpoint>().hit = true;
				c.gameObject.GetComponent<RaceCheckpoint>().hitby = playerID;
			}
		}
	}
}