using UnityEngine;
using System.Collections;

/// <summary>
/// Patrol state will cause the object to go through the waypoints one by one.
/// Once an object is within range of a target, it will switch to attack mode.
/// </summary>
public class PatrolState : IState {

	public bool LoopWayPoints;
	public Vector3[] WayPoints;
	private int currentWaypoint;
	public MovementUtil movement;
	public GameObject Target;
	public float AttackDist = 15;

	public PatrolState() 
	{
		
	}
	
	// Use this for initialization
	public override void OnStart () {
		NextState = this;

		//We should always have somewhere to go.
		if ( WayPoints == null || WayPoints.Length == 0 )
		{
			WayPoints = new Vector3[1];
			WayPoints[0] = new Vector3(0,0,0);
			this.currentWaypoint = 0;
		}

	}
	
	// Update is called once per frame
	public override void OnUpdate () {
		GameObject player = Target;

		if ( Target == null ) 
		{
			//Target destroyed! What do we do.. FIND THE PLAYER?
			this.Target = GameObject.FindGameObjectWithTag("Player") as GameObject;
			this.NextState = this;

			WayPoints = new Vector3[]{ Target.transform.position }; 
			this.currentWaypoint = 0;
			return;
		}

		Vector3 playerPos = player.transform.position;

		//Close enough to attack, switch modes, put ourselves in the stack so we can resume later.
		if ( (playerPos - StateGameObject.transform.position).magnitude < AttackDist ) 
		{
			NextState = this.gameObject.GetComponent<AttackState>();
			(NextState as AttackState).movement = this.movement;
			(NextState as AttackState).Target = this.Target;

			this.ActionHistory.Push(this);
			NextState.ActionHistory = this.ActionHistory;
		}

		//Move toward a waypoint
		if ( currentWaypoint < WayPoints.Length)
		{		
			Vector3 waypoint = WayPoints[currentWaypoint];
			Vector3 toWaypoint = waypoint - StateGameObject.transform.position;
			float dist = toWaypoint.magnitude;

			//Close enough to consider us there.
			if ( dist < 3 ) 
			{
				//We are at the end, lets stop moving.
				if ( LoopWayPoints == false && currentWaypoint == WayPoints.Length - 1 )
				{
					movement.SlowDown(12.5f);
				}

				currentWaypoint++;
				if ( LoopWayPoints )
				{
					currentWaypoint %= WayPoints.Length;
				}
				else if ( currentWaypoint >= WayPoints.Length  )
				{
					currentWaypoint = WayPoints.Length - 1;
				}
			}
			else if ( dist < 10 )
			{
				//Useful incase we overshot our target, we'll slow down a bit as we get closer.
				movement.SlowDown(2.5f);
			}


			movement.ThrustersDir( toWaypoint, 10.0f);
			movement.StablizeToward(toWaypoint);

		}
		
		
		movement.Update();
		
	}
}
