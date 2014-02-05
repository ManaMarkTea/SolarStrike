using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(QuestSystemState))]
public abstract class QuestState : MonoBehaviour {

	public event System.Action<QuestState> OnComplete;
	public event System.Action<QuestState> OnQuestLogUpdated;

	public string QuestText;
	public bool Complete;
	public bool Failed;
	protected string baseQuestText;

	// Use this for initialization
	public virtual void Start () {
		this.baseQuestText = QuestText;
		OnStart();
	}
	
	// Update is called once per frame
	public virtual void Update () {
		OnUpdate();
	}

	//Template Method Pattern:
	public abstract void OnStart();
	public abstract void OnUpdate();


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
