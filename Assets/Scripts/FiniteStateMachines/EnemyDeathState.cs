using UnityEngine;
using System.Collections;

/// <summary>
/// Enemy death state will activate gravity and allow the object to fall out of the sky for a while.
/// Then destroys itself.
/// </summary>
public class EnemyDeathState : IState {

	public float TimeTillFade = 2.0f;
	private float TimeTillFadeRemaining;
	
	// Use this for initialization
	public override void OnStart () {
		NextState = this;
		this.TimeTillFadeRemaining = this.TimeTillFade;
	}
	
	// Update is called once per frame
	public override void OnUpdate () {

		this.TimeTillFadeRemaining -= Time.deltaTime;


		if ( this.rigidbody != null )
		{
			this.rigidbody.useGravity = true;
			this.rigidbody.AddForce(Vector3.down);
		}

		if ( this.TimeTillFadeRemaining <= 0 )
		{
			DestroyObject(this.StateGameObject);
		}

	}
}
