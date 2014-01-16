using UnityEngine;
using System.Collections;

/// <summary>
/// Game flow. Controls the flow of the game states. (THE GAME STATE MACHINE!)
/// </summary>
public class GameFlow : MonoBehaviour {

	public StateMachine stateMachine;
	private GameObject states;

	public IState StartState;

	//For cheating:
	private float lastGrav;

	// Use this for initialization
	void Start () {
		stateMachine = new StateMachine(this.gameObject);
		this.states = this.transform.FindChild("States").gameObject;

		var buildState = StartState;
		stateMachine.SetState( buildState );

		var waveInfo = GameObject.FindGameObjectWithTag("WaveTextUI");
		if ( waveInfo != null && waveInfo.guiText != null)
		{
			waveInfo.guiText.text = "";
		}

	}
	
	// Update is called once per frame
	void Update () {
		if ( stateMachine == null ) return;

		stateMachine.Update();

		//Lock and unlock the mouse:
		if ( Input.GetKeyUp(KeyCode.LeftAlt ) ) {
			if ( Screen.lockCursor ) {
				Screen.showCursor = true;
				Screen.lockCursor = false;
			} else {
				Screen.showCursor = false;
				Screen.lockCursor = true;
			}
			
		}

		
		//Switch between Walk and fly mode.  Its easier to build in fly mode.
		if ( Input.GetKeyDown(KeyCode.F5) )
		{
			var player = GameObject.FindGameObjectWithTag("Player");
			var motor = player.GetComponent<CharacterMotor>();
			if ( motor.movement.gravity != 0 ) {
				lastGrav = motor.movement.gravity;
				motor.movement.gravity = 0;
			}
			else 
			{
				motor.movement.gravity = lastGrav;
			}
		}

	}
}
