using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class VoxelArgs : EventArgs
{
	public BlockIndex Index { get; set; }
	public GameObject Block { get; set; }
	public VoxelArgs(BlockIndex index, GameObject Block)
	{
		this.Index = index;
		this.Block = Block;
	}
}


public class VoxelCube
{	
	public event Action<VoxelArgs> CellAdded;
	public event Action<VoxelArgs> CellRemoved;
	public event Action<VoxelArgs> CellChanged;
	
	
	public Dictionary<BlockIndex, GameObject> Cells;	
	
	
	public VoxelCube ()
	{		
		Cells = new Dictionary<BlockIndex, GameObject>();
	}
	
	public GameObject this[BlockIndex key]
	{
		get 
		{
			if ( Cells.ContainsKey(key) )
			{
				return Cells[key];
			}
			
			return null;
		}
		
		set 
		{ 			
			bool needsAdd = true;
			
			if ( value == null ) 
			{
				if (Cells.ContainsKey(key))
				{
					var current = Cells[key];
					var removeHandler = CellRemoved;
					if ( removeHandler != null ) { removeHandler(new VoxelArgs(key, current)); }
					Cells.Remove(key);
				}
				return;
			}
			
			if ( Cells.ContainsKey(key) )
			{
				var current = Cells[key];
				if ( current != value) 
				{
					var removeHandler = CellRemoved;
					if ( removeHandler != null ) { removeHandler(new VoxelArgs(key, current)); }
				}
				else 
				{
					needsAdd = false;
				}
			} 
			
			if ( needsAdd ) 
			{			
				Cells[key] = value;
				var block = value.GetBlock();
				if ( block != null )
				{
					block.Index = key;
				}
				
				var addHandler = CellAdded;
				if ( addHandler != null ) { addHandler(new VoxelArgs(key, value)); }
			}
			
		}
	}
	
	public GameObject this[Vector3 index]
	{
		get 
		{ 
			int x = (int)Math.Round(index.x);
			int y = (int)Math.Round(index.y);
			int z = (int)Math.Round(index.z);
			BlockIndex key = new BlockIndex(x,y,z);
			
			if ( Cells.ContainsKey(key) )
			{
				return Cells[key];
			}
			
			return null;
		}
		
		set { 
			int x = (int)Math.Round(index.x);
			int y = (int)Math.Round(index.y);
			int z = (int)Math.Round(index.z);
			BlockIndex key = new BlockIndex(x,y,z);
			bool needsAdd = true;
			
			if ( value == null ) 
			{
				if (Cells.ContainsKey(key))
				{
					var current = Cells[key];
					var removeHandler = CellRemoved;
					if ( removeHandler != null ) { removeHandler(new VoxelArgs(key, current)); }
					Cells.Remove(key);
				}
				return;
			}
			
			if ( Cells.ContainsKey(key) )
			{
				var current = Cells[key];
				if ( current != value) 
				{
					var removeHandler = CellRemoved;
					if ( removeHandler != null ) { removeHandler(new VoxelArgs(key, current)); }
				}
				else 
				{
					needsAdd = false;
				}
			} 
			
			if ( needsAdd ) 
			{				
				Cells[key] = value;
				var addHandler = CellAdded;
				if ( addHandler != null ) { addHandler(new VoxelArgs(key, value)); }
			}
			
		}
	}
	
	public void Rotate( BlockIndex index )
	{
		if ( Cells.ContainsKey(index) )
		{
			var current = Cells[index];
			current.transform.Rotate(0, 90, 0);
			
			var rotationHandler = CellChanged;
			if ( rotationHandler != null ) { rotationHandler( new VoxelArgs(index, current) ); }
			
		}
		
	}
	
	public BlockIndex GetCellIndex(ref Vector3 point )
	{
		int x = (int)Math.Round(point.x);		
		int y = (int)Math.Round(point.y);
		int z = (int)Math.Round(point.z);		
		return new BlockIndex(x,y,z);
	}	
	public BlockIndex GetCellIndex(int x, int y, int z)
	{
		return new BlockIndex(x,y,z);
	}	
}
