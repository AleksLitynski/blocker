using UnityEngine;
using System.Collections;

public class InputDispatcher : BlockerObject
{
    void Update()
    {
        foreach(NetPlayer curPlayer in playerManager.localPlayers)
        {
            InputCollection collection = new InputCollection(curPlayer);
            if (curPlayer.KeyboardPlayer)
            {
                collection.forward = 0;
                if(Input.GetKey("w")) collection.forward += 1;
                if(Input.GetKey("s")) collection.forward -= 1;
                collection.straff = 0;
                if (Input.GetKey("d")) collection.straff += 1;
                if (Input.GetKey("a")) collection.straff -= 1;
                collection.turnRight = Input.GetAxis("mouseX");
                collection.turnUp = Input.GetAxis("mouseY");
                collection.jump = Input.GetKeyDown(KeyCode.Space);
                collection.fireOne = Input.GetMouseButton(0);
                collection.fireTwo = Input.GetMouseButton(1);
                collection.sprint = Input.GetKeyDown(KeyCode.LeftShift);
                collection.crouch = Input.GetKeyDown(KeyCode.LeftControl);
            }
            else if (curPlayer.MobilePlayer)
            {
                Debug.Log("We don't support mobile. Yet.");
            }
            else if (curPlayer.ControllerNumber >= 0 && curPlayer.ControllerNumber <= 3)
            {
				int num = curPlayer.ControllerNumber + 1;
                collection.forward = -Input.GetAxis("L_YAxis_" + num);
                collection.straff = Input.GetAxis("L_XAxis_" + num);
                collection.turnRight = 2*Input.GetAxis("R_XAxis_" + num);
                collection.turnUp = -1.5f*Input.GetAxis("R_YAxis_" + num);
                collection.jump = (Input.GetAxis("A_" + num) != 0) && curPlayer.jumpWasUp;
                collection.fireOne = Input.GetAxis("RB_" + num) != 0; 
                collection.fireTwo = Input.GetAxis("LB_" + num) != 0; 
                collection.sprint = Input.GetAxis("L_XAxis_" + num) != 0;
                collection.crouch = Input.GetAxis("L_XAxis_" + num) != 0;
				
				if(Input.GetAxis("A_" + num) == 0)
				{
					curPlayer.jumpWasUp = true;	
				}
				else
				{
					curPlayer.jumpWasUp = false;	
				}
				
            }
			Debug.Log(collection.netPlayer.player.name + ": " + collection.jump);
            collection.sendToServerVia(networkView, inputReceiver);
			
			
        }
    }
	
	public override void Start()
	{
		base.Start();	
	}
	
	
	

}
