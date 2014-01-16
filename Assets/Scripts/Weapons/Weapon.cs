using UnityEngine;
using System.Collections;

public class Weapon : IWeapon {
	
	public GameObject Projectile;
	public GameObject SecondaryProjectile;

	public float Speed;
	public float AutoExploDistance = 15;
	public IFLTexture CrossHairIFL;

	public float MaxCharge;
	public float ChargePerSecond;
	public GameObject ChargeUI;

	private float currentCharge;



	// Use this for initialization
	public virtual void Start () {

		this.ChargeUI = GameObject.FindGameObjectWithTag("ChargeUI");

		var multiples = GameObject.FindGameObjectsWithTag("ChargeUI");


	}


	// Update is called once per frame
	public virtual void Update () {

		if ( ChargeUI != null && MaxCharge != 0 )
		{
			var scale = this.ChargeUI.transform.localScale ;
			scale.x = currentCharge / MaxCharge;

			if ( isPlayerWeapon )
			{
				this.ChargeUI.transform.localScale = scale;
			}

			var system = this.GetComponentInChildren<ParticleSystem>();
			if ( system != null )
			{
				system.emissionRate = scale.x * 200;
			}

		}

		if ( isPlayerWeapon && Util.IsUiActive() )
		{
			return;
		}


		if ( CurrentCoolDown > 0.0f ) 
		{
			CurrentCoolDown -= Time.deltaTime;
		}

		if ( CurrentCoolDown < 0 ) 
		{
			CurrentCoolDown = 0.0f;
		}

		RaycastHit? hit = AimDistance(2000);

		if ( hit != null )
		{
			int color = 1;
			if ( hit.Value.distance < AutoExploDistance )
			{
				color = 2;
			}

			//Enemies == Red
			if ( hit.Value.collider.gameObject.GetBlock() == null ) 
			{
				color = 1;
			}

			if ( CrossHairIFL != null ) { CrossHairIFL.TextureIndex = color; }
		}
		else 
		{
			if ( CrossHairIFL != null ) { CrossHairIFL.TextureIndex  = 0; }
		}

	}
	
	public GameObject GetChild( string name )
	{
		GameObject output = null;
		
	   	Component[] transforms = GetComponentsInChildren( typeof( Transform ), true );
	   	foreach( Transform transform in transforms )
	   	{
			if( transform.gameObject.name == name )
	     	{
	        	 output = transform.gameObject;
	      	}
	   	}
	   	return output;
	}
	
	public override void PutAway ()
	{
		
	}

	public RaycastHit? AimDistance( float maxDist) 
	{
		RaycastHit hit;
		Ray cast = getRayCast();
		
		int everythingButPlayer = 1 << LayerMask.NameToLayer("Player") |  1 << LayerMask.NameToLayer("Projectile");
		everythingButPlayer = ~everythingButPlayer;
		
		if ( Physics.Raycast( cast, out hit, maxDist, everythingButPlayer )) return hit;


		if ( Physics.CapsuleCast( cast.origin, cast.origin, 2.0f, cast.direction, out hit, maxDist, LayerMask.NameToLayer("Enemy")) )
		{
			return hit;
		}

		return null;
	}

	private GameObject LaunchProjectile( GameObject launchFrom, GameObject projectile)
	{
		// Instantiate the projectile at the position and rotation of this transform
		var spawn = getShootPoint();
		var cameraView = launchFrom.transform.FindChild("Main Camera");



		var cam = launchFrom.gameObject;
		
		if ( cameraView != null ) {
			cam = cameraView.gameObject;
		}
		
		var FireSound = GetComponent<AudioSource>();
		if ( FireSound != null )
		{
			FireSound.pitch = UnityEngine.Random.value * 0.2f + 0.9f;
			FireSound.Play();
			
			
		}
		
		GameObject clone =  Instantiate(projectile, spawn.transform.position, transform.rotation) as GameObject;
		clone.SetActive(true);
		CurrentCoolDown = CoolDown;

		RaycastHit? hit = AimDistance(2000);
		
		if ( isPlayerWeapon && hit != null )
		{
			var proj = clone.GetComponent<Projectile>();
			proj.TrackObject = hit.Value.collider.gameObject;
			proj.Tracking = true;
		}

		if ( launchFrom.collider != null)
		{
			Collider[] colliders = clone.GetComponentsInChildren<Collider>();
			if ( colliders.Length > 0 )
			{
				Collider cloneCollider = colliders[0];
				Physics.IgnoreCollision(cloneCollider, launchFrom.collider);

				cloneCollider.gameObject.tag = "Projectile";
			}
		}
		
		
		// Gve the cloned object an initial velocity along the current
		// object's Z axis
		clone.rigidbody.velocity = cam.transform.TransformDirection(Vector3.forward * Speed);

		return clone;
	}


	public override GameObject Fire (GameObject Player)
	{	
		if ( Armed == false ) { return null; }
		if ( isPlayerWeapon && Util.IsUiActive() ) { return null; }
		if ( CurrentCoolDown > 0.0f ) { return null; }

		var clone = LaunchProjectile(Player, Projectile);
		return clone;
	}	

	private GameObject lastGranade;

	public override GameObject Fire2 (GameObject Player)
	{	
		if ( Armed == false ) { return null; }
		if ( Util.IsUiActive() ) { return null; }
		if ( CurrentCoolDown > 0.0f ) { return null; }
		if ( SecondaryProjectile == null ) { return null; }

		GameObject clone = null;

		if ( lastGranade == null && Input.GetButtonDown("Fire2") )
		{
			clone = LaunchProjectile(Player, SecondaryProjectile);
			lastGranade = clone;
		}

		return clone;
	}
	
	public override GameObject Fire3 (GameObject Player)
	{		
		if ( Armed == false ) { return null; }
		//Primary Weapon CHARGED.

		if ( currentCharge < MaxCharge )
		{
			currentCharge += Time.deltaTime * ChargePerSecond;
		}

		if ( currentCharge > MaxCharge ) { currentCharge = MaxCharge; }

		if ( Input.GetButtonUp("Fire3") )
		{
			var clone = LaunchProjectile(Player, Projectile);
			if ( clone != null )
			{
				clone.GetComponent<Projectile>().Damage *= currentCharge;
				CurrentCoolDown = CoolDown * currentCharge;
			}

			currentCharge = 0.0f;
			return clone;
		}

		return null;
	}
	
	public GameObject getShootPoint() 
	{
		return GetChild("ShootPoint");
	}
	
	public Ray getRayCast()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		var cam = player.transform.FindChild("Main Camera").gameObject.GetComponent<Camera>();		
		var dir = cam.transform.TransformDirection(Vector3.forward);
		return new Ray( cam.transform.position, dir );		
	}
	
	
}
