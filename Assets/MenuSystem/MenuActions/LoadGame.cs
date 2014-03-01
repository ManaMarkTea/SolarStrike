using UnityEngine;
using System.Collections;
using System.IO;

public class LoadGame : MenuAction {
	
	public string Level;
	public TextAsset defaultLevel;
	
	public override void SelectAction (MenuItem item)
	{
		string filename = "Level.txt";

		#if UNITY_EDITOR
		var curLock = Screen.lockCursor;
		var curShow = Screen.showCursor;
		Screen.lockCursor = false;
		Screen.showCursor = true;
		filename = UnityEditor.EditorUtility.OpenFilePanel("Level to load", Application.dataPath, "txt");
		Screen.lockCursor = curLock;
		Screen.showCursor = curShow;
		#endif

		if ( System.IO.File.Exists(filename) == false )
		{
			StreamWriter sw = new StreamWriter(filename);
			sw.Write(defaultLevel.text);
			sw.Close();
		} else {
			GenerateWorld.LevelToLoad = filename;
		}

		Application.LoadLevel(Level);
		item.enabled = false;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}