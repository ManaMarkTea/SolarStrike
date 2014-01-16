using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IFLTexture))]
public class IFLTextureEditor : Editor
{		
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
		IFLTexture ifl = target as IFLTexture;

        if (GUI.changed){
			EditorUtility.SetDirty (target);
			LoadIFL();
		}
            
	}
	
	public void LoadIFL() 
	{
		IFLTexture ifl = target as IFLTexture;
		
		if ( ifl != null)
		{
			
			string [] lines = ifl.IFL.text.Split('\n');
			ifl.textures = new Texture[lines.Length];
			
			for ( int i = 0; i < lines.Length; i++ )
			{
				string path = AssetDatabase.GetAssetPath( ifl.IFL.GetInstanceID() );
				path = System.IO.Path.GetDirectoryName(path);
				string line = lines[i].Trim();
				Texture tx = Resources.LoadAssetAtPath( path + "/" + line, typeof(Texture) ) as Texture;
				ifl.textures[i] = tx;	
			}
		}
	}
}

