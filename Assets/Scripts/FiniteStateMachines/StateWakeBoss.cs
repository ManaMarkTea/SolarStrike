using UnityEngine;
using System.Collections;

/// <summary>
/// Game state: Wakes the boss!
/// </summary>
public class StateWakeBoss : IState {
	
	public GUIText txtTime;
	public EnemyAIStateMachine Enemy;
	
	// Use this for initialization
	public override void OnStart () {
		Enemy.Disabled = false;
		
		
	}
	
	// Update is called once per frame
	public override void OnUpdate () {

	}
}
