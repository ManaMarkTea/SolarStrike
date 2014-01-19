using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public RandomSound ExplosionSound;
	private bool firstUpdate = true;

	// Use this for initialization
	void Start () {

	}


	// Update is called once per frame
	void Update () {
		if ( firstUpdate && ExplosionSound != null )
		{
			firstUpdate = false;
			ExplosionSound.PlayRandom();
		}
	}
}
