using UnityEngine;
using System.Collections;

public class StayAlive : QuestState {
	
	// Use this for initialization
	public override void OnStart () {
		
	}
	
	// Update is called once per frame
	public override void OnUpdate () {
		
		bool done = false;
		
		var ship = GameObject.FindGameObjectsWithTag("PhalanxShip");
		for ( int i = 0; i < ship.Length; i++ )
		{
			var ai =  ship[i].GetComponent<PhalanxShip>();
			if ( ai.HP <= 0 )
			{
				done = true;
			}
		}
		
		if ( done)
		{
			this.Failed = true;
			this.Complete = true;
			NotifyComplete();
		}
		
		
	}
}