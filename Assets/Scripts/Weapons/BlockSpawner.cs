using UnityEngine;
using System.Collections;

public class BlockSpawner : Projectile {
	
	private GenerateWorld World;
	public BlockObject SpawnType;
	private Collision lastCollision;

	public BlockSpawner()
	{
	}

	// Use this for initialization
	public override void Start () {
		base.Start();

		var worldObj = GameObject.FindGameObjectWithTag("World");
		World = worldObj.GetComponent<GenerateWorld>();
		
		if ( this.ProjectilePrefab != null )
		{
			//Remove the preview objects, we want the real deal ( the prefab )
			for ( int i = gameObject.transform.childCount-1; i >= 0 ; i--)
			{
				Destroy(gameObject.transform.GetChild(i).gameObject);
			}
			
			GameObject newKids = Instantiate(ProjectilePrefab, this.transform.position, this.transform.rotation) as GameObject;
			newKids.transform.parent = this.transform;
			newKids.transform.localScale = this.transform.localScale;
			if ( newKids.collider != null )
			{
				Physics.IgnoreCollision(newKids.collider, GameObject.FindGameObjectWithTag("Player").collider);
			}
		}
		
		
		
	}
	

	// Update is called once per frame
	void Update () {
		base.Update();

		if (Input.GetButtonDown("Fire2"))
        {	
			Trigger();
        }
	}
	
	public override void Trigger ()
	{
		if ( time < TriggerTime )
		{
			//Orient:
			Vector3 pos = this.transform.position;
			Vector3 normal;
			if ( lastCollision != null )
			{
				normal = lastCollision.contacts[0].normal;
				pos -= normal * 0.25f;
			}
			else 
			{
				normal = -this.rigidbody.velocity.normalized;
			}
			
			Vector3 rot = Quaternion.FromToRotation(-Vector3.forward, normal).eulerAngles;
			rot.x = 0;
			rot.z = 0;		

			if ( normal.y < -0.5f ) { 
				rot.x = 180;
				rot.y += 180;
			}

			bool placed = World.CreateBlockAt(pos, SpawnType.ObjectID, rot );
		}

		DestroyObject(this.gameObject);
	}
	
	
}
