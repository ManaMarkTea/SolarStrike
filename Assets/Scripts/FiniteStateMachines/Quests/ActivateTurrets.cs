using UnityEngine;
using System.Collections;

public class ActivateTurrets  : QuestState {

	public int AmountOfTurretsNeeded = 2;
	private int counted;
	private string baseQuestText;

	// Use this for initialization
	void Start () {
		this.counted = 0;
		this.baseQuestText = QuestText;
	}
	
	// Update is called once per frame
	void Update () {
		
		//TODO: Add an event for item pickup instead of scanning.
		
		if ( Complete == false )
		{
			int newCount = 0;

			var turrets = GenerateWorld.World.FindBlocksOfType("Turret");
			for ( int i = 0; i < turrets.Count; i++ )
			{
				var turret = turrets[i].GetComponentInChildren<IWeapon>();
				if ( turret != null && turret.Armed ) { newCount++; }
			}

			if ( newCount != counted )
			{
				counted = newCount;

				if ( counted == AmountOfTurretsNeeded){
					this.Complete = true;
					this.QuestText = baseQuestText + " (" + counted + " / " + AmountOfTurretsNeeded + " )";
					NotifyComplete();
				}
				else
				{
					this.QuestText = baseQuestText + " (" + counted + " / " + AmountOfTurretsNeeded + " )";
					NotifyQuestLogUpdate();
				}
			}
		}
		
	}
}
