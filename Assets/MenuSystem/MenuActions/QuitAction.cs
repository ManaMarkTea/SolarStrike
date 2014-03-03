using UnityEngine;
using System.Collections;

public class QuitAction : MenuAction {
	public static string webplayerQuitURL = "http://www.GhostTap.com";

	public override void SelectAction (MenuItem item)
	{	
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#elif UNITY_WEBPLAYER
		Application.OpenURL(webplayerQuitURL);
		#else
		Application.Quit();
		#endif

	}

}
