using UnityEngine;
using System.Collections;

/// <summary>
/// State defend: In this state, the game will activate any spawners associated.
/// Once active, the game will wait until all the enemies have been spawned from those spawners.
/// Then the game will move on to the future state.
/// 
/// </summary>
public class StateDefend : IState {

	public int WaveNumber;
	public GameObject SpawnSet;
	
	public GUIText txtWaveInfo;

	// Use this for initialization
	public override void OnStart () {

		int enemies = 0;

		//Activate all spawners!
		for ( int i = 0; i < SpawnSet.transform.childCount; i++ )
		{
			SpawnSet.transform.GetChild(i).gameObject.SetActive(true);
			var spawner = SpawnSet.transform.GetChild(i).gameObject.GetComponent<EnemySpawner>();

			if ( spawner != null )
			{
				enemies += spawner.Count;
			}
		}

		if ( txtOutput != null )
		{
			txtOutput.text = "Defend the <color=yellow>power cell</color>!";		
		}

		if ( txtWaveInfo != null )
		{
			txtWaveInfo.text = "Wave " + WaveNumber + ": " + enemies +" enemies.";
		}

	}
	
	// Update is called once per frame
	public override void OnUpdate () {
		bool doneSpawning = true;



		//Check to see if they are done.
		var spawners = SpawnSet.GetComponentsInChildren<EnemySpawner>();
		for ( int i = 0; i < spawners.Length; i++ )
		{
			var spawner = spawners[i];
			if ( spawner.spawned != spawner.Count )
			{
				doneSpawning = false;
			}
		}

		//Done, lets move to the future state ( Or cheat with f1 )
		if ( doneSpawning || Input.GetKeyDown(KeyCode.F1 ) )
		{
			this.NextState = this.FutureState;
		}
	}
}
