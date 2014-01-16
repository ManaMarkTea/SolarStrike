using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract class. Can be attached to things and then the script is performed.
/// Example: when you look at a block and press the "use" key, the block's IActivate class is called:
/// Give weapons, turn turrets on and off.
/// </summary>
public abstract class IActivate : MonoBehaviour {

	public abstract void Activate();


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
