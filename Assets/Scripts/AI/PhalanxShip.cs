using UnityEngine;
using System.Collections;

/// <summary>
/// The AI for the player's ship: "Phalanx ship".
/// </summary>
public class PhalanxShip : MonoBehaviour {

	public IWeapon PrimaryTurret;
	public GameObject PrimaryTurretPivot;
	public GameObject Explosion;

	public GUIText txtPowerLevel;

	public float PowerLevel;
	public BlockObject PowerCell;
	public float MinPowerLevel = 100;

	public float HP = 10000.0f;


	// Use this for initialization
	void Start () {
	
	}

	public void TakeDamage(float distance)
	{		

		HP -= distance;	
	}

	// Update is called once per frame
	void Update () {

		/* normally the state machine will tell us... but maybe we should just check ourselves?
		if ( PowerCell == null )
		{
			PowerCell = GenerateWorld.World.FindNearestBlockOfType(this.transform.position, "PowerCell");
		}
		*/

		//If we have found a power cell, use it to increase our power level:
		if ( PowerCell != null && PowerCell.HP > 0 )
		{
			this.PowerLevel += Time.deltaTime;
		}

		if ( HP == 0)
		{
			var pos = this.transform.position;
			Instantiate(Explosion, pos, Quaternion.identity);
			pos.y += 10;
			Instantiate(Explosion, pos, Quaternion.identity);
			pos.y -= 10;
			pos.x += 10;
			Instantiate(Explosion, pos, Quaternion.identity);
			pos.x -= 10;
			pos.z += 10;
			Instantiate(Explosion, pos, Quaternion.identity);
			Destroy(this.gameObject);

		}

		//CHEATER!!!!!!!!!!!
		if ( Input.GetKeyDown( KeyCode.F2) )
		{
			this.PowerLevel += 10;
		}
		if ( Input.GetKeyDown( KeyCode.F3) )
		{
			this.HP -= 1000;
		}

		//Update any text that we may have associated:
		if ( txtPowerLevel != null )
		{
			float percent = (PowerLevel / MinPowerLevel) * 100;
			if ( percent > 100 )
			{
				percent = 100;
			}

			txtPowerLevel.text = "Ship power: " + percent.ToString("0.0") + "%";
			txtPowerLevel.text += "\nHull power: " + HP.ToString("0.0");
		}

		//If we have enough power we will shoot:
		if ( PowerLevel > MinPowerLevel ) {
			var player = GameObject.FindGameObjectWithTag("Player");
			var pivot = PrimaryTurretPivot;

			//Exmaple: Aim at player:
			//var target = player.transform.position - pivot.transform.position;
			//pivot.transform.rotation = Quaternion.LookRotation( target.normalized, Vector3.up  );

			//Attack the next enemy:
			var enemies = GameObject.FindGameObjectsWithTag("Enemy");
			for ( int i = 0; i < enemies.Length; i++ )
			{
				//Check to make sure its active:
				var ai =  enemies[i].GetComponent<EnemyAIStateMachine>();
				if ( ai != null )
				{
					if ( ai.Disabled ) continue;
				}

				//Add some variance:
				var pos = enemies[i].transform.position;
				pos.y -= 1;
				pos.x += Random.value * 10 - 5;
				pos.y += Random.value * 10 - 5;
				pos.z += Random.value * 10 - 5;

				//Rotate to face that object (  )
				var target = pos - pivot.transform.position;
				var newRotation = Quaternion.LookRotation( target.normalized, Vector3.up  );
				float diff = Quaternion.Angle( pivot.transform.rotation, newRotation);
				pivot.transform.rotation = Quaternion.Slerp(pivot.transform.rotation, newRotation, Time.deltaTime * 3.0f);

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
