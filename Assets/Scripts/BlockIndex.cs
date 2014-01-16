using UnityEngine;
using System.Collections;

/// <summary>
/// Block index: A simple representation of an index in the dictionary, X Y Z based.
/// </summary>
public struct BlockIndex {
	public int x { get; set;}
	public int y { get; set;}
	public int z { get; set;}
	private static int MaxSize = 32768;
	private static float inverseCellSize = 1.0f/32768.0f;
	
	public BlockIndex(int x, int y, int z) : this()
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	/*
	 * SLOW!  Useful for hasing into a uniform grid, but we dont need that right now, just use dictionary.
	public override int GetHashCode ()
	{				
		int xHash, yHash, zHash;
		int mask = MaxSize - 1;
		
        if (x < 0) { float i = (-x) * inverseCellSize; xHash = MaxSize - 1 - (int)i & mask; }
        else { float i = (x) * inverseCellSize; xHash = (int)i & mask; }

        if (y < 0) { float i = (-y) * inverseCellSize; yHash = MaxSize - 1 - (int)i & mask; }
        else { float i = (y) * inverseCellSize; yHash = (int)i & mask; }

        if (z < 0) { float i = (-z) * inverseCellSize; zHash = MaxSize - 1 - (int)i & mask; }
        else { float i = (z) * inverseCellSize; zHash = (int)i & mask; }
        
        int finalIndex = xHash + yHash * MaxSize + zHash * MaxSize * MaxSize;
		return finalIndex;
	}	
	*/

	public override bool Equals (object obj)
	{
		BlockIndex o2 = (BlockIndex)obj;
		return  o2.x == this.x &&
					o2.y == this.y &&
					o2.z == this.z;
	}		
	
	public static bool operator ==(BlockIndex o1, BlockIndex o2)
	{
	    return  o2.x == o1.x &&
				o2.y == o1.y &&
				o2.z == o1.z;
	}
	
	public static bool operator !=(BlockIndex o1, BlockIndex o2)
	{
	    return  o2.x != o1.x ||
				o2.y != o1.y ||
				o2.z != o1.z;
	}

	public override string ToString ()
	{
		return string.Format ("[BlockIndex: x={0}, y={1}, z={2}]", x, y, z);
	}
}
