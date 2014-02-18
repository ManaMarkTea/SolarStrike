using System;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{	    
	public float Damage;	
	public GameObject ProjectilePrefab;
	public float MaxSpeed = 50;
	public float MinSpeed = 30;
	public float TriggerTime = 7.0f;
	protected float time;

	public int ExplodeOnImpacts = -1;
	public Collision lastCollision;
	private int collisions;

	public GameObject TrackObject;
	public bool Tracking;

	public abstract void Trigger();	
	public virtual bool IsProjectile() { return true; }

	public virtual void OnTriggerEnter( Collider collider )
	{

	}
						
	public virtual void OnCollisionEnter(Collision collision)
	{
		if ( collision.gameObject.tag == "Projectile" )
		{

			return;
		}

		if ( ExplodeOnImpacts > 0 ) 
		{
			collisions++;
			lastCollision = collision;

			if ( collisions >= ExplodeOnImpacts )
			{
				Trigger();
			}
		}
		else 
		{
			this.rigidbody.useGravity = true;
		}


	}

	public virtual void Start() 
	{

	}

	public virtual void Update() {	
		time += Time.deltaTime;
		
		if ( time > TriggerTime ) {
			Trigger();
		}

		if ( this.rigidbody != null )
		{
			var vel =  this.rigidbody.velocity;


			float mag = vel.magnitude;
			float speed = mag;

			if ( mag != 0 ) 
			{
				vel /= mag;	
			}

			if ( speed != 0 && speed > MaxSpeed )
			{
				speed = MaxSpeed;
			}
			
			if ( speed <= MinSpeed )
			{
				speed = MinSpeed;
			}

			if ( Tracking && TrackObject != null )
			{
				var newDir = TrackObject.transform.position - this.transform.position;
				newDir.Normalize();

				vel = Vector3.Lerp( vel, newDir, 0.2f);
			}

			vel *= speed;
			this.rigidbody.velocity = vel;

		}
		
	}
	
}


