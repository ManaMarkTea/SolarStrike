
using UnityEngine;
using System.Collections;

/// <summary>
/// Billboards an object to look @ the camera at all times, plus an extra rotation if you want it.
/// </summary>
public class Billboard : MonoBehaviour {

	public float extraRX = -90;
	public float extraRY = 0;
	public float extraRZ = 0;



	// Update is called once per frame
	void Update () {
		transform.LookAt(Camera.main.transform.position, -Vector3.up);
		transform.Rotate(extraRX, extraRY, extraRZ);
	}
}
