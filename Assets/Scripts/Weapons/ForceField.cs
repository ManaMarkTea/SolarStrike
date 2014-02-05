using UnityEngine;
using System.Collections;

public class ForceField : Projectile {
		
    public float Radius;
	public float Power;

	
	public GameObject Explosion;

	public ForceField()
	{
		this.ExplodeOnImpacts = 3;
	}

	// Use this for initialization
	void Start () {
		Physics.IgnoreLayerCollision( LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Debris"));
		Physics.IgnoreLayerCollision( LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Debris"));
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();

		if ( Util.IsUiActive() )
		{
			return;
		}
		
		if (Input.GetButtonDown("Fire2"))
        {			
			Trigger();
        }
	}
	
	public override void Trigger ()
	{
		//Only explode if it was triggered.
		if ( time < TriggerTime )
		{
			RaycastHit hit;			
			var results = Physics.OverlapSphere( this.transform.position, Radius);
			
			for ( int i = 0; i < results.Length; i++) 
			{
				Collider col = results[i];
				CharacterMotor motor = col.gameObject.GetComponent<CharacterMotor>();
				if ( motor != null ) 
				{
					Vector3 dir = col.gameObject.transform.position - this.transform.position;
					float scale = 1.0f - (dir.magnitude/Radius);
					float power = scale * Power;
					
					motor.SetVelocity( dir.normalized * power);
				}
											
				Vector3 dist = col.gameObject.transform.position - this.transform.position;
				float mag = 1.0f - (dist.magnitude / Radius);
				if ( mag < 0 ) mag = 1.0f;
				float damage = mag * Damage;
				
				col.gameObject.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
				
			}
			
			Vector3 normal = this.rigidbody.velocity.normalized;
			Vector3 exploPoint = this.transform.position;
			
			if ( lastCollision != null )
			{
				normal = -lastCollision.contacts[0].normal;
				exploPoint = lastCollision.contacts[0].point;
				
				normal = Vector3.Lerp( normal, this.rigidbody.velocity.normalized, 0.5f).normalized;
				
				//UnityEditor.EditorApplication.isPaused = true;
			}
			
			var rot = Quaternion.FromToRotation(-Vector3.forward, normal );
			
			Instantiate( Explosion, exploPoint, rot);							
		}
		DestroyObject(this.gameObject);
	}
}
