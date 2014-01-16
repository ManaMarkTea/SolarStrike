using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GenerateWorld))]
public class WorldEditor : Editor
{
	
	public WorldEditor ()
	{
	}
	
	private Dictionary<string, GameObject> BlockTypes = new Dictionary<string, GameObject>();	
	
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		
		GenerateWorld world = target as GenerateWorld;
		List<GameObject> names = new List<GameObject>();	
		
		string [] files = UnityEditor.AssetDatabase.GetAllAssetPaths();
		for ( int i = 0; i < files.Length; i++ )
		{
			if ( files[i].StartsWith("Assets/Environment/BlockTypes")){
				GameObject prefab = Resources.LoadAssetAtPath(files[i], typeof(GameObject)) as GameObject;
				if ( prefab != null )
				{
					BlockObject block = prefab.GetComponent<BlockObject>();
					names.Add(block.gameObject);
				}
			}
		}

		world.AvailbleBlocks = names.ToArray();
		
		
		
	}
	
}


