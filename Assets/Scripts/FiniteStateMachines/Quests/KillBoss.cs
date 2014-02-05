using UnityEngine;
using System.Collections;

public class KillBoss : QuestState {

	// Use this for initialization
	public override void OnStart () {
		
	}

	// Update is called once per frame
	public override void OnUpdate () {

		bool done = true;

		var enemies = GameObject.FindGameObjectsWithTag("Enemy");
		for ( int i = 0; i < enemies.Length; i++ )
		{
			var ai =  enemies[i].GetComponent<EnemyAIStateMachine>();

			if ( ai.name.Contains("Tazion_Harvestor")) {
				this.QuestText = baseQuestText + " Hull:" + ai.HP;
				NotifyQuestLogUpdate();
			}

			if ( ai != null )
			{
				if ( ai.Disabled ) continue;
			}

			done = false;
		}

		if ( done)
		{
			this.Complete = true;
			NotifyComplete();
		}

		
	}
}