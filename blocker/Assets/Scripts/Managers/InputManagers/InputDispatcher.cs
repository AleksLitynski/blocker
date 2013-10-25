using UnityEngine;
using System.Collections;

/* This is called on the machine responsable for collecting input about a given player (there may be multiple players per machine)
 * 
 * 
 */
public class InputDispatcher : BlockerObject
{
    void Update()
    {
        foreach(NetPlayer curPlayer in playerManager.localPlayers) //when players join the game, they are added to "localPlayers", the sum players don't have to be analysed to find local ones!
        {
            InputCollection collection = new InputCollection(curPlayer);
            if (curPlayer.KeyboardPlayer) //collect input off the keyboard
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
            else if (curPlayer.MobilePlayer) //we can't collect input from mobile, we don't support it. Yet. DOCUMENTATION!@!!
            {
                Debug.Log("We don't support mobile. Yet.");
            }
            else if (curPlayer.ControllerNumber >= 0 && curPlayer.ControllerNumber <= 3) //We can only do 4 controllers at once. it sucks. I tried really hard to do more, but microsoft's a stinker.
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
            collection.sendToServerVia(networkView, inputReceiver); //The collection has a simple RPC call in it
			
			
        }
    }
	
	public override void Start()
	{
		base.Start();	
	}
	
	
	

}
