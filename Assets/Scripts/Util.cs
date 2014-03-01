using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using UnityEngine;
 
public static class Util
{
	public static bool IsUiActive() { 
		var inventory = GameObject.FindGameObjectWithTag("Inventory");
		if ( inventory != null )
		{
			var inv = inventory.GetComponent<Inventory>();
			return inv.visible;
		}
		return false;
	}

	public static List<Renderer> FindRenderers( GameObject obj)
	{
		List<Renderer> output = new List<Renderer>();
		output.AddRange(obj.GetComponents<Renderer>());		
		output.AddRange(obj.GetComponentsInChildren<Renderer>(true));
		return output;
	}
	
	public static BlockObject GetBlock( this GameObject obj )
	{
		BlockObject output = obj.GetComponent<BlockObject>();
		if ( output == null )
		{
			//Go up!
			Transform trans = obj.transform;
			while ( trans != null )
			{
				output = trans.GetComponent<BlockObject>();
				if ( output != null ) {
					break;
				}
				trans = trans.parent;
			}
		}

		return output;
	}

	/// <summary>
	/// Iteratively searches for a BLOCK object in the object
	/// tree.
	/// </summary>
	/// <returns>The block in children.</returns>
	/// <param name="obj">Object.</param>
	public static BlockObject GetBlockInChildren( this GameObject obj )
	{
		BlockObject output = obj.GetComponent<BlockObject>();
		if ( output == null )
		{
			Stack<Transform> stack = new Stack<Transform>();
			stack.Push(obj.transform);
			
			while ( stack.Count > 0 ) 
			{
				var top = stack.Pop();

				output = top.gameObject.GetComponent<BlockObject>();
				if ( output != null )
				{
					break; 
				}
				
				for ( int i = 0; i < top.childCount; i++ )
				{
					var child = top.GetChild(i);
					stack.Push(child);
				}
			}
		}

		return output;
	}

	/// <summary>
	/// Iteratively walks the stack, setting all children's layer value.
	/// </summary>
	/// <param name="node">Node.</param>
	/// <param name="layer">Layer.</param>
	public static void SetLayer( Transform node, int layer)
	{
		Stack<Transform> stack = new Stack<Transform>();
		stack.Push(node);

		while ( stack.Count > 0 ) 
		{
			var top = stack.Pop();
			top.gameObject.layer = layer;

			for ( int i = 0; i < top.childCount; i++ )
			{
				var child = top.GetChild(i);
				stack.Push(child);
			}
		}
	}


	public static void DisableAllColliders( GameObject obj )
	{
		var previewColider = obj.collider;
		if ( previewColider != null )
		{
			previewColider.enabled = false;
		} 					
		
		var coliders = obj.GetComponentsInChildren<Collider>(true);
		for( int i = 0; i < coliders.Length; i++ ) 
		{				
			coliders[i].enabled = false;
		}
	}
	
	/// <summary>
	/// Destroies all colliders. Why? BUG: Its not enough to just disable the collider in some cases.
	/// </summary>
	/// <param name='obj'>
	/// Object.
	/// </param>
	public static void DestroyAllColliders( GameObject obj )
	{
		var previewColider = obj.collider;
		if ( previewColider != null )
		{
			previewColider.enabled = false;
		} 					
		
		var coliders = obj.GetComponentsInChildren<Collider>(true);
		for( int i = 0; i < coliders.Length; i++ ) 
		{				
			coliders[i].enabled = false;
			
			//Strange bug, its not enough to just disable the collider in some cases:
			//Delete it!!
			Collider.Destroy(coliders[i]);
		}
	}
	
	public static bool IsProjectile( this GameObject obj )
	{
		bool output = false;
		Transform trans = obj.transform;
		while ( trans != null )
		{
			if ( trans.GetComponent<Projectile>() != null ) { 
				output = true; 
				break;
			}
			trans = trans.parent;
		}
		
		return output;
	}



	static bool lastTouch;
	public static bool IsMouseUp(int button)
	{
		bool touchOn = Input.touchCount != 0;
		if (lastTouch == true && touchOn == false) 
		{
			lastTouch = touchOn;
			return true;
		}
		return Input.GetMouseButtonUp (button);
	}
	
	public static bool IsMouseDown(int button)
	{
		bool touchOn = Input.touchCount != 0;
		if (lastTouch == false && touchOn) {
			return true;
		}
		return Input.GetMouseButtonDown (button);
	}
	
	public static bool IsMouse(int button)
	{
		bool touchOn = Input.touchCount != 0;
		return Input.GetMouseButton (button) || touchOn;
	}
	
	public static Vector3 getMousePosition () {
		bool touchOn = Input.touchCount != 0;
		if (touchOn)
		{
			return new Vector3( Input.touches[0].rawPosition.x, Input.touches[0].rawPosition.y, 0);
		}
		return Input.mousePosition;
	}



	
#if UNITY_STANDALONE_WIN
	
	[DllImport("user32.dll")]
	static extern IntPtr SetWindowLong (IntPtr hwnd,int  _nIndex ,int  dwNewLong);

	[DllImport("user32.dll")]
	static extern IntPtr FindWindow (IntPtr hwnd, string name);
	
	[DllImport("user32.dll")]
	static extern bool SetWindowPos (IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
	[DllImport("user32.dll")]
	static extern IntPtr GetForegroundWindow ();

	const uint SWP_NOMOVE = 0x2;
	const uint SWP_NOSIZE = 1;
	const uint SWP_NOZORDER = 0x4;
	const uint SWP_HIDEWINDOW = 0x0080;
	 
	const uint SWP_SHOWWINDOW = 0x0040;
	const int GWL_STYLE = -16;
	const int WS_BORDER = 1;
#endif 


	public static void GoFullscreenBorderless ()
	{	
#if UNITY_STANDALONE_WIN		
		var win = FindWindow(IntPtr.Zero, "BlasterBounce");
		if ( win != IntPtr.Zero )
		{
			int w = Screen.currentResolution.width;
			int h = Screen.currentResolution.height;
			SetWindowLong(win, GWL_STYLE, WS_BORDER);			
			var result = SetWindowPos (win, 0, 0, 0, (int)w, h, SWP_SHOWWINDOW);
		}
#endif		
	}


	
	
}
