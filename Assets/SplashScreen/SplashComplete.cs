using UnityEngine;
using System.Collections;

public class SplashComplete : MonoBehaviour {

	public int Level;

	public void Complete() 
	{
		Application.LoadLevel(Level);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if ( Input.GetButton("Fire1") )
		{
			Complete();
		}

	}



}
