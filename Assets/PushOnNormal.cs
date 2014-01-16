using UnityEngine;
using System.Collections;

/// <summary>
/// Adds a force based on the local position of the object.
/// Useful for explosions.
/// </summary>
public class PushOnNormal : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.rigidbody.AddRelativeForce( this.transform.localPosition * 400 );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
