using UnityEngine;
using System.Collections;

public class ObjectManager : BlockerObject 
{

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
	
	[RPC]
	void removeObject(string path)
	{
		Destroy(GameObject.Find(path));
	}
	
	[RPC]
	void setBulletVelocity(Vector3 velo, string path)
	{
		GameObject.Find(path).rigidbody.AddForce(velo, ForceMode.Force);
	}
	
	[RPC]
	void setObjectGravity(Vector3 velo, string path)
	{
		GameObject.Find(path).GetComponent<ObjectStats>().grav = velo;
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
