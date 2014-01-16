using UnityEngine;
using System.Collections;

/// <summary>
/// Spawns x enemies every x seconds;
/// </summary>
public class EnemySpawner : MonoBehaviour {

	public GameObject Enemy;

	public int Count = 10;
	public float SecondsPerEnemy = 1.0f;

	//Status variables:
	private float currentTime;
	public int spawned;
	
	private GameObject pathObj;
	private AStarPath path;
	private bool validPath;

	// Use this for initialization
	void Start () {
		this.spawned = 0;

		//The only way to create a new object from scratch is to attach it to a new object:
		pathObj = new GameObject("PathSystem");
		pathObj.transform.parent = this.transform;
		pathObj.transform.localPosition = Vector3.zero;

		//This is the only way to create a component properly:
		path = pathObj.AddComponent<AStarPath>();


		//If the world changes we need to recompute:
		GenerateWorld.WorldCubeData.CellAdded += (obj) => { this.validPath = false; };
		GenerateWorld.WorldCubeData.CellRemoved += (obj) => { this.validPath = false; };
	}

	/// <summary>
	/// Checks the path, if needed, it will recompute it:
	/// </summary>
	public void CheckPath()
	{
		if ( validPath == false ) 
		{
			BlockObject powerCellBlock = GenerateWorld.World.FindNearestBlockOfType( this.transform.position, "PowerCell");

			if ( powerCellBlock != null )
			{
				var startTime = System.DateTime.Now;
				path.ComputePath(this.transform.position, powerCellBlock.transform.position);
				var endTime = System.DateTime.Now;

				double diff = (endTime - startTime).TotalSeconds;

				//Slowness detector!
				if ( diff > 1 )
				{
					Debug.LogWarning("Path Creation: " + diff + " sec");
				}

				validPath = true;
								
				//Updates the visuals if we have any:
				LineRenderer line = GetComponent<LineRenderer>();
				if ( line != null )
				{
					line.SetVertexCount(path.WayPoints.Length);
					
					for ( int i =0; i < path.WayPoints.Length; i++ )
					{
						line.SetPosition(i, path.WayPoints[i]);
						
					}
				}
				
			}
		}
	}



	// Update is called once per frame
	void Update () {
		CheckPath();

		if ( validPath && spawned < Count)
		{
			currentTime += Time.deltaTime;

			//Is it time to spawn one?
			if ( currentTime >= SecondsPerEnemy )
			{
				currentTime -= SecondsPerEnemy;

				var enemyInstance = Instantiate( Enemy, this.transform.position, this.transform.rotation) as GameObject;
				var ai = enemyInstance.GetComponent<EnemyAIStateMachine>();

				//Sets the target on the AI of the new enemy, and gives it a patrol path:
				var target = GenerateWorld.World.getBlockAtIndex( path.targetDest );
				if ( target == null )
				{
					target = GameObject.FindGameObjectWithTag("Player");
				}

				ai.Target = target;
				ai.WayPoints = path.WayPoints;

				spawned++;
			}
		}

	}
}
