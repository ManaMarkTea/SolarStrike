using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

/// <summary>
/// Generates the world.
/// It contains the world.  This is what we presist out.
/// Changes here are reflected in the actual world.
/// This is placed onto an object and that object will contain the world.
/// </summary>
public class GenerateWorld : MonoBehaviour {
	
	public static GenerateWorld World;
	
	public Dictionary<string, GameObject> BlockTypes = new Dictionary<string, GameObject>();	
	public GameObject[] AvailbleBlocks;
	
	public static VoxelCube WorldCubeData;

	public bool EditorMode;
	public static string LevelToLoad = "Level.txt";

	public GenerateWorld()
	{

	}
	
	System.Random rand = new System.Random(42);
	
	// Use this for initialization
	void Start () {
		World = this;

		Screen.lockCursor = true;
		Screen.showCursor = false;

		//Store prefabs of all blocks we know about into a map by name:
		foreach ( var prefab in AvailbleBlocks )
		{
			BlockObject block = prefab.GetBlock();
			if ( block != null )
			{
				BlockTypes[block.ObjectID] = prefab;
			}
		}

		
		//HACKY, removes the window border using PInvoke.
		{
			Util.GoFullscreenBorderless();
		}
			
		WorldCubeData = new VoxelCube();

		//When a cell is removed, destroy it!
		WorldCubeData.CellRemoved += (obj) => 
		{
			Destroy(obj.Block);
		};

		LoadLevel();

	}

	/// <summary>
	/// Finds the nearest block of a given type
	/// </summary>
	/// <returns>The nearest block of type. Null if none was found.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="blockType">Block type.</param>
	public BlockObject FindNearestBlockOfType( Vector3 pos, string blockType)
	{
		//TODO: Optimize?

		float minDist = float.MaxValue;
		BlockObject powerCellBlock = null;
		foreach ( var blockObject in GenerateWorld.WorldCubeData.Cells )
		{
			var block = blockObject.Value.GetBlock();
			if ( block != null && block.ObjectID == blockType)
			{
				float dist = (pos - block.transform.position).magnitude;
				if ( dist < minDist )
				{
					powerCellBlock = block;
					minDist = dist;
				}
			}
		}
		return powerCellBlock;
	}

	/// <summary>
	/// Finds the blocks of a given type
	/// </summary>
	/// <returns>The blocks of type. Empty if none was found.</returns>
	/// <param name="pos">Position.</param>
	/// <param name="blockType">Block type.</param>
	public List<BlockObject> FindBlocksOfType( string blockType)
	{
		List<BlockObject> output = new List<BlockObject>();

		foreach ( var blockObject in GenerateWorld.WorldCubeData.Cells )
		{
			var block = blockObject.Value.GetBlock();
			if ( block != null && block.ObjectID == blockType)
			{
				output.Add(block);
			}
		}
		return output;
	}


	/// <summary>
	/// Gets the block at the given index;
	/// </summary>
	/// <returns>The block at index.</returns>
	/// <param name="index">Index.</param>
	public GameObject getBlockAtIndex( BlockIndex index )
	{
		return WorldCubeData[index];
	}

	/// <summary>
	/// Gets the index at the point.
	/// Since the data is 1x1 units, but the world is actually 2x2 we need to multiply by 0.5f
	/// </summary>
	/// <returns>The index of point.</returns>
	/// <param name="pos">Position.</param>
	public BlockIndex getIndexOfPoint( ref Vector3 pos )
	{
		Vector3 cellPos = new Vector3(pos.x * 0.5f, pos.y * 0.5f, pos.z * 0.5f);
		var index = WorldCubeData.GetCellIndex(ref cellPos);			
		return index;
	}

	/// <summary>
	/// Gets the index at the point.
	/// Or you could new BlockIndex(x,y,z) urself...
	/// </summary>
	/// <returns>The index of point.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public BlockIndex getIndexOfPoint( int x, int y, int z )
	{
		var index = WorldCubeData.GetCellIndex(x, y, z);			
		return index; 
	}

	/// <summary>
	/// Gets the world position of point rounded properly to the Grid Unit.
	/// </summary>
	/// <returns>The position of point.</returns>
	/// <param name="pos">Position.</param>
	public Vector3 getPositionOfPoint( ref Vector3 pos )
	{
		Vector3 cellPos = new Vector3(pos.x * 0.5f, pos.y * 0.5f, pos.z * 0.5f);
		var index = WorldCubeData.GetCellIndex(ref cellPos);			
		return new Vector3(index.x * 2, index.y * 2, index.z * 2);
	}

	/// <summary>
	/// Gets the world position of the index, rounded properly to the grid unit.
	/// </summary>
	/// <returns>The position of point.</returns>
	/// <param name="index">Index.</param>
	public Vector3 getPositionOfPoint( ref BlockIndex index )
	{
		return new Vector3(index.x * 2, index.y * 2, index.z * 2);
	}

	public void SnapRotationToGrid( ref Vector3 euler )
	{
		euler.x = (float)(Math.Round((float)euler.x / 90.0f) * 90);
		euler.y = (float)(Math.Round((float)euler.y / 90.0f) * 90);
		euler.z = (float)(Math.Round((float)euler.z / 90.0f) * 90);
	}
	
	public bool CreateBlockAt( Vector3 pos, string blockId, Vector3 eulerRotation)
	{
		Vector3 cellPos = new Vector3(pos.x * 0.5f, pos.y * 0.5f, pos.z * 0.5f);
		var index = WorldCubeData.GetCellIndex(ref cellPos);
		SnapRotationToGrid(ref eulerRotation);	
		Vector3 gridPos = new Vector3(index.x * 2, index.y * 2, index.z * 2);

		var playerPos = GameObject.FindGameObjectWithTag("MainCamera").transform.position;		
		var playerIndexTop = getIndexOfPoint( ref playerPos);
		var playerIndexBottom = new BlockIndex(playerIndexTop.x, playerIndexTop.y-1, playerIndexTop.z);
		
		//Don't create on top of ourselves:
		if ( (playerIndexTop != index) && (playerIndexBottom != index) )
		{		
			//WorldCubeData[index] = new Block(spawnType, (int)eulerRotation.x, (int)eulerRotation.y, (int)eulerRotation.z);
			GameObject clone = Instantiate(BlockTypes[blockId], gridPos, Quaternion.Euler(eulerRotation)) as GameObject;
			clone.name = blockId + " " + index.x + ", " + index.y + "," + index.z;
			clone.transform.parent = this.transform;
			WorldCubeData[index] = clone;
			clone.GetBlock().ConfigureSubType();
			SaveLevel("Level.txt");
			return true;
		}
		
		return false;
	}
	
	public void RemoveBlockAt( Vector3 pos )
	{
		Vector3 cellPos = new Vector3(pos.x * 0.5f, pos.y * 0.5f, pos.z * 0.5f);
		BlockIndex index = WorldCubeData.GetCellIndex( ref cellPos );
		WorldCubeData[index] = null;
		SaveLevel("Level.txt");
	}
	/// <summary>
	/// Update this instance.
	/// </summary>
	// Update is called once per frame
	void Update () {

	}

	//IO: Probably not the right way to do this...
	
	public void SaveLevel(string fileName)
	{
		StringBuilder builder = new StringBuilder();
		foreach( BlockIndex index in WorldCubeData.Cells.Keys)
		{
			var block = WorldCubeData[index];
			BlockObject blockObj = block.GetBlock();
			if ( blockObj != null )
			{
				Vector3 rot = block.transform.rotation.eulerAngles;
				builder.Append(index.x).Append(" ").Append(index.y).Append(" ").Append(index.z).Append(" ").Append(blockObj.ObjectID )
					.Append(" ").Append((int)rot.x)
						.Append(" ").Append((int)rot.y)
						.Append(" ").Append((int)rot.z)
						.Append(" ").Append((int)blockObj.SubType).
						AppendLine();
			}
		}

		if ( fileName == null )
		{
#if UNITY_EDITOR
			var curLock = Screen.lockCursor;
			var curShow = Screen.showCursor;
			Screen.lockCursor = false;
			Screen.showCursor = true;
			fileName = UnityEditor.EditorUtility.SaveFilePanel("Save this level to .txt", Application.dataPath, "DefaultLevel.txt", "txt");
			Screen.lockCursor = curLock;
			Screen.showCursor = curShow;
#endif
		}

		if ( string.IsNullOrEmpty(fileName) == false )
		{
			#if UNITY_WEBPLAYER
				PlayerPrefs.SetString("LevelData", builder.ToString());
			#else
				File.WriteAllText(fileName, builder.ToString());
			#endif
		}
	}
	
	public void LoadLevel() 
	{
		var levelName = LevelToLoad;

		#if UNITY_WEBPLAYER
		bool levelOkay = PlayerPrefs.HasKey("LevelData");
		#else
		bool levelOkay = File.Exists(levelName);
		#endif	


		if ( EditorMode) 
		{
			Stack<GameObject> stack = new Stack<GameObject>();
			stack.Push(this.gameObject);

			while (stack.Count > 0 ) 
			{
				var top = stack.Pop();
				var block = top.GetComponent<BlockObject>();
				if ( block != null )
				{
					var blockPos = top.transform.position;
					BlockIndex index = getIndexOfPoint(ref blockPos);
					WorldCubeData[index] = block.gameObject;
				}

				for ( int i = 0; i < top.transform.childCount; i++ )
				{
					stack.Push(top.transform.GetChild(i).gameObject);
				}
			}

			var assetPath = Application.dataPath;
			var fileName = assetPath + System.IO.Path.DirectorySeparatorChar + "DefaultLevel.txt";
			#if UNITY_EDITOR
			fileName = null;
			#endif

			SaveLevel(fileName);
			Application.LoadLevel("MainMenu");

		}
		else if ( levelOkay )
		{
			#if UNITY_WEBPLAYER
				string[] data = PlayerPrefs.GetString("LevelData").Split('\n');
			#else
				string[] data = File.ReadAllLines(levelName);
			#endif	

			foreach( string line in data)
			{
				string[] parts = line.Split(' ');
				if ( parts.Length > 6)
				{
					int x = int.Parse(parts[0]);
					int y = int.Parse(parts[1]);
					int z = int.Parse(parts[2]);
					string type = parts[3];
					int rX = int.Parse(parts[4]);
					int rY = int.Parse(parts[5]);
					int rZ = int.Parse(parts[6]);
					int subType = 0;
					
					if ( parts.Length > 7 ) 
					{
						subType = int.Parse(parts[7]);
					}
					
					
					var index = new BlockIndex( x,y,z);					
					GameObject obj = null;
					if ( BlockTypes.ContainsKey(type) )
					{
						obj = BlockTypes[type];
					}
					
					if ( obj != null )
					{
						GameObject clone = Instantiate(obj, new Vector3(x*2,y*2,z*2), Quaternion.Euler(rX, rY, rZ)) as GameObject;
						clone.name = type + " " + x + ", " + y + "," + z;
						clone.transform.parent = this.transform;					
						WorldCubeData[index] = clone;
						clone.GetBlock().SubType = subType;
					}
					else
					{
						Debug.LogError("Missing: " + type);
					}
				}
			}
		}
	}




	//Procedural Generation: (NOT IMPLEMENTED) (NOT FINISHED)


	/// <summary>
	/// Creates 10 objects in a line from a point:
	/// 
	/// </summary>
	/// <param name="startPoint">Start point.</param>
	void BranchOut( Vector3 startPoint )
	{
		Vector3 point = startPoint;		
		//point = WorldCubeData.AddPathX( startPoint, 10);
		
		CreateBlockAt( startPoint, "Metal" , Vector3.zero);
		
		Vector3 end = Vector3.zero;
		for ( int i = 0; i < 10; i ++ )
		{
			//BranchRand(ref point, ref end);
			point = end;
		}
		
	}

}
