using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshCombineChildren : MonoBehaviour {

	public int ChunkSize = 64;

	public class ChunkContext {
		public List<MeshFilter> MeshFilters = new List<MeshFilter>();
		public GameObject Chunk;
	}

	private GameObject OptimizedChunk;
	private bool dirty;
	private List<BlockIndex> indices = new List<BlockIndex>();
	private Dictionary<BlockIndex, ChunkContext> batches = new Dictionary<BlockIndex, ChunkContext>();

	private List<MeshFilter> meshFilters = new List<MeshFilter>();
	private List<MeshRenderer> renderers = new List<MeshRenderer>();
	private List<BlockIndex> dirtyIndices = new List<BlockIndex>();

	// Use this for initialization
	void Start () {
		
		OptimizedChunk = new GameObject();
		OptimizedChunk.transform.parent = this.transform.parent;
		OptimizedChunk.name = this.name + "_CHUNKS";
        

		meshFilters = this.gameObject.FindInChildren<MeshFilter>();
		renderers = this.gameObject.FindInChildren<MeshRenderer>();

		RecalculateMesh();
		dirty = false;

		GenerateWorld.World.OnWorldChanged += (BlockIndex index, GameObject block) => {

			var pos = block.transform.position;
			BlockIndex chunkIndex = new BlockIndex( (int)(pos.x / ChunkSize), (int)(pos.y / ChunkSize), (int)(pos.z / ChunkSize));            
			dirtyIndices.Add(chunkIndex);

			this.dirty = true;
		};

	}

	public void RecalculateMesh(){

		Material[] mats = null;
		meshFilters = this.gameObject.FindInChildren<MeshFilter>();
		renderers = this.gameObject.FindInChildren<MeshRenderer>();
        
        if ( renderers.Count > 0 )
		{
			var myRender = this.GetComponent<MeshRenderer>();
			mats = myRender.materials = renderers[renderers.Count-1].materials;
		}
			
		CombineInstance[] combine = null;


		for ( int i = 0; i < dirtyIndices.Count; i++ )
		{
			if ( batches.ContainsKey(dirtyIndices[i]) ) {
				batches[dirtyIndices[i]].MeshFilters.Clear();
			}
		}

		for (var i = 0; i < meshFilters.Count; i++){
			if ( meshFilters[i] == this.GetComponent<MeshFilter>() )
			{
				continue; 
			}

			var pos = meshFilters[i].transform.position;
			BlockIndex index = new BlockIndex( (int)(pos.x / ChunkSize), (int)(pos.y / ChunkSize), (int)(pos.z / ChunkSize));

			if ( dirtyIndices.Count > 0 && dirtyIndices.Contains(index) == false )
			{
				continue;
			}

			ChunkContext context = null;

			if ( batches.ContainsKey( index ) == false )
			{
				context = new ChunkContext();
				context.Chunk = new GameObject();
				context.Chunk.AddComponent<MeshFilter>();
				context.Chunk.AddComponent<MeshRenderer>().materials = mats;
				context.Chunk.transform.parent = OptimizedChunk.transform;
				context.Chunk.name = this.name + "_CHUNK_" + (index.x + "_" +index.y + "_"  +index.z );
				batches[index] = context;
				indices.Add(index);

			} else {
				context = batches[index];
			}

			context.MeshFilters.Add(meshFilters[i]);
		}

		var indexList = (dirtyIndices.Count == 0) ? indices : dirtyIndices;

		for ( int i = 0; i < indexList.Count; i++ )
		{
			BlockIndex index = indexList[i];

			if ( batches.ContainsKey(index) == false )
			{
				//Might have been a diff kinda block.
				continue;
			}

			ChunkContext context = batches[index];

			combine = new CombineInstance[ context.MeshFilters.Count ];
			for ( int j = 0; j < context.MeshFilters.Count; j++)
			{
				combine[j].mesh = context.MeshFilters[j].sharedMesh;
				combine[j].transform = context.MeshFilters[j].transform.localToWorldMatrix;
				context.MeshFilters[j].gameObject.active = false;
			}

			if ( combine.Length > 0 )
			{
				context.Chunk.transform.GetComponent<MeshFilter>().mesh = new Mesh();
				context.Chunk.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
				context.Chunk.transform.gameObject.active = true;
			}
		}

		dirtyIndices.Clear();
		meshFilters.Clear();
		renderers.Clear();
		dirty = false;
    }
    
    // Update is called once per frame
	void Update () {
		if ( dirty )
		{
			RecalculateMesh();
		}
	}
}


/*int index = i % batchSize;
			int batch = i / batchSize;

			if ( index == 0 ){
				if ( combine != null && combine.Length > 0 ) 
				{
					currentBatch.transform.GetComponent<MeshFilter>().mesh = new Mesh();
					currentBatch.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
					currentBatch.transform.gameObject.active = true;
				}

				combine = new CombineInstance[ System.Math.Min( meshFilters.Count, batchSize) ];

				while ( batch >= batches.Count ) {
					currentBatch = new GameObject();
					currentBatch.AddComponent<MeshFilter>();
					currentBatch.AddComponent<MeshRenderer>().materials = mats;
					currentBatch.transform.parent = this.transform.parent;
					currentBatch.name = this.name + "_BATCH_" + (batch);
					batches.Add(currentBatch);
				}
				currentBatch = batches[batch];
			}

			if ( meshFilters[i] == this.GetComponent<MeshFilter>() )
			{
				continue; 
			}

			combine[index].mesh = meshFilters[i].sharedMesh;
			combine[index].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].gameObject.active = false;
		}

		if ( combine.Length > 0 )
		{
			currentBatch.transform.GetComponent<MeshFilter>().mesh = new Mesh();
			currentBatch.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
			currentBatch.transform.gameObject.active = true;
        }
*/