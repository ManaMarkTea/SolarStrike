using UnityEngine;
using System.Collections;

/// <summary>
/// Gives player all weapons. (Cheating!)
/// </summary>
public class GivePlayerAllWeapons : IActivate {

	public string[] WeaponsToGive;

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
		weapons.PickUpAllWeapons();
	}
	
}
