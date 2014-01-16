using UnityEngine;
using System.Collections;

public class WaitForQuestSetComplete : IState {

	public QuestSystemState QuestsToWaitFor;
	public int[] QuestsNeeded;

	// Use this for initialization
	public override void OnStart () {
		if ( txtOutput != null )
		{
			txtOutput.text = "";
		}
	}

	// Update is called once per frame
	public override void OnUpdate () {

		//CHEAT: Skip to the next state without finishing all quests!!

		if ( QuestsToWaitFor == null || QuestsToWaitFor.AreQuestsComplete(QuestsNeeded) || Input.GetKeyDown(KeyCode.F1 ) )
		{
			this.NextState = this.FutureState;
		}
	}
}
