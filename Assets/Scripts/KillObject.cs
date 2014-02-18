using UnityEngine;
using System.Collections;

/// <summary>
/// Destroys an object after X seconds. Fades it out as it goes.
/// </summary>
public class KillObject : MonoBehaviour {
	
	public float TimeTillKill;
	public bool FadeOut;
	public Shader FadeShader;
	private float timeSoFar;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timeSoFar += Time.deltaTime;
		
		if ( FadeOut )
		{
			float alpha = 1.0f - (timeSoFar / TimeTillKill);
			var renderers = gameObject.GetComponents<Renderer>();
			if ( renderers == null || renderers.Length == 0 ) {
				renderers = gameObject.GetComponentsInChildren<Renderer>();				
			}
			
			if ( renderers != null )
			{
				for ( int i = 0 ; i < renderers.Length; i++ )
				{
					for( int m = 0; m < renderers[i].materials.Length; m++ )
					{
						if ( renderers[i].materials[m].HasProperty("color") )
						{
							Color color = renderers[i].materials[m].color;
							color.a = alpha;
							renderers[i].material.shader = FadeShader;
							renderers[i].materials[m].color = color;
						}
					}
				}
			}
			
		}
		
		if ( timeSoFar >= TimeTillKill ) 
		{
			DestroyObject(this.gameObject);
			
		}
		
	}
}
