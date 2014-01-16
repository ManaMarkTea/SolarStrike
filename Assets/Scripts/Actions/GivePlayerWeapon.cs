using UnityEngine;
using System.Collections;

/// <summary>
/// Gives the player weapon by name.
/// Kind of error prone as you could misspell the name! Enums? Lookups?
/// </summary>
public class GivePlayerWeapon : IActivate {

	public string WeaponToGive;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public override void Activate ()
	{
		var player = GameObject.FindGameObjectWithTag("Player");
		var weapons = player.GetComponent<WeaponScript>();
		weapons.PickUpWeapon(WeaponToGive);
	}
	
}
