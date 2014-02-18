using UnityEngine;
using System.Collections;

public class ForceBeam : Projectile {

	public float Power;

	private float scaleTime;
	private Vector3 scaleSize;
	
	public GameObject Explosion;
	
	public ForceBeam()
	{
		this.ExplodeOnImpacts = 1;
	}
	
	// Use this for initialization
	void Start () {
		base.Start();
		Physics.IgnoreLayerCollision( LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Debris"));
		Physics.IgnoreLayerCollision( LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Debris"));
		this.scaleSize = this.transform.localScale;
		this.transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	public override void Update () {
		if ( ExplodeOnImpacts > 0 || ExplodeOnImpacts < 0 ) 
		{
			ExplodeOnImpacts = 1;
		}

		this.scaleTime += Time.deltaTime;

		if ( this.scaleTime > 0.06f) 
		{
			this.transform.localScale = scaleSize;
		}
		else {
			this.transform.localScale = scaleSize * scaleTime / 0.06f;
		}



		base.Update();
		
		if ( Util.IsUiActive() )
		{
			return;
		}
	}
	
	public override void Trigger ()
	{
		//Only explode if it was triggered.
		if ( this.gameObject.activeSelf && time < TriggerTime )
		{
			if ( lastCollision != null )
			{
				Collider col = lastCollision.collider;
				CharacterMotor motor = col.gameObject.GetComponent<CharacterMotor>();

				Vector3 dist = col.gameObject.transform.position - this.transform.position;

				//If its a player, move it!
				if ( motor != null ) 
				{
					motor.SetVelocity( motor.movement.velocity + (dist.normalized * Power));
				}

				col.gameObject.SendMessageUpwards("TakeDamage", this.Damage, SendMessageOptions.DontRequireReceiver);

				
				Vector3 normal = this.rigidbody.velocity.normalized;
				Vector3 exploPoint = this.transform.position;
				

				normal = -lastCollision.contacts[0].normal;
				exploPoint = lastCollision.contacts[0].point;
				
				normal = Vector3.Lerp( normal, this.rigidbody.velocity.normalized, 0.5f).normalized;
				
				//UnityEditor.EditorApplication.isPaused = true;			
				
				var rot = Quaternion.FromToRotation(-Vector3.forward, normal );
				
				Instantiate( Explosion, exploPoint, rot);	
				this.gameObject.SetActive(false);
			}
		}
		DestroyObject(this.gameObject);
	}
}
