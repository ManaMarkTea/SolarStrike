using UnityEngine;
using System.Collections;

/// <summary>
/// AI for the Turret
/// </summary>
public class TurretAI : MonoBehaviour {
	public IWeapon PrimaryTurret;
	public GameObject PrimaryTurretPivot;

	private bool lastArmedState;

	public GUIText txtPowerLevel;
	
	public float PowerLevel;
	public BlockObject PowerCell;
	public float MinPowerLevel = 100;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//If the turret is unarmed, have it face downwards.
		if ( PrimaryTurret != null && PrimaryTurret.Armed == false )
		{
			lastArmedState = false;
			if ( PrimaryTurretPivot != null )
			{
				PrimaryTurretPivot.transform.rotation = Quaternion.Euler(60,0,0);
			}
			return;
		}

		if ( PrimaryTurretPivot != null && PrimaryTurret.Armed && lastArmedState == false )
		{
			lastArmedState = PrimaryTurret.Armed;
			//Look forward if its just armed:
			PrimaryTurretPivot.transform.rotation = Quaternion.identity;
		}

		//Power level?
		if ( PowerCell != null && PowerCell.HP > 0 )
		{
			this.PowerLevel += Time.deltaTime;
		}

		//CHEAT!
		if ( Input.GetKeyDown( KeyCode.F2) )
		{
			this.PowerLevel += 20;
		}
		
		//Update any text that we might need.
		if ( txtPowerLevel != null )
		{
			float percent = (PowerLevel / MinPowerLevel) * 100;
			if ( percent > 100 )
			{
				percent = 100;
			}
			
			txtPowerLevel.text = "Ship power: " + percent.ToString("0.0") + "%";
			
		}

		//Fire the turret at the near by enemies.
		if ( PowerLevel >= MinPowerLevel ) {
			var player = GameObject.FindGameObjectWithTag("Player");
			var pivot = PrimaryTurretPivot;
			
			//Exmaple: Aim at player:
			//var target = player.transform.position - pivot.transform.position;
			//pivot.transform.rotation = Quaternion.LookRotation( target.normalized, Vector3.up  );
			
			
			var enemies = GameObject.FindGameObjectsWithTag("Enemy");
			for ( int i = 0; i < enemies.Length; i++ )
			{
				var ai =  enemies[i].GetComponent<EnemyAIStateMachine>();
				if ( ai != null )
				{
					if ( ai.Disabled ) continue;
				}
				
				
				var pos = enemies[i].transform.position;
				pos.y -= 1;
				pos.x += Random.value * 4 - 2;
				pos.y += Random.value * 4 - 2;
				pos.z += Random.value * 4 - 2;
				
				//Look toward the enemy:
				var target = pos - pivot.transform.position;
				var newRotation = Quaternion.LookRotation( target.normalized, Vector3.up  );
				float diff = Quaternion.Angle( pivot.transform.rotation, newRotation);
				pivot.transform.rotation = Quaternion.Slerp(pivot.transform.rotation, newRotation, Time.deltaTime * 3.0f);

				//If we're almost aimed at our target, we can start to fire.
				if ( diff < 4 )
				{
					//FIRE!
					var projObj = PrimaryTurret.Fire(pivot);
					break;
				}
				break;
			}
		}
		
	}
}
