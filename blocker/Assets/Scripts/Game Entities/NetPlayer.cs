using UnityEngine;
using System.Collections;

public class NetPlayer : NetObject 
{
    public int localPlayerNumber;
    public NetworkPlayer networkPlayer;

    bool _keyboardPlayer = false;
    bool _mobilePlayer = false;
    int _controllerNumber = -1;
	

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
		
	}
	
	public void move(InputCollection col)
	{
			
		rigidbody.velocity = new Vector3(rigidbody.velocity.x + col.straff, rigidbody.velocity.y, rigidbody.velocity.z + col.forward);
		rigidbody.angularVelocity = new Vector3(rigidbody.angularVelocity.x/2 + 0.0f, rigidbody.angularVelocity.y/2 + col.turnRight, rigidbody.angularVelocity.z/2 + col.turnUp);
		
		/*
		if(col.jump)
		{
			
		}
		if(col.fireOne)
		{
			
		}
		if(col.fireTwo)
		{
			
		}
		if(col.sprint)
		{
			
		}
		if(col.crouch)
		{
			
		}*/
		
	}

}