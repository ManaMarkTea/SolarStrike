using UnityEngine;
using System.Collections;

public class PlanetSpin : MonoBehaviour {

	public float SpinSpeed = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.transform.Rotate(0, Time.deltaTime * SpinSpeed, 0);
	}
}
