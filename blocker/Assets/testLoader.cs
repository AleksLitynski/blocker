using UnityEngine;
using System.Collections;


/*
 * Loads a map from XML. ----------STILL TESTING. DO NOT REALY ON IT------------
 * 
 * 
 */
public class testLoader : BlockerObject {

	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	mapConverter mapC = new mapConverter();
	// Update is called once per frame
	void Update () {
		if(menuManager.bgMap != null && !hasLoaded)
		{
			string outa = 	mapC.createMapXML(menuManager.bgMap);
			Debug.Log(mapC.readXmlMap(outa));
				hasLoaded = true;
		}
	}
				bool hasLoaded = false;
}
