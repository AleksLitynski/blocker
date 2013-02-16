using UnityEngine;
using System.Collections;
using System.Xml;

public class mapConverter 
{
	
	//it gets a map and a description
	public string createMapXML(GameObject map)
	{
		//makes an xml doc.
		XmlDocument doc = new XmlDocument();
		XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", null, null);
		doc.AppendChild(declaration);
		XmlElement root = doc.CreateElement("Map");
		root.SetAttribute("name", map.name);
		root.SetAttribute("description", map.GetComponent<GameManager>().gameDescription);
		doc.AppendChild(root);
		
		
		for(int i = 0; i < map.transform.childCount; i++)//go through the map and put everything in xml
		{
			GameObject child = map.transform.GetChild(i).gameObject;
			XmlElement newChild = doc.CreateElement("object");
			switch (child.tag)
			{
				case "Spawn":   
		            newChild = createSpawnNode(child, doc);
		            break;   
				case "terrain":   
		            newChild = createTerrainNode(child, doc);
		            break;  
				case "RaceCheckpoint":   
		            newChild = createCheckpointNode(child, doc);
		            break; 
				case "light":   
		            newChild = createLightNode(child, doc);
		            break; 
			}
			if(newChild.Name != "object")
			{
				root.AppendChild(newChild);	
			}
			
		}
		return doc.OuterXml;//return xml doc.
	}
	
	//returns xml of spawn
	private XmlElement createSpawnNode(GameObject spawn, XmlDocument doc)
	{
		XmlElement toReturn = doc.CreateElement("Spawn");
		
		toReturn.SetAttribute("positionX", ""+spawn.transform.position.x);
		toReturn.SetAttribute("positionY", ""+spawn.transform.position.y);
		toReturn.SetAttribute("positionZ", ""+spawn.transform.position.z);
		toReturn.SetAttribute("scaleX", ""+spawn.transform.position.x);
		toReturn.SetAttribute("scaleY", ""+spawn.transform.localScale.y);
		toReturn.SetAttribute("scaleZ", ""+spawn.transform.localScale.z);
		
		return toReturn;
	}
	private XmlElement createTerrainNode(GameObject terrain, XmlDocument doc)
	{
		XmlElement toReturn = doc.CreateElement("Terrain");
		
		toReturn.SetAttribute("positionX", ""+terrain.transform.position.x);
		toReturn.SetAttribute("positionY", ""+terrain.transform.position.y);
		toReturn.SetAttribute("positionZ", ""+terrain.transform.position.z);
		toReturn.SetAttribute("scaleX", ""+terrain.transform.position.x);
		toReturn.SetAttribute("scaleY", ""+terrain.transform.localScale.y);
		toReturn.SetAttribute("scaleZ", ""+terrain.transform.localScale.z);
		string type = "sphere"; if(terrain.collider is BoxCollider) type = "cube";
		toReturn.SetAttribute("bigG", ""+terrain.GetComponent<Graviton>().bigG);
		toReturn.SetAttribute("type", type);
		float x = 0;
		float y = 0;
		float z = 0;
		if(terrain.transform.childCount >= 1)
		{
			x = terrain.transform.GetChild(0).localScale.x;
			y = terrain.transform.GetChild(0).localScale.y;
			z = terrain.transform.GetChild(0).localScale.z;
		}
		toReturn.SetAttribute("influenceX", ""+x);
		toReturn.SetAttribute("influenceY", ""+y);
		toReturn.SetAttribute("influenceZ", ""+z);
		
		return toReturn;
	}
	
	
	private XmlElement createCheckpointNode(GameObject checkpoint, XmlDocument doc)
	{
		XmlElement toReturn = doc.CreateElement("Checkpoint");
		Zone comp = checkpoint.GetComponent<Zone>();
		int order = -1;
		float maxPoints = -1;
		float scoreReward = -1;
		string type = "sphere";
		float scaleX = 0;
		float scaleY = 0;
		float scaleZ = 0;
		
		if(comp)
		{
			order = comp.orderInRace;
			maxPoints = comp.maxPoints;
			scoreReward = comp.scoreReward;
			type = comp.collisionType.ToString();
			scaleX = comp.scale.x;
			scaleY = comp.scale.y;
			scaleZ = comp.scale.z;
		}
		toReturn.SetAttribute("positionX", ""+checkpoint.transform.position.x);
		toReturn.SetAttribute("positionY", ""+checkpoint.transform.position.y);
		toReturn.SetAttribute("positionZ", ""+checkpoint.transform.position.z);
		toReturn.SetAttribute("orderInRace", ""+order);
		toReturn.SetAttribute("maxPoints", ""+maxPoints);
		toReturn.SetAttribute("scoreReward", ""+scoreReward);
		toReturn.SetAttribute("type", type);
		toReturn.SetAttribute("scaleX", ""+scaleX);
		toReturn.SetAttribute("scaleY", ""+scaleY);
		toReturn.SetAttribute("scaleZ", ""+scaleZ);
		
		return toReturn;
	}
	
	//returns an xmlElement version of a light
	private XmlElement createLightNode(GameObject light, XmlDocument doc)
	{
		XmlElement toReturn = doc.CreateElement("Light");
		
		toReturn.SetAttribute("positionX", ""+light.transform.position.x);
		toReturn.SetAttribute("positionY", ""+light.transform.position.y);
		toReturn.SetAttribute("positionZ", ""+light.transform.position.z);
		string type = "Point";
		string color = Color.red.ToString();
		float intensity = -1;
		float spotangle = -1;
		float range = -1;
		
		Light comp = light.GetComponent<Light>();
		if(comp)
		{
			type = comp.type.ToString();
			color = comp.color.ToString();
			intensity = comp.intensity;
			spotangle = comp.spotAngle;
			range = comp.range;
			//range,color,intensity,spotangle,type
		}		
		
		toReturn.SetAttribute("type", type);
		toReturn.SetAttribute("color", ""+color);
		toReturn.SetAttribute("type", type);
		toReturn.SetAttribute("intensity", ""+intensity);
		toReturn.SetAttribute("spotangle", ""+spotangle);
		toReturn.SetAttribute("range", ""+range);
		
		return toReturn;
	}
	
	
	
	public GameObject readXmlMap(string xmlMap)
	{
		GameObject toReturn = new GameObject();
		XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlMap);

        XmlNodeList root = xmlDoc.GetElementsByTagName("Map");
		
		foreach (XmlNode node in root)
		{
			Debug.Log(node.Name);
			if(node.Name == "Map")
			{
				toReturn.name = node.Attributes["name"].Value;
				toReturn.AddComponent<MapMetadata>();
				toReturn.GetComponent<MapMetadata>().description = node.Attributes["description"].Value;
				
				for(int i = 0; i < node.ChildNodes.Count; i++)
				{
					GameObject newChild = new GameObject();
					newChild.name = "temp";
					XmlNode child = node.ChildNodes[i];
					switch (child.Name)
					{
						case "Spawn":   
				            newChild = createSpawn(child.Attributes);
				            break;   
						case "Terrain":   
				            newChild = createTerrain(child.Attributes);
				            break;  
						case "Checkpoint":   
				            newChild = createCheckpoint(child.Attributes);
				            break; 
						case "light":   
				            newChild = createLight(child.Attributes);
				            break; 
					}
					if(newChild.name == "temp")
					{
						GameObject.Destroy(newChild);
					}
					else
					{
						newChild.transform.parent = toReturn.transform;
					}
				}
			}
		}
		
		
		return toReturn;
	}
	
	private GameObject createSpawn(XmlAttributeCollection xml)
	{
		GameObject toReturn = new GameObject();
	
		//xml["positionX"];
		//xml["positionY"];
		//xml["positionZ"];
		//xml["scaleX"];
		//xml["scaleY"];
		//xml["scaleZ"];
		
		
		return toReturn;
	}
	private GameObject createTerrain(XmlAttributeCollection xml)
	{
		GameObject toReturn = new GameObject();
		//xml["positionX"];
		//xml["positionY"];
		//xml["positionZ"];
		//xml["scaleX"];
		//xml["scaleY"];
		//xml["scaleZ"];
		
		
		//xml["influenceX"];
		//xml["influenceY"];
		//xml["influenceZ"];
		
		return toReturn;	
	}
	private GameObject createCheckpoint(XmlAttributeCollection xml)
	{
		GameObject toReturn = new GameObject();
		
		//xml["positionX"];
		//xml["positionY"];
		//xml["positionZ"];
		//xml["scaleX"];
		//xml["scaleY"];
		//xml["scaleZ"];
		//xml["orderInRace"];
		//xml["maxPoints"];
		//xml["scoreReward"];
		//xml["type"];
		
		return toReturn;	
	}
	private GameObject createLight(XmlAttributeCollection xml)
	{
		GameObject toReturn = new GameObject();
		
		//xml["positionX"];
		//xml["positionY"];
		//xml["positionZ"];
		//xml["type"];
		//xml["color"];
		//xml["type"];
		//xml["intensity"];
		//xml["spotangle"];
		//xml["range"];
		
		return toReturn;	
	}
	
	
	
	
}

//block location
//block l,w,h
//block type (sphere/cube)
//block big G
//block influince radius

//spawn point
//order in race, maxPoints, scoreReward, type:sphere/cube, scale

//description


		//root.AppendChild(book);