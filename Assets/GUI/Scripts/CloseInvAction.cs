using UnityEngine;
using System.Collections;

public class CloseInvAction : MenuAction {

	public Inventory InvContainer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void SelectAction (MenuItem item)
	{
		InvContainer.visible = !InvContainer.visible;
		
		if ( InvContainer.visible ) {
			Screen.showCursor = true;
			Screen.lockCursor = false;
		} else {
			Screen.showCursor = false;
			Screen.lockCursor = true;
		}

	}

}
