using UnityEngine;
using System.Collections;
using System.IO;

public class StartGame : MenuAction {

	public string Level;
	public string DefaultLevelData;

	public override void SelectAction (MenuItem item)
	{
		var sr = new StreamReader(Application.dataPath + System.IO.Path.DirectorySeparatorChar +  DefaultLevelData);		
		var fileContents = sr.ReadToEnd();
		sr.Close();

		StreamWriter sw = new StreamWriter("Level.dat");
		sw.Write(fileContents);
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
