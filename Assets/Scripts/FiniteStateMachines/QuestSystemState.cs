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
	public IState FailState;

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

	public bool AreQuestsFailed(int [] indicesToCheck)
	{
		bool fail = false;
		var quests = GetComponents<QuestState>();
		
		for ( int i = 0; i < indicesToCheck.Length; i++)
		{
			if ( i < quests.Length)
			{
				var quest = quests[i];
				if ( quest.Failed == true )
				{
					fail = true;
				}
			}
		}
		
		return fail;
	}

	public bool AreQuestsFailed()
	{
		bool fail = false;
		var quests = GetComponents<QuestState>();
		
		for ( int i = 0; i < quests.Length; i++)
		{
			var quest = quests[i];
			if ( quest.Failed == true )
			{
				fail = true;
			}
		}
		
		return fail;
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
				if ( quest.Complete && quest.Failed == false )
				{
					text = "<color=grey>" + text + " <i>(Complete)</i></color>";
				}
				else if ( quest.Complete && quest.Failed == true )
				{
					text = "<color=red>" + text + " <i>(FAILED)</i></color>";
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

		bool failed = false;
		bool allObjectivesDone = true;
		var quests = GetComponents<QuestState>();
		for ( int i = 0; i < quests.Length; i++)
		{
			var quest = quests[i];
			if ( quest.Failed ) {
				failed = true;
			}
			if ( quest.Complete == false )
			{
				allObjectivesDone = false;
			}
		}

		//CHEAT: Skip to the next state without a power cell!  The ship won't charge!
		if ( allObjectivesDone || Input.GetKeyDown(KeyCode.F1 ) )
		{
			if ( failed )
			{
				this.NextState = this.FailState;
			} 
			else 
			{
				this.NextState = this.FutureState;
			}

			this.AllQuestsComplete = true;
		}
		
	}
}
