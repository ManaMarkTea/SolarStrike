using UnityEngine;
using System.Collections;
using System.IO;

public class StartGame : MenuAction {

	public string Level;

	public TextAsset defaultLevel;

	public override void SelectAction (GMenuItem item)
	{
		#if UNITY_WEBPLAYER
		PlayerPrefs.SetString("LevelData", defaultLevel.text);
		#else
		File.WriteAllText(defaultLevel.text);
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
