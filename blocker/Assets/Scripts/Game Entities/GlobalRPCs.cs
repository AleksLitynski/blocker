using UnityEngine;
using System.Collections;

public class GlobalRPCs : BlockerObject 
{

	public override void Start () {
		base.Start();
	}
	
	
	[RPC]
	public void setTransform(Vector3 pos, Vector3 rot, string objName)
	{
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("netObject"))
        {
            if (obj.name == objName)
            {
				obj.GetComponent<NetObject>().setTransform(pos, rot);
                break;
            }
        }
	}
	
	[RPC]
	public void createNetObject(Vector3 pos, Vector3 rot, string objName)
	{
		//Instantiate(
	}
	
	[RPC]
	public void destroyNetObject(string objName)
	{
		
	}
	
	[RPC]
	public void applyVelocityToNetObject(string objName)
	{
		
	}
	
		
	//http://answers.unity3d.com/questions/8500/how-can-i-get-the-full-path-to-a-gameobject.html
	public static string GetGameObjectPath(GameObject obj)
	{
	    string path = "/" + obj.name;
	    while (obj.transform.parent != null)
	    {
	        obj = obj.transform.parent.gameObject;
	        path = "/" + obj.name + path;
	    }
	    return path;
	}
	
	
}
