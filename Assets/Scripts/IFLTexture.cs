using UnityEngine;
using System;

/// <summary>
/// IFL texture.
/// Image file list.  Used to update an object to a given texture index.  Useful for animating textures.
/// </summary>
[ExecuteInEditMode]
public class IFLTexture : MonoBehaviour
{
	public bool UseRandomIndex;
	public TextAsset IFL;
	public Texture[] textures;
	public int TextureIndex;
	public int MaterialIndex;
	private int lastIndex;
	
	public bool Animate;
	public float FPS;	
	private float frameIndex;
	
	public IFLTexture ()
	{		
		lastIndex = -1;
		FPS = 30.0f;
	}
		
	void Start() {
		
		if ( UseRandomIndex ) 
		{		
			TextureIndex = UnityEngine.Random.Range(0, textures.Length);
		}
		
	}
		
	// Update is called once per frame	
	void Update () {
		
		if ( Animate )
		{
			frameIndex += Time.deltaTime * FPS;
			
			while ( frameIndex > 1.0f ) {
				TextureIndex++;
				frameIndex -= 1.0f;
				if ( TextureIndex > textures.Length )
				{
					TextureIndex = 0;
				}
			}
		}
		
		if ( lastIndex != TextureIndex )
		{			
			
			if ( TextureIndex >= 0 && TextureIndex < textures.Length )
			{			
				lastIndex = TextureIndex;	
				
				var renderers = Util.FindRenderers(gameObject);
				
				
				if ( Application.isPlaying ) 
				{
					for ( int r = 0; r < renderers.Count; r++)
					{					
						if ( MaterialIndex < renderers[r].materials.Length )
						{
							renderers[r].materials[MaterialIndex].mainTexture = textures[TextureIndex];					
						}
					}	
				}
			}
		}		
		
	}
		
}

