#pragma strict


function Update () {
	if(Input.GetButtonUp("Fire3")){
		Debug.Log("Jumped!");
	}

	var x : float = Input.GetAxis("Horizontal");
	transform.Translate(x ,0,0);
}

function OnCollisionEnter (obj : Collision) {
	if (obj.gameObject.name == "RightWall"){
		Debug.Log("Hit on RightWall");
	}
	if (obj.gameObject.name == "LeftWall"){
		Debug.Log("Hit on LeftWall");
	}
}