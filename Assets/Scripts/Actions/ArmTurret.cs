using UnityEngine;
using System.Collections;

/// <summary>
/// Arms the nearest turret.
/// </summary>
public class ArmTurret  : IActivate {
	
	public string[] WeaponsToGive;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
	public override void Activate ()
	{
		//Finds a nearby turret and arms it if its within range:
		var turret = GenerateWorld.World.FindNearestBlockOfType(this.gameObject.transform.position, "Turret");
		if ( turret != null )
		{
			var dist = this.gameObject.transform.position - turret.transform.position;
			if ( dist.magnitude < 10 )
			{
				var weapon = turret.GetComponentInChildren<IWeapon>();
				if ( weapon != null) 
				{
					weapon.Armed = !weapon.Armed;
				}
			}
		}
	}

}
