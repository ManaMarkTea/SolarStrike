using UnityEngine;
using System.Collections;

public class GameOverState : IState {

	public float timer;

	public override void OnStart ()
	{
		timer = 0.0f;
	}

	public override void OnUpdate ()
	{
		timer += Time.deltaTime;

		if ( timer > 3 ) 
		{
			Application.LoadLevel("MainMenu");
		}
	}
}
