using UnityEngine;
using System.Collections;
using System.IO;

public class LoadGame : MenuAction {
	
	public string Level;
	public TextAsset defaultLevel;
	
	public override void SelectAction (GMenuItem item)
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
		GenerateWorld.LevelToLoad = filename;
		#endif

		#if UNITY_WEBPLAYER
		if ( PlayerPrefs.HasKey("LevelData") == false )
		{
			PlayerPrefs.SetString("LevelData", defaultLevel.text);
		}
		#else
		if ( System.IO.File.Exists("Level.txt") == false )
		{
			File.WriteAllText(defaultLevel.text);
		}
		#endif

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