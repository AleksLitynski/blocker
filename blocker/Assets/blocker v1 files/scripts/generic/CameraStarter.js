function FocusCameraOnObject(focusOn:String, displacement:Vector3, cameraTilt:float)
{
	if(focusOn != null)
	{
		transform.parent = GameObject.Find(focusOn).transform;
	}
	else
	{
		transform.parent = GameObject.Find("World").transform;
	}
	//zero out camera mount and camera locations 
	transform.localPosition = Vector3.zero;
	transform.FindChild("camera").localPosition = Vector3.zero;
	transform.localRotation = Quaternion.identity;
	transform.FindChild("camera").localRotation = Quaternion.identity;
	
	//position camera behind mount
	transform.FindChild("camera").localPosition.z += displacement.z;
	transform.FindChild("camera").localPosition.y += displacement.y;
	transform.FindChild("camera").localPosition.x += displacement.x;
	transform.FindChild("camera").Rotate(new Vector3(cameraTilt,0,0));
}
