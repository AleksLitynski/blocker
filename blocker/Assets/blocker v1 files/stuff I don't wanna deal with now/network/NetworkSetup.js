var windowSize:Rect;

var remoteIP = "bigwhite.student.rit.edu";
var remotePort = 25000;
var listenPort = 25000;
var useNAT = false;
var yourIP = "";
var yourPort = "";
//var defaultFont:Font;

function Start()
{
	ResolutionChanged(Screen.width, Screen.height);
}

function OnGUI()
{
		GUI.skin = GetComponent("GUIProperties").mySkin;

		GUI.BeginGroup(windowSize);
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			GUI.DrawTexture(new Rect(0,0,windowSize.width, windowSize.height), GetComponent("GUIProperties").transBox);
			if(GUI.Button (new Rect(120,40,100,30), "Connect"))
			{
				connect();
			}
			if(GUI.Button (new Rect(10,40,100,30), "Start Server"))
			{
				startServer();
			}
			remoteIP = GUI.TextField(new Rect(10, 10, 160, 20), remoteIP);
			remotePort = parseInt(GUI.TextField(new Rect(170, 10, 50, 20), remotePort.ToString()));
			listenPort = remotePort;
		}
		else
		{
			if(gameObject.GetComponent("PauseGame").paused)
			{
				GUI.DrawTexture(new Rect(0,0,windowSize.width, windowSize.height), GetComponent("GUIProperties").transBox);
				var ipaddress = Network.player.ipAddress;
				var port = Network.player.port.ToString();
				GUI.Label(new Rect(10, 10, 230, 20), "IP Adress: " + ipaddress + ":" + port);
				
				if(GUI.Button(new Rect(10, 30, 210, 40), "Disconnect"))
				{
					disconnect();
				}
			}
		}
	GUI.EndGroup();
}

//network function
function connect()
{
	var connection = Network.Connect(remoteIP, remotePort);
	if(connection == NetworkConnectionError.NoError)
	{
		windowSize.y = Screen.height - windowSize.height;
		windowSize.x = Screen.width - windowSize.width;
	}
	
}

function disconnect()
{
	Network.Disconnect(200);
	windowSize.y = Screen.height/2-40;
	windowSize.x = Screen.width/2-115;
}

function startServer()
{
	var connection = Network.InitializeServer(32, listenPort, useNAT);
	if(connection == NetworkConnectionError.NoError)
	{
		for(var go: GameObject in FindObjectsOfType(GameObject))
		{
			go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
		}
		windowSize.y = Screen.height - windowSize.height;
		windowSize.x = Screen.width - windowSize.width;
	}
}

//true if connected
var connected = false;
function OnDisconnectedFromServer (m:NetworkDisconnection){connected = false;}
function OnServerInitialized(){ connected = true;}
function OnConnectedToServer(){ connected = true;}

function ResolutionChanged(width:int, height:int)
{
	if(connected)
	{
		windowSize = new Rect(width - windowSize.width, height - windowSize.height,230,80);
	}
	else
	{
		windowSize = new Rect(width/2-115,height/2-40,230, 80);
	}
}