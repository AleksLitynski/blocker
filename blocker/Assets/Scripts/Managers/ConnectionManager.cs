using UnityEngine;
using System.Collections.Generic;

public class ConnectionManager : BlockerObject {
    public GUISkin skin;
	string serverAddress = "bigwhite.student.rit.edu";

    void OnGUI()
    {
        GUI.skin = Resources.Load("MetalGUISkin") as GUISkin;
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            GUILayout.Window(0, new Rect(0, 0, 200, 140), drawWindow, "Connection Settings");
        }
        else
        {
            GUILayout.Window(0, new Rect(0, 0, 200, 80), drawWindow, "Connection Settings");
        }
	}

    void drawWindow(int winID)
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            serverAddress = GUILayout.TextField(serverAddress, 25);
            if (GUILayout.Button("start"))
            {
                Network.InitializeSecurity();
                Network.InitializeServer(32, 44344, false);
            }

            if (GUILayout.Button("join"))
            {
                Network.Connect(serverAddress, 44344);
            }
        }
        else
        {
            if (GUILayout.Button("disconnect"))
            {
                Network.Disconnect();
            }
        }
    }
	


    
}



