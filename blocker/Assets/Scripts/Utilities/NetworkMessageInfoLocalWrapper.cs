using UnityEngine;
using System.Collections;

public class NetworkMessageInfoLocalWrapper
{
    public double timestamp;
    public NetworkPlayer sender;
    public NetworkMessageInfoLocalWrapper(NetworkMessageInfo info)
    {
        NetworkMessageInfo voidInfo = new NetworkMessageInfo();
        if (info.sender == voidInfo.sender && info.timestamp == voidInfo.timestamp)
        {
            timestamp = Network.time;
            sender = Network.player;
        }
        else
        {
            timestamp = info.timestamp;
            sender = info.sender;
        }
    }
}
