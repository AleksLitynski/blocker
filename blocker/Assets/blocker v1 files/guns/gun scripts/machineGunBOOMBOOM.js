function Fire(type:String)
{
	Debug.Log("firing BOOMBOOM" + type);
	if(type == "left")
	{
		Debug.Log("BOOM Left");
	}
	if(type == "right")
	{
		Debug.Log("BOOM Right");
	}
}