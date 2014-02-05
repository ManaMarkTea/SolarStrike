using UnityEngine;
using System.Collections;

public class ResumeGame  : MenuAction {
	
	public string Level;
	
	public override void SelectAction (MenuItem item)
	{
		Application.LoadLevel(Level);
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
