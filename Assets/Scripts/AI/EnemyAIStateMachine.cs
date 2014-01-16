using UnityEngine;
using System.Collections;

/// <summary>
/// State machine for the Enemy AI.
/// </summary>
public class EnemyAIStateMachine : MonoBehaviour {

	public StateMachine stateMachine;

	public Material TrailOverrideMaterial;
	public Material ProjectileOverrideMaterial;

	public bool Disabled;
	public IWeapon weapon;
	public GameObject Target;

	public string txtHPTag;
	public GUIText txtHP;

	public Vector3[] WayPoints;	
	private MovementUtil movement;

	protected bool alive = true;
	public float HP = 50;


	public void TakeDamage(float distance)
	{		
		if ( Disabled )
		{
			return;
		}

		HP -= distance;		


		//Find the tag if we must!
		if ( txtHPTag != null )
		{
			var obj = GameObject.FindGameObjectWithTag(txtHPTag);
			if ( obj != null )
			{
				txtHP = obj.GetComponent<GUIText>();
			}

		}

		//Update the hp bar if we have one:
		if ( txtHP != null )
		{
			this.txtHP.text = HP.ToString("0");
		}

	}

	// Use this for initialization
	void Start () {
		stateMachine = new StateMachine(this.gameObject);

		//Find the patrol state from our component list and set it.
		var patrol = GetComponent<PatrolState>();
		patrol.Target = Target;
		patrol.WayPoints = WayPoints;
		patrol.StateGameObject = this.gameObject;
		patrol.Start();

		stateMachine.SetState( patrol );

		//Just makes moving around easier:
		movement = new MovementUtil();
		movement.ControlObject = this.gameObject;
		movement.MaxSpeed = 10.0f;
		
		patrol.movement = movement;
	}
	
	// Update is called once per frame
	void Update () {

		//Don't update if we're disabled.
		if ( Disabled )
		{
			if ( stateMachine == null ) return;
			this.stateMachine.currentState.enabled = false;
			return;
		}

		this.stateMachine.currentState.enabled = true;
		stateMachine.Update();

		//Check for death:
		if ( HP <= 0 )
		{		
			if ( alive)
			{	
				this.stateMachine.SetState( this.GetComponent<EnemyDeathState>() );
				alive = false;			
			}
		}

	}
}
