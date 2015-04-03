using UnityEngine;
using System.Collections;

/* 
 * This is the terminus for network activity on objects. 
 * It "manages" the state of objects when the server tells it management must be done.
 */
public class ObjectManager : BlockerObject 
{
	//This will create a new GameObject in a given location.
	[RPC]
	void spawnObject(Vector3 pos, Vector3 rot, string name, string assetName, string parentPath)
	{
		GameObject newObj = Instantiate(Resources.Load(assetName), pos, Quaternion.Euler(rot)) as GameObject;
		newObj.name = name;
		newObj.transform.parent = GameObject.Find(parentPath).transform;
		if(newObj.GetComponent<bullet>())
		{
			newObj.GetComponent<bullet>().creationTime = Time.time;	
		}
	}
	
	
	//This destorys an object. Obviously.
	[RPC]
	void removeObject(string path)
	{
		Destroy(GameObject.Find(path));
	}
	
	//This updates a bullets velocity. Duh. Man, I'm good at self documenting code!
	[RPC]
	void setBulletVelocity(Vector3 velo, string path)
	{
		GameObject.Find(path).GetComponent<Rigidbody>().AddForce(velo, ForceMode.Force);
	}
	
	//Sets gravity.
	[RPC]
	void setObjectGravity(Vector3 velo, string path)
	{
		GameObject.Find(path).GetComponent<ObjectStats>().grav = velo;
	}
	
	//Converts a string to path.
	//Why do this? RPCs only support primative (Number, String, Byte types, basically)
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
