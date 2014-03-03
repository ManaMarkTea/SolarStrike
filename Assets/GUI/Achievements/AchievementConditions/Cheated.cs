using UnityEngine;
using System.Collections;

public class Cheated : IAchievementCondition {

	public override bool IsUnlocked ()
	{
		if ( 
		    Input.GetKeyDown(KeyCode.F1 ) || 
		    Input.GetKeyDown(KeyCode.F2 ) || 
		    Input.GetKeyDown(KeyCode.F3 ) || 
		    Input.GetKeyDown(KeyCode.F5 ) || 
		    Input.GetKeyDown(KeyCode.F6 )
		    )
		{
			return true;
		}

		return false;
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
