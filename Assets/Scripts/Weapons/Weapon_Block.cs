using UnityEngine;
using System;


public class Weapon_Block : Weapon
{		
	private GameObject PreviewBlockInstance;
	private BlockObject projectileBlock;
	public float CastTestDist = 10;
	
	
	public delegate void OnHit ( GenerateWorld World, BlockSpawner spawner, RaycastHit hit, Vector3 currentDirection, Vector3 createAt, BlockIndex index);
	
	public Weapon_Block ()
	{
		
	}
	
	public override void Start ()
	{
		base.Start ();
		
		Projectile projectile = Projectile.GetComponentsInChildren<Projectile>(true)[0];						
		if ( projectile.ProjectilePrefab != null )
		{
			projectileBlock = projectile.ProjectilePrefab.GetBlock();
			PreviewBlockInstance = (GameObject)Instantiate(projectileBlock.PreviewBlock);
			PreviewBlockInstance.SetActive(false);
			Util.DestroyAllColliders(PreviewBlockInstance);
		}

	}		
	
	/// <summary>
	/// Casts the ray and does whatever the action is. (Template Method pattern)
	/// </summary>
	/// <returns>
	/// The ray and do.
	/// </returns>
	/// <param name='action'>
	/// If set to <c>true</c> action.
	/// </param>
	public bool CastRayAndDo( OnHit action )
	{
		RaycastHit hit;
		Ray cast = getRayCast();
		int mask = 1 << LayerMask.NameToLayer("Catwalk");
		if ( Physics.Raycast( cast, out hit, CastTestDist, mask ) )
		{
			var blockSpawner = Projectile.GetComponent<BlockSpawner>();
						
			if ( blockSpawner != null && hit.collider.gameObject.IsProjectile() == false )
			{
				var worldObj = GameObject.FindGameObjectWithTag("World");
				var World = worldObj.GetComponent<GenerateWorld>();
				//var createAt = cast.origin + (cast.direction * (hit.distance - 0.1f));
				var createAt = hit.point + (hit.normal *(0.5f));
				
				var playerPos = getRayCast().origin;
				var futureIndex = World.getIndexOfPoint( ref createAt);
				var playerIndexTop = World.getIndexOfPoint( ref playerPos);
				var playerIndexBottom = new BlockIndex(playerIndexTop.x, playerIndexTop.y-1, playerIndexTop.z);
				
				//Don't create on top of ourselves:
				if ( (playerIndexTop != futureIndex) && (playerIndexBottom != futureIndex) )
				{		
					Vector3 currentDirection = getRayCast().direction.normalized;
					action(World, blockSpawner, hit, currentDirection, createAt, futureIndex);
				}
			}
			return true;
		}			
		return false;
	}
	
	
	public override void Update ()
	{
		//This can be null in some cases.
		if ( PreviewBlockInstance == null ) return;		

		PreviewBlockInstance.SetActive(false);

		//Check to see if we should
		CastRayAndDo( ( World, spawner, hit, curDir, createAt, futureIndex) => 
		{
			if ( Input.GetKeyUp(KeyCode.Delete) || Input.GetKeyUp(KeyCode.Backspace) ) 
			{
				World.RemoveBlockAt(hit.transform.position);
			}
			
			PreviewBlockInstance.SetActive(true);
			PreviewBlockInstance.transform.position = World.getPositionOfPoint(ref createAt);
			
			//Orient:
			Vector3 rot = Quaternion.FromToRotation(-Vector3.forward, curDir).eulerAngles;
			rot.x = 0;
			rot.z = 0;				
			World.SnapRotationToGrid(ref rot);
			PreviewBlockInstance.transform.rotation = Quaternion.Euler(rot);
			
			var renderer = PreviewBlockInstance.GetComponentInChildren<Renderer>();				
			for ( int i = 0; i < renderer.materials.Length; i++ )
			{
				renderer.materials[i].shader = projectileBlock.PreviewShader;
				renderer.materials[i].mainTexture = projectileBlock.PreviewTexture;
				renderer.materials[i].color = new Color(1,1,1,0.5f);
			}
		});
		
		base.Update ();	

	}

	public override void PutAway ()
	{
		if ( PreviewBlockInstance != null )
		{
			PreviewBlockInstance.SetActive(false);	
		}
	}

	public Quaternion ComputeRotationForPlacement( GenerateWorld World, BlockSpawner spawner, Vector3 normalOfPlacementSurface, Vector3 curDir, BlockIndex futureIndex)
	{
		Quaternion output = Quaternion.identity;

		//Gets the block prefab so we can check its properties:
		var block = spawner.SpawnType;
		
		//We only care about the direction, not the inclination:
		curDir.y = 0;
		curDir.Normalize();
		
		//Due to gimbal lock that can happen along a wall, we use a different axis and then rotate into the axis we want!
		//Thus the 270 rotation.
		
		Quaternion wallDirectionAxis = Quaternion.FromToRotation( Vector3.up, normalOfPlacementSurface);
		Quaternion upDirectionAxis = Quaternion.FromToRotation( Vector3.right, normalOfPlacementSurface) * Quaternion.Euler(0,270,0);
		Quaternion rotationTowardDown = wallDirectionAxis * upDirectionAxis ;
		
		
		//Is it going on the floor/ceiling: 0.2f episilon
		if ( normalOfPlacementSurface.y < -0.2f || normalOfPlacementSurface.y > 0.2f)
		{					
			float angle = Vector3.Angle(Vector3.forward, curDir);
			var side = Vector3.Cross( Vector3.up, curDir);			
			angle *= (side.z < 0) ? 1 : -1;


			upDirectionAxis = Quaternion.AngleAxis( angle, normalOfPlacementSurface);
			var upDirEuler = upDirectionAxis.eulerAngles;


			rotationTowardDown = wallDirectionAxis * upDirectionAxis ;
			
			//This adds the special rotation that the object gets when its on the floor or ceiling.
			//This exists because different objects act differently when placed.  This covers most cases!
			if ( block.SpecialRotateX90) { rotationTowardDown = rotationTowardDown  * Quaternion.Euler(90,0,0); }
			if ( block.SpecialRotateY90) { rotationTowardDown = rotationTowardDown  * Quaternion.Euler(0,90,0); }
			if ( block.SpecialRotateZ90) { rotationTowardDown = rotationTowardDown  * Quaternion.Euler(0,0,0); }
			
			if ( block.SpecialRotateX270) { rotationTowardDown = rotationTowardDown  * Quaternion.Euler(270,0,0); }
			if ( block.SpecialRotateY270) { rotationTowardDown = rotationTowardDown  * Quaternion.Euler(0,270,0); }
			if ( block.SpecialRotateZ270) { rotationTowardDown = rotationTowardDown  * Quaternion.Euler(0,0,270); }
			
		}
		
		Vector3 rot = rotationTowardDown.eulerAngles;
		
		//If the object is designed to sit only on the floor remove the x and z rotations:
		if( block.RotatesTowardWalls == false )		
		{
			//Take only the y rotation.
			rot.x = 0;
			rot.z = 0;
		}

		output = Quaternion.Euler(rot);
		return output;

	}

	//Throw the block like a weapon!!
	public override GameObject Fire (GameObject Player)
	{	
		var proj = base.Fire(Player);	
		if( proj != null )
		{
			proj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}
		return proj;
	}

	//Place the block minecraft style:
	public override GameObject Fire2 (GameObject Player)
	{
		if ( Input.GetButtonDown("Fire2") == false || CurrentCoolDown > 0.0f ) {
			return null;
		}

		//Cast a ray outwards and see if we can place a block at the intersection point:
		bool isHit = CastRayAndDo( ( World, spawner, hit, curDir, createAt, futureIndex) => 
		                          {	
			Quaternion rot = ComputeRotationForPlacement(World, spawner, hit.normal, curDir, futureIndex);
			World.CreateBlockAt(createAt, spawner.SpawnType.ObjectID, rot.eulerAngles);
			CurrentCoolDown = CoolDown * 2;
		});
		
		return null;
	}

	
	public override GameObject Fire3 (GameObject Player)
	{	
		if ( CurrentCoolDown > 0.0f ) {
			return null;
		}

		var World = GenerateWorld.World;
		var blockSpawner = Projectile.GetComponent<BlockSpawner>();

		var newBlockObj = World.BlockTypes[blockSpawner.SpawnType.ObjectID];
		var newBlock = newBlockObj.GetBlock();

		//Only pathable blocks can be built this way.
		if ( newBlock.Pathable == false ) return null;

		//Orient:
		Ray ray = getRayCast();
		var pos = ray.origin;

		var dir = ray.direction;
		dir.y = 0;
		dir.Normalize();

		var normal = -getRayCast().direction.normalized;
		
		Vector3 rot = Quaternion.FromToRotation(-Vector3.forward, normal).eulerAngles;
		rot.x = 0;
		rot.z = 0;
		World.SnapRotationToGrid(ref rot);

		//The camera is 2 units above the "position" of the player ( using the grid position ).		
		Vector3 currentPos =  World.getPositionOfPoint(ref pos);
		currentPos.y -= 2;

		for ( int i = 0; i < newBlock.MaxPathDistance; i++ )
		{
			var currentBlock = World.getBlockAtIndex(World.getIndexOfPoint(ref currentPos));
			if ( currentBlock != null )
			{
				//If its a ramp it'll step us up.
				currentPos.y += currentBlock.GetBlock().StepDelta * 2.0f;
			}

			var posBelow = currentPos;
			posBelow.y -= 2.0f;


			var currentBlockBelow = World.getBlockAtIndex(World.getIndexOfPoint(ref posBelow));
			if ( currentBlock == null && currentBlockBelow == null )
			{
				//Ignore the block we're standing on ( 0th block )
				if ( i != 0 )
				{
					currentPos.y -= 2.0f - ( 2.0f * newBlock.StepDelta);

					World.CreateBlockAt(currentPos, blockSpawner.SpawnType.ObjectID, rot);
					CurrentCoolDown = CoolDown;
					break;
				}
			}

			//Move the path forward (each block in the world is 2 units big)
			currentPos += (dir * 2);
		}		

		return null;
		
	}
	
	
	
}

