var speed:float = 1;
var turnSpeed:float = 40;
var jump:float = 75;
var grav:Vector3 = new Vector3(0, -3, 0);
var isGrounded:boolean = false;
var fric:float = 0.90;
var velo:Vector3 = new Vector3(0, 0, 0);
var maxGravRoll:float = 5;
var scopedRange:float = 20;
var mass:float = 10;

var unitOppGrav:Vector3;
var guns:GameObject[];
var curGun:GameObject;
var maxGuns:int = 10;
var crossHairLoc:Vector2;
var target:Target;
class Target extends System.ValueType
{
    var position:Vector3;
    var forward:Vector3;
}

var score:int = 0;
var health:float = 100;


function ToString()
{
	return "Speed: " + speed + 
	" || Turn Speed: " + turnSpeed + 
	" || Jump: " + jump + 
	" || grav: " + grav + 
	" || grounded: " + isGrounded + 
	" || fric: " + fric + 
	" || velo: " + velo + 
	" || Max Grav Roll: " + maxGravRoll +
	" || Score: " + score + 
	" || Health: " + health; 
}

