using UnityEngine;
using System.Collections;

/// <summary>
/// Game state: State Trigger.  This state will wait until a certain condition is met
/// before advancing to the Future state.
/// 
/// In this particular case, it waits for the world to contain a PowerCell.
/// </summary>
public class QuestSystemState : IState {

	public bool AllQuestsComplete;

	// Use this for initialization
	public override void OnStart () {
		var quests = GetComponents<QuestState>();

		for ( int i = 0; i < quests.Length; i++)
		{
			var quest = quests[i];
			quest.OnComplete += (obj) => {
				SyncQuestText();
			};

			quest.OnQuestLogUpdated += (obj) => {
				SyncQuestText();
			};
		}

		SyncQuestText();
	}

	public bool AreQuestsComplete(int [] indicesToCheck)
	{
		bool allDone = true;
		var quests = GetComponents<QuestState>();
		
		for ( int i = 0; i < indicesToCheck.Length; i++)
		{
			if ( i < quests.Length)
			{
				var quest = quests[i];
				if ( quest.Complete == false )
				{
					allDone = false;
				}
			}
		}

		return allDone;
	}

	public void SyncQuestText()
	{
		var quests = GetComponents<QuestState>();
		for ( int i = 0; i < quests.Length; i++)
		{
			var quest = quests[i];
			if ( txtOutput != null)
			{
				string text = quest.QuestText;
				if ( quest.Complete )
				{
					text = "<color=grey>" + text + " <i>(Complete)</i></color>";
				}

				if ( i == 0 )
				{
					txtOutput.text = text;
				}
				else 
				{
					txtOutput.text += "\n" + text;
				}
				
				//"Find a <b><color=yellow>Pulse Rifle</color></b>!";
				//txtOutput.text += "\nActivate <b><color=yellow>Defense Turrets</color></b>!";
				//txtOutput.text += "\nFind and defend the <b><color=yellow>Power Cell</color></b>!";
				//txtOutput.text += "\nRecharge the <b><color=yellow>Phalanx Ship</color></b>!";
			}
		}
	}

	// Update is called once per frame
	public override void OnUpdate () {

		bool allObjectivesDone = true;
		var quests = GetComponents<QuestState>();
		for ( int i = 0; i < quests.Length; i++)
		{
			var quest = quests[i];
			if ( quest.Complete == false )
			{
				allObjectivesDone = false;
			}
		}

		//CHEAT: Skip to the next state without a power cell!  The ship won't charge!
		if ( allObjectivesDone || Input.GetKeyDown(KeyCode.F1 ) )
		{
			this.NextState = this.FutureState;
			this.AllQuestsComplete = true;
		}
		
	}
}
