using UnityEngine;
using System.Collections;

/// <summary>
/// Block object: Represents a block in the world.
/// All useful information it takes to make stuff happen in the world.
/// </summary>
public class BlockObject : MonoBehaviour {

	public bool showHP;
	public GameObject HPBar;
	private GameObject HPBarInstance;

	public string ObjectID;	
	public bool CollideWithPlayer = true;
	public BlockIndex Index;
	public int SubType;
	private int lastSubType = -1;
	
	private bool alive = true;
	private float MaxHP = 10;
	public float HP = 10;
	public float DPS = 10;
	public bool ReGen = true;
	public float ReGenDelay = 5.0f;
	public float ReGenDelayRemaining = 5.0f;
	public float ReGenHealthPerSecond = 5.0f;

	public GameObject debris;

	//Useful for pathing ( Middle click, easy placement )
	public bool Pathable = false;
	public int MaxPathDistance = 5;
	public float StepDelta = 0;

	public GameObject PreviewBlock;
	public Texture PreviewTexture;
	public Shader PreviewShader;

	//Lights, posters, flags, etc sit on walls, their initial oritentation is different.
	public bool RotatesTowardWalls;

	public bool SpecialRotateX90;
	public bool SpecialRotateY90;
	public bool SpecialRotateZ90;
	public bool SpecialRotateX270;
	public bool SpecialRotateY270;
	public bool SpecialRotateZ270;

	//Triggers the IActivate Handler on the block, if it has one.
	public void Activate() 
	{
		IActivate handler = this.GetComponent<IActivate>();
		if ( handler != null )
		{
			handler.Activate();
		}
	}

	/// <summary>
	/// Takes the damage and updates a floating text box above the block. (if it has one)
	/// </summary>
	/// <param name="distance">Distance.</param>
	public void TakeDamage(float distance)
	{		
		HP -= distance;		

		if ( showHP == false )
		{
			showHP = true;

			if ( HPBar != null )
			{
				Vector3 pos = this.transform.position;
				pos.y += 2;

				HPBarInstance = Instantiate(HPBar, pos, Quaternion.identity) as GameObject; 
				HPBarInstance.transform.parent = this.transform;

				UpdateHPBar();
			}
		}

		UpdateHPBar();

		ReGenDelayRemaining = ReGenDelay;
	}

	private void UpdateHPBar () 
	{
		if ( HPBarInstance != null )
		{
			var text = HPBarInstance.GetComponentInChildren<TextMesh>();
			text.text = HP.ToString("0");

			//If it refills, we don't care to see it.
			if ( HP >= MaxHP )
			{
				DestroyObject(HPBarInstance);
				HPBarInstance = null;
				showHP = false;
			}

		}

	}

	// Use this for initialization
	void Start () {
		this.MaxHP = this.HP;
	}
	
	public void ConfigureSubType() 
	{
		//Glass does some special stuff if its placed on top of another object vs if its placed over open space.
		//This should probably be a special handler instead of hard coded in here.  
		if ( this.ObjectID == "Glass") 
		{
			var below = Index;
			below.y -= 1;
			if ( GenerateWorld.World.getBlockAtIndex(below) == null )
			{
				//Show the horizontal version:
				this.transform.Rotate( new Vector3(90,0,0));
			} 
			else 
			{
				//Show the Verticle version:
				SubType = 1;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if ( collider != null )
		{
			//Turn off collisions if this don't collide with the player:
			Physics.IgnoreCollision(collider, GameObject.FindGameObjectWithTag("Player").collider, CollideWithPlayer == false);
		}

		//Refills the HP after a delay:
		if ( HP < MaxHP && ReGen)
		{
			ReGenDelayRemaining -= Time.deltaTime;
			if ( ReGenDelayRemaining <= 0 ) 
			{
				HP += ReGenHealthPerSecond * Time.deltaTime;

				if ( HP >= MaxHP ) { HP = MaxHP; }

				UpdateHPBar();
			}

		}

		//This is old, but if an object has multiple representations, this
		//will show the given subtype. (think Pipe Elbows etc, not implemented);
		if ( lastSubType != SubType )
		{
			var subTypes = this.transform.FindChild("SubTypes");
			if ( subTypes != null )
			{
				for ( int i = 0; i < subTypes.childCount; i++ )
				{
					subTypes.GetChild(i).gameObject.SetActive(false);	
				}
				
				if ( this.SubType >= 0 && this.SubType <= subTypes.childCount )
				{
					subTypes.GetChild(this.SubType).gameObject.SetActive(true);				
				}
			}
		}

		//Block is destroyed!! Remove it from the world, create the debris.
		if ( HP <= 0 )
		{		
			if ( alive)
			{				
				var pos = this.transform.position;				
				
				var worldObj = GameObject.FindGameObjectWithTag("World");
				var World = worldObj.GetComponent<GenerateWorld>();
				World.RemoveBlockAt( this.transform.position);
				alive = false;
				
				
				Instantiate(debris, pos, Quaternion.identity); 
				
			}
			
			
		}
	}
}
