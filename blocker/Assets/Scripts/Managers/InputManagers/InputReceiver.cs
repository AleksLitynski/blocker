using UnityEngine;
using System.Collections;

public class InputReceiver : BlockerObject {

	[RPC]
    public void AddInput(float f, float s, float tR, float tU, bool j, bool f1, bool f2, bool sp, bool c, int localNumber, NetworkMessageInfo incomingInfo=new NetworkMessageInfo())
    {
        NetworkMessageInfoLocalWrapper info = new NetworkMessageInfoLocalWrapper(incomingInfo);

        InputCollection sentCollection = new InputCollection();
        foreach (NetPlayer player in playerManager.players)
        {
            if (player.networkPlayer == info.sender && player.localPlayerNumber == localNumber)
            {
                sentCollection = new InputCollection(player, f, s, tR, tU, j, f1, f2, sp, c);
                break;
            }
        }
    }
}
