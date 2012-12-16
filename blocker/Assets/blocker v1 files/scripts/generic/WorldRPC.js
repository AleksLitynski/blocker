@RPC
function renameObject(objectPath:String, newName:String)
{
	gameObject.Find(objectPath).name = newName;
}