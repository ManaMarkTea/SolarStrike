using UnityEngine;
using System.Collections;

/// <summary>
/// Game state: BUILD. In this state, the game will wait for x minutes to go by.
/// Once the time expires it will advance to the future state.
/// </summary>
public class StateBuild : IState {

	/// <summary>
	/// The time until attack in minutes.
	/// </summary>
	public float TimeUntilAttackMinutes = 10;
	private float TimeRemainingUntilAttackMinutes;
	
	private static float PerMinute = 1.0f / 60.0f;

	// Use this for initialization
	public override void OnStart () {
		NextState = this;
		this.TimeRemainingUntilAttackMinutes = TimeUntilAttackMinutes;

	}	
	 
	// Update is called once per frame
	public override void OnUpdate () {

		float seconds = Time.deltaTime;
		float minutes = seconds * PerMinute;
		TimeRemainingUntilAttackMinutes -= minutes;

		System.TimeSpan time = System.TimeSpan.FromMinutes(TimeRemainingUntilAttackMinutes);

		if ( txtOutput != null )
		{
			txtOutput.text = "Time left until attack: " + time.Minutes + ":" + time.Seconds.ToString("00");
			txtOutput.text += "\nAll your base.... make your time...";
		}

		//F1 to skip this stage early.
		if ( TimeRemainingUntilAttackMinutes <= 0 || Input.GetKeyDown(KeyCode.F1 ) )
		{
			//Switch to the future state:
			NextState = this.FutureState;
			this.TimeRemainingUntilAttackMinutes = TimeUntilAttackMinutes;

		}

	}
}
