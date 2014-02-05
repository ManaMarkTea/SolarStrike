using UnityEngine;
using System.Collections;
using System.IO;

public class StartGame : MenuAction {

	public string Level;

	public TextAsset defaultLevel;

	public override void SelectAction (MenuItem item)
	{
		StreamWriter sw = new StreamWriter("Level.dat");
		sw.Write(defaultLevel.text);
		sw.Close();

		
		Application.LoadLevel(Level);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
