using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponScript : MonoBehaviour {

	public List<IWeapon> Weapons;
	public IWeapon GenericBlockWeapon;
	public string[] GeneratedTypes = new string[] { "Metal" , "Metal_Half", "Metal_Ramp", "Metal_Ramp_Half", "Light", "Glass" };
	
	protected IWeapon activeWeapon;
	
	int activeIndex = 0;
	List<IWeapon> InstancedWeapons;

	public Dictionary<string, IWeapon> WeaponMap = new Dictionary<string, IWeapon>();

	// Use this for initialization
	void Start () {
		this.InstancedWeapons = new List<IWeapon>();
			
		Transform cam = transform.FindChild("Main Camera").transform;		
		
		var blocks = GameObject.FindGameObjectWithTag("World");
		var world = blocks.GetComponent<GenerateWorld>();
		
		foreach ( IWeapon weap in Weapons )
		{					
			GameObject weaponInstance = Instantiate(weap.gameObject, cam.position, cam.rotation) as GameObject;
			IWeapon instance = weaponInstance.GetComponent<IWeapon>();
			weaponInstance.transform.parent = cam;

			InstancedWeapons.Add( instance );
			instance.ItemId = weap.name;
			WeaponMap[weap.name] = instance;

			weaponInstance.gameObject.SetActive(false);
			instance.isPlayerWeapon = true;
			weaponInstance.gameObject.layer = LayerMask.NameToLayer("Foreground");
		}

		if ( GenericBlockWeapon != null )
		{			
			foreach ( string genWeapon in GeneratedTypes )
			{
				GameObject weaponInstance = Instantiate(GenericBlockWeapon.gameObject, cam.position, cam.rotation) as GameObject;
				weaponInstance.transform.parent = cam;
				IWeapon instance = weaponInstance.GetComponent<IWeapon>();
				instance.isPlayerWeapon = true;

				InstancedWeapons.Add(instance);
				weaponInstance.gameObject.layer = LayerMask.NameToLayer("Foreground");
				
				Weapon gun = weaponInstance.GetComponent<Weapon>();
				Transform placement = gun.transform.FindChild("Weapon").FindChild("Placement");
				Transform crosshair = gun.transform.FindChild("CrossHair");
				
				if ( placement != null ) 
				{						
					gun.Projectile.SetActive(true);
					var projectile = Instantiate(gun.Projectile, gun.Projectile.gameObject.transform.position, gun.Projectile.gameObject.transform.rotation) as GameObject;
					gun.Projectile.SetActive(false);
					projectile.SetActive(false);

					projectile.transform.parent = placement.transform;
					
					BlockSpawner spawner = projectile.GetComponent<BlockSpawner>();
					spawner.ProjectilePrefab = world.BlockTypes[genWeapon];
					spawner.SpawnType = spawner.ProjectilePrefab.GetBlock();
					weaponInstance.name = genWeapon;
					gun.Projectile = projectile;

					instance.ItemId = genWeapon;
					WeaponMap[instance.ItemId] = instance;

					GameObject visual = Instantiate(spawner.ProjectilePrefab, placement.position, placement.rotation) as GameObject;
					visual.transform.localScale = placement.localScale;
					visual.transform.parent = placement.transform;

					var subWeapon = visual.GetComponentInChildren<IWeapon>();
					if ( subWeapon != null )
					{
						subWeapon.Armed = false;
					}

					Util.SetLayer(visual.transform, LayerMask.NameToLayer("Foreground"));
					Util.DestroyAllColliders(visual);
				}
				weaponInstance.gameObject.SetActive(false);
			}
		}

		Inventory equipBar = GameObject.FindGameObjectWithTag("Equipment").GetComponent<Inventory>();
		Inventory inv = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

		int slots = equipBar.slots[0].Slots.Count;

		for ( int i = 0; i < InstancedWeapons.Count; i++) 
		{
			Inventory dest = equipBar;
			if ( i >= slots ){
				dest = inv;
			}


		}
		
		//activeWeapon = InstancedWeapons[activeIndex].GetComponent<IWeapon>();		
		//InstancedWeapons[activeIndex].gameObject.SetActive(true);
		
	}

	public void PickUpAllWeapons()
	{
		foreach ( string genWeapon in GeneratedTypes )
		{
			PickUpWeapon(genWeapon);
		}
	}

	public void PickUpWeapon(string weaponName)
	{
		Inventory equipBar = GameObject.FindGameObjectWithTag("Equipment").GetComponent<Inventory>();
		Inventory inv = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
		Inventory dest = equipBar;

		var slotCheck = equipBar.GetOpenSlot();
		if ( slotCheck == null )
		{
			dest = inv;
		}


		IWeapon weapon = null;

		for ( int i = 0; i < InstancedWeapons.Count; i++ )
		{
			if ( InstancedWeapons[i].ItemId == weaponName )
			{
				weapon = InstancedWeapons[i].GetComponent<Weapon>();
				break;
			}
		}

		if ( weapon == null ) { return; }

		if ( dest.GetItemSlotWithItem(weapon.ItemId) != null ) 
		{
			//We already have it!
			return;
		}


		var slot = dest.GetOpenSlot();
		
		if ( slot == null ) 
		{
			return;
		}


		BlockObject block = weapon.gameObject.GetBlockInChildren();
		if ( block != null )
		{
			GameObject itemObject = new GameObject(weapon.ItemId);
			InventoryItem item = itemObject.AddComponent<InventoryItem>();
			item.Icon = block.gameObject;
			item.ItemId = weapon.ItemId;

			//Objects need to be active in order to attach to them!
			bool state = dest.SetTempActive();
		
			itemObject.transform.parent = slot.transform;
			itemObject.transform.localPosition = Vector3.zero;
			itemObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
			itemObject.transform.localRotation = Quaternion.Euler(-30,45,0);
			dest.RestoreTempActive(state);

			this.ChangeWeapon( weapon.ItemId);
		}
		else 
		{
			var weaponShape =  weapon.transform.FindChild("Weapon");
			if ( weaponShape != null )
			{
				GameObject itemObject = new GameObject(weapon.ItemId);
				InventoryItem item = itemObject.AddComponent<InventoryItem>();
				item.Icon = weaponShape.gameObject;
				item.ItemId = weapon.ItemId;

				bool state = dest.SetTempActive();

				itemObject.transform.parent = slot.transform;
				itemObject.transform.localPosition = Vector3.zero;
				itemObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
				itemObject.transform.localRotation = Quaternion.Euler(-30,45,0);
				dest.RestoreTempActive(state);			

				this.ChangeWeapon( weapon.ItemId);
			}
		}

	}


	// Update is called once per frame
	void Update () {

		if ( Util.IsUiActive() )
		{
			return;
		}

	     // Put this in your update function
		if (Input.GetButton("Fire1") || Input.GetButtonUp("Fire1"))
        {
			if ( activeWeapon != null )
			{
				activeWeapon.Fire(this.gameObject);
			}
        }
		
		if (Input.GetButton("Fire2") || Input.GetButtonUp("Fire2"))
        {
			if ( activeWeapon != null )
			{
				activeWeapon.Fire2(this.gameObject);
			}
        }
		
		if (Input.GetButton("Fire3") || Input.GetButtonUp("Fire3"))
        {
			if ( activeWeapon != null )
			{
				activeWeapon.Fire3(this.gameObject);
			}
        }

		if (Input.GetKeyDown(KeyCode.F6) )
		{
			this.PickUpAllWeapons();
		}

		if (Input.GetKeyDown(KeyCode.E) )		
		{
			RaycastHit hit;
			Ray cast = getRayCast();
			
			int everythingButPlayer = 1 << LayerMask.NameToLayer("Player") |  1 << LayerMask.NameToLayer("Projectile");
			everythingButPlayer = ~everythingButPlayer;
			
			if ( Physics.Raycast( cast, out hit, 4, everythingButPlayer ))
			{
				var blockTouched = hit.collider.gameObject.GetBlock();
				if ( blockTouched != null )
				{
					blockTouched.Activate();
				}

			}

		}

	}

	public Ray getRayCast()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		var cam = player.transform.FindChild("Main Camera").gameObject.GetComponent<Camera>();		
		var dir = cam.transform.TransformDirection(Vector3.forward);
		return new Ray( cam.transform.position, dir );		
	}

	public void ChangeWeapon( string weaponId ) {
		for( int i = 0; i < this.InstancedWeapons.Count; i++ )
		{
			if ( this.InstancedWeapons[i].ItemId == weaponId ) {
				ChangeWeapon(i);
				break;
			}
		}
	}

	public void ChangeWeapon( int newIndex ) 
	{
		InstancedWeapons[activeIndex].gameObject.SetActive(false);
		InstancedWeapons[activeIndex].PutAway();

		activeIndex = newIndex;

		activeWeapon = InstancedWeapons[activeIndex].GetComponent<IWeapon>();
		InstancedWeapons[activeIndex].gameObject.SetActive(true);
	}

}
