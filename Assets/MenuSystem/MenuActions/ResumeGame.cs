using UnityEngine;
using System.Collections;
using System.IO;

public class ResumeGame  : MenuAction {
	
	public string Level;
	public TextAsset defaultLevel;

	public override void SelectAction (MenuItem item)
	{
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
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
