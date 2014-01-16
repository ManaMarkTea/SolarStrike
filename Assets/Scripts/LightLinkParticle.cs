using UnityEngine;
using System.Collections;

/// <summary>
/// Links a light and a particle system.
/// Useful for explosions.
/// </summary>
public class LightLinkParticle : MonoBehaviour {
	
	public ParticleSystem Particles;
	public Light LightToControl;
	public float LightIntensity;
	public GameObject FadeObj;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		float alpha = (1.0f - (Particles.time / Particles.duration)) ;
		
		LightToControl.intensity = alpha * LightIntensity;
		
		if ( this.FadeObj != null )
		{
			this.FadeObj.renderer.material.color = new Color(1,1,1,alpha);
		}
		
	}
}
