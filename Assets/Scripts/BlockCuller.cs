using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(BlockObject))]
public class BlockCuller : MonoBehaviour {

	BlockObject block;

	public class SubMesh {
		public int[] triangles;
	}

	protected static Dictionary<Mesh, SubMesh[]> meshLookup = new Dictionary<Mesh, SubMesh[]>();

	// Use this for initialization
	void Start () {
		this.block = this.GetComponent<BlockObject>();

		if ( block != null )
		{
			var mesh = this.GetComponentInChildren<MeshFilter>();
			if ( meshLookup.ContainsKey(mesh.sharedMesh) == false )
			{
				SubMesh[] subMesh = new SubMesh[6];

				Vector3[] directions = new Vector3[]{ Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};
				for ( int i = 0; i < directions.Length; i++ )
				{
					subMesh[i] = new SubMesh();
					List<int> triangles = new List<int>();

					for ( int t = 0; t < mesh.sharedMesh.triangles.Length; t+=3)
					{
						int indexA = mesh.sharedMesh.triangles[t+0];
						int indexB = mesh.sharedMesh.triangles[t+1];
						int indexC = mesh.sharedMesh.triangles[t+2];

						Vector3 a = mesh.sharedMesh.vertices[indexA];
						Vector3 b = mesh.sharedMesh.vertices[indexB];
						Vector3 c = mesh.sharedMesh.vertices[indexC];

						Vector3 normal = Vector3.Cross( (c-a), (c-b));
						float angle = Vector3.Angle(directions[i], normal);

						if ( Mathf.Approximately( angle, 0) )
						{
							triangles.Add( indexA);
							triangles.Add( indexB);
							triangles.Add( indexC);
						}
					}

					subMesh[i].triangles = triangles.ToArray();
					meshLookup[mesh.sharedMesh] = subMesh;
				}
			}

			SubMesh[] submesh = meshLookup[mesh.sharedMesh];

			mesh.mesh.triangles = submesh[4].triangles;

		}

	}
	
	// Update is called once per frame
	void Update () {

	}
}
