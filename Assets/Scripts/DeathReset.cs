using UnityEngine;
using System.Collections;

/// <summary>
/// Death reset. Saves the player from certain DOOM!
/// Beams the player back to a given location.
/// </summary>
public class DeathReset : MonoBehaviour {

	public GameObject SavingParticles;


	public float HeightCutOff = -1000.0f;

	public float TeleportTime = 5.0f;
	private float TeleportTimeRemaining;
	private bool Teleporting;
	private GameObject Teleporter;

	private Vector3 StartPoint;
	private Quaternion StartOrientation;
	
	public Transform PlayerSpawn;

	
	// Use this for initialization
	void Start () {

		this.TeleportTimeRemaining = TeleportTime;

		this.StartPoint = PlayerSpawn.transform.position;
		this.StartOrientation = PlayerSpawn.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {

		//If they go out of bounds, beam them back!
		if (this.transform.position.y < HeightCutOff)
        {
			this.TeleportTimeRemaining -= Time.deltaTime;
			if ( this.Teleporting == false )
			{
				//Create a teleport effect around the player:
				this.Teleporting = true;
				if ( SavingParticles != null )
				{
					Teleporter = Instantiate(SavingParticles) as GameObject;
					Teleporter.transform.parent = this.gameObject.transform;
					Teleporter.transform.localPosition = SavingParticles.transform.localPosition;
					Teleporter.transform.localRotation = SavingParticles.transform.rotation;

				}
			}

			//Increase the particles as we get closer to finishing the teleportation.
			if ( this.Teleporter != null )
			{
				float percent = TeleportTimeRemaining / TeleportTime;
				float alpha = 1.0f - percent;
				float speed = alpha * 100;
				this.Teleporter.particleSystem.startSize = (speed * 0.1f) + 1.0f;
				this.Teleporter.particleSystem.emissionRate = speed;
			}

			//Kill the effect!  (Or should we kill it off slowly?
			if ( this.TeleportTimeRemaining <= 0 )
			{
				this.TeleportTimeRemaining = this.TeleportTime;
				
				if ( this.Teleporter != null )
				{
					DestroyObject(this.Teleporter);
					this.Teleporting = false;
				}

				this.transform.position = StartPoint;
				this.transform.rotation = StartOrientation;

				CharacterMotor  motor = this.GetComponent<CharacterMotor>();
				motor.SetVelocity(Vector3.zero);

			}
        }

	}
}
