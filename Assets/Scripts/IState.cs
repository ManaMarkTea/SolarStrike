using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class IState : MonoBehaviour
{	
	public GameObject StateGameObject;
	public IState NextState;
	public IState FutureState;
	public Stack<IState> ActionHistory;

	public GUIText txtOutput;
	public string txtOuputTag;
	
	public IState()
	{
		ActionHistory = new Stack<IState>();
	}
	
	public void Start() 
	{
		this.NextState = this;
		OnStart();
	}
	
	public void Update() 
	{
		//Find the tag if we must!
		if ( txtOutput == null && txtOuputTag != null )
		{
			var obj = GameObject.FindGameObjectWithTag(txtOuputTag);
			if ( obj != null )
			{
				txtOutput = obj.GetComponent<GUIText>();
			}		
		}

		OnUpdate();

	}

	//Template Method Pattern:
	public abstract void OnStart();	
	public abstract void OnUpdate();	

}