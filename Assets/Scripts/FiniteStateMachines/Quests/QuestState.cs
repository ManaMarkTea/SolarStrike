using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(QuestSystemState))]
public class QuestState : MonoBehaviour {

	public event System.Action<QuestState> OnComplete;
	public event System.Action<QuestState> OnQuestLogUpdated;

	public string QuestText;
	public bool Complete;


	// Use this for initialization
	public void Start () {
	
	}
	
	// Update is called once per frame
	public void Update () {
	
	}

	public void NotifyComplete() 
	{
		var handler = OnComplete;
		if ( handler != null)
		{
			handler(this);
		}
	}

	public void NotifyQuestLogUpdate() 
	{
		var handler = OnQuestLogUpdated;
		if ( handler != null)
		{
			handler(this);
		}
	}

}
