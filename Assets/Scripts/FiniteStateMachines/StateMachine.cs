using System;
using UnityEngine;


/// <summary>
/// State machine.
/// This class controls the current state. 
/// If the current state's "Next state" changes, that will become the new active state.
/// 
/// If the states are associated as objects then they will be hidden as shown inaddition to being disabled.
/// 
/// If the states are components of a single object, the states will be disabled as the current state changes.
/// 
/// </summary>
public class StateMachine
{
	public IState currentState;
	public GameObject gameObject;

	public StateMachine (GameObject gameObject)
	{
		this.gameObject = gameObject;
	}

	public void SetState( IState stateObject )
	{
		if ( this.currentState != null )
		{
			//If the state is located on a child, this will enable and disable that child ( useful for custom GUI text per state )
			if ( this.currentState.gameObject != gameObject)
			{
				this.currentState.gameObject.SetActive(false);
			}

			//If the state is just a component of the same object we'll turn it off now.
			this.currentState.enabled = false;
		}

		//Sets the new state:
		this.currentState = stateObject;

		//If the state is located on a child, this will enable and disable that child ( useful for custom text per state )
		if ( this.currentState.gameObject != gameObject)
		{
			this.currentState.gameObject.SetActive(true);
		}

		//If the state is just a component of the same object we'll turn it on now.
		this.currentState.enabled = true;

		//This is the root object that contains the state machine:
		this.currentState.StateGameObject = this.gameObject;

	}

	public void Update()
	{
		//Check to see if we need to change the state.
		IState nextState = currentState.NextState;
		
		if ( currentState != nextState && nextState != null )
		{
			nextState.NextState = nextState;
			SetState(nextState);
			currentState = nextState;
		}


	}

}




public class StateMachine
{
	public IState currentState;

	public void SetState( IState newState )
	{
		//Sets the new state:
		this.currentState.enabled = false;
		this.currentState = newState;
		this.currentState.OnStart();
		this.currentState.enabled = true;
	}
	
	public void Update()
	{
		//Check to see if we need to change the state.
		IState gotoState = currentState.NextState;
		
		if ( currentState != gotoState && gotoState != null )
		{
			//Goto next state:
			SetState(gotoState);
		}
	}
}







