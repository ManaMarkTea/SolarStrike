using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Based on code from:
/// http://blogs.msdn.com/b/ericlippert/archive/2007/10/04/path-finding-using-a-in-c-3-0-part-two.aspx
/// </summary>
public class Path : IEnumerable<BlockIndex>
{
	public BlockIndex LastStep { get; private set; }
	public Path PreviousSteps { get; private set; }
	public double TotalCost { get; private set; }

	private Path(BlockIndex lastStep, Path previousSteps, double totalCost)
	{
		LastStep = lastStep;
		PreviousSteps = previousSteps;
		TotalCost = totalCost;
	}

	public Path(BlockIndex start) : this(start, null, 0) {}

	public Path AddStep(BlockIndex step, double stepCost)
	{
		return new Path(step, this, TotalCost + stepCost);
	}

	public IEnumerator<BlockIndex> GetEnumerator()
	{
		for (Path p = this; p != null; p = p.PreviousSteps)
			yield return p.LastStep;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}

/// <summary>
/// Based on code from: 
/// http://blogs.msdn.com/b/ericlippert/archive/2007/10/08/path-finding-using-a-in-c-3-0-part-three.aspx
/// </summary>
class PriorityQueue<P, V>
{
	private SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();
	public void Enqueue(P priority, V value)
	{
		Queue<V> q;
		if (!list.TryGetValue(priority, out q))
		{
			q = new Queue<V>();
			list.Add(priority, q);
		}
		q.Enqueue(value);
	}
	public V Dequeue()
	{
		// will throw if there isn’t any first element!
		var numerator = list.GetEnumerator();
		numerator.MoveNext();
		var pair = numerator.Current;
		var v = pair.Value.Dequeue();
		if (pair.Value.Count == 0) // nothing left of the top priority.
			list.Remove(pair.Key);
		return v;
	}
	public bool IsEmpty
	{
		get { return list.Count == 0; }
	}
}



/// <summary>
/// A* star path finding system
/// </summary>
public class AStarPath : MonoBehaviour {

	protected GenerateWorld World;

	public Vector3[] WayPoints = new Vector3[0];
	public BlockIndex targetDest;

	//Buffers:
	private List<BlockIndex> neighborBuffer = new List<BlockIndex>();
	private List<BlockIndex> neighborBuffer2 = new List<BlockIndex>();

	//Diagnostics:
	private int skippedNodes;
	private int testsPerformed;
	private int time;


	// Update is called once per frame
	public void Start () {
		this.World = GenerateWorld.World;	
		GameObject player = GameObject.FindGameObjectWithTag("Player");
	}


	public List<BlockIndex> Neighbors( List<BlockIndex> neighbors, BlockIndex index )
	{
		neighbors.Clear();

		for ( int x = -1; x <= 1; x++)
		{
			for ( int y = -1; y <= 1; y++)
			{
				for ( int z = -1; z <= 1; z++)
				{
					if ( x == 0 && y == 0 && z == 0 ) continue;

					neighbors.Add( new BlockIndex( index.x + x, index.y + y, index.z + z));
				}
			}
		}

		return neighbors;

	}

	/// <summary>
	/// Distance between the specified start and dest, takes into account walls and such.
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="dest">Destination.</param>
	public double distance( BlockIndex start, BlockIndex dest )
	{
		Vector3 destPos = new Vector3(dest.x, dest.y, dest.z);
		Vector3 startPos = new Vector3(start.x, start.y, start.z);

		bool hasNeighbor = false;

		/*
		 * This is slow, so we'll just use the optimized physics system to check.
		 */
		/*
		var neighbors = this.Neighbors(neighborBuffer2, dest);			

		foreach(BlockIndex n in neighbors)
		{
			var block = World.getBlockAtIndex( n);
			if ( block != null && n != targetDest )
			{
				hasNeighbor = true;
			}
		}
		*/

		bool occupied = Physics.CheckSphere( new Vector3(dest.x * 2, dest.y * 2, dest.z * 2), 0.5f);
		hasNeighbor = Physics.CheckSphere( new Vector3(dest.x * 2, dest.y * 2, dest.z * 2), 2);

		if ( occupied )
		{
			return 10000;
		}

		float dist = (destPos - startPos).magnitude;

		if ( hasNeighbor )
		{
			//Makes it less appealing to take a closed path.
			dist += 2;
		}

		return dist;
	}

	/// <summary>
	/// Estimate the specified distance between two points ( Euclidian distance )
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="dest">Destination.</param>
	public double estimate( BlockIndex start, BlockIndex dest )
	{
		Vector3 destPos = new Vector3(dest.x, dest.y, dest.z);
		Vector3 startPos = new Vector3(start.x, start.y, start.z);
		return (destPos - startPos).magnitude;

	}

	/// <summary>
	/// Computes the full path between two points.  Uses A* when needed, otherwise goes for line of sight.
	/// </summary>
	/// <param name="startPos">Start position.</param>
	/// <param name="destinationPos">Destination position.</param>
	public void ComputePath(Vector3 startPos, Vector3 destinationPos )
	{
		Path pathSteps = this.FindPath( startPos, destinationPos);
		List<Vector3> points = new List<Vector3>();
		foreach ( BlockIndex index in pathSteps)
		{
			BlockIndex i = index;
			points.Insert(0, World.getPositionOfPoint( ref i) );
		}

		this.WayPoints = points.ToArray();
	}

	/// <summary>
	/// Finds the path between two points using the A* algorithm.
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="startPos">Start position.</param>
	/// <param name="destinationPos">Destination position.</param>
	public Path FindPath( Vector3 startPos, Vector3 destinationPos )
	{
		testsPerformed = 0;

		var start = World.getIndexOfPoint( ref startPos);
		var destination = World.getIndexOfPoint( ref destinationPos);
		targetDest = destination;

		var closed = new HashSet<BlockIndex>();
		var queue = new PriorityQueue<double, Path>();
		queue.Enqueue(0.0, new Path(start));

		while (!queue.IsEmpty)
		{
			var path = queue.Dequeue();

			if (closed.Contains(path.LastStep))
			{
				continue;
			}

			if (path.LastStep.Equals(destination))
			{
				return path;
			}

			testsPerformed++;

			if ( testsPerformed > 500 ) 
			{
				return path;
			}

			closed.Add(path.LastStep);

			var neighbors = this.Neighbors(neighborBuffer, path.LastStep);

			foreach(BlockIndex n in neighbors)
			{
				double d = distance(path.LastStep, n);

				if (n.Equals(destination))
				{
					d = 0;
				}

				var newPath = path.AddStep(n, d);

				double distanceFromTarget = estimate(n, destination);
				queue.Enqueue( d + distanceFromTarget, newPath);
			}
		}
		return null;
	}



	// Update is called once per frame
	public void Update () {

	}
}
