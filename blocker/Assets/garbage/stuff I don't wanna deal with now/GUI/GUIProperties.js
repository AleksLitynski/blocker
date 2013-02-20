var mySkin:GUISkin;

@HideInInspector
var transBox:Texture2D;

function Start()
{
	transBox = new Texture2D(1,1);
	var temp = Color.black;
	temp.a = 0.5;
	transBox.SetPixel(0,0,temp);
	transBox.Apply();
}