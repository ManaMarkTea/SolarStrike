using UnityEngine;
using System.Collections;
using System.IO;

public class ResumeGame  : MenuAction {
	
	public string Level;
	public TextAsset defaultLevel;

	public override void SelectAction (MenuItem item)
	{
		if ( System.IO.File.Exists("Level.txt") == false )
		{
			StreamWriter sw = new StreamWriter("Level.txt");
			sw.Write(defaultLevel.text);
			sw.Close();
		}

		Application.LoadLevel(Level);
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
