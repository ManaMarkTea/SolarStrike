using UnityEngine;
using System.Collections;

/// <summary>
/// Attack state.
/// In this state, the enemy AI will fire toward the target once it is in range.
/// While out of range, it will set course for the target by passing A-Star. (Just line of sight!)
/// </summary>
public class AttackState : IState {
	
	public MovementUtil movement;
	public float turnSpeed = 10.0f;
	public GameObject Target;
	public float AttackDist = 15;
	public float SwarmPower = 2;

	// Use this for initialization
	public override void OnStart () {
		NextState = this;
	}
	
	// Update is called once per frame
	public override void OnUpdate () {
		GameObject player = Target;

		if ( Target == null ) 
		{
			//Target destroyed! What do we do.. FIND THE PLAYER?
			this.Target = GameObject.FindGameObjectWithTag("Player") as GameObject;
			this.NextState = this;
			return;
		}

		Vector3 playerPos = player.transform.position;

		//If the target gets out of range, we will return back to our previous state.
		if ( (playerPos - StateGameObject.transform.position).magnitude > AttackDist * 2 ) 
		{			
			IState previousState = this.ActionHistory.Pop();
			NextState = previousState;
		}
		
		Vector3 target = playerPos;

		var ai = gameObject.GetComponent<EnemyAIStateMachine>();
		Vector3 toTarget = playerPos - ai.weapon.transform.position;

		//Close enough fire!
		if ( toTarget.magnitude < AttackDist ) 
		{
			//FIRE!!!!
			var projObj = ai.weapon.Fire(ai.gameObject);

			//Bad guys have red trails!
			if ( projObj != null )
			{
				var trail = projObj.GetComponent<TrailRenderer>();
				if ( ai.TrailOverrideMaterial != null && trail != null )
				{
					if ( ai.TrailOverrideMaterial != null )
					{
						trail.material = ai.TrailOverrideMaterial;
					}
				}

				if ( ai.ProjectileOverrideMaterial != null && projObj.gameObject.renderer != null )
				{
					projObj.gameObject.renderer.material = ai.ProjectileOverrideMaterial;
				}

			}

			//Slow down, we are close enough, we'll circle the target a bit to confuze them!
			movement.SlowDown(2.0f);
			movement.ThrustersSide(toTarget.normalized, SwarmPower, true );
		}
		else 
		{
			//We're not close enough to fire, but we'll move on a slight angle so its harder to hit us!
			movement.ThrustersDir(toTarget, 10.0f);
			movement.ThrustersSide(toTarget.normalized, SwarmPower, true);
		}
	
		//Always try to face up:
		movement.StablizeToward(toTarget);

		movement.Update();		
	}
}
