
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockSpawner))]
public class ProjectileEditor : Editor
{
	public override void OnPreviewGUI (Rect r, GUIStyle background)
	{
		base.OnPreviewGUI (r, background);
	}
	
	public override Texture2D RenderStaticPreview (string assetPath, Object[] subAssets, int width, int height)
	{
		Object[] assets = subAssets;
		
		Projectile child = this.target as Projectile;
		if ( child.ProjectilePrefab != null )
		{
			assets = new Object[subAssets.Length+1];
			subAssets.CopyTo(assets,0);
			assets[subAssets.Length-1] = child.ProjectilePrefab;
		}
		
		return base.RenderStaticPreview (assetPath, assets, width, height);
	}
}
	
	
/*
public class IFLTextureEditor : Editor
{		
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
		IFLTexture ifl = target as IFLTexture;

        if (GUI.changed){
			EditorUtility.SetDirty (target);
			ifl.LoadIFL();
		}
            
	}
	
}


*/