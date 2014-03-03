using UnityEngine;
using System.Collections;

public class Achievement : MonoBehaviour {

	protected TextMesh TextObject;	
	protected ParticleSystem Particles;

	public string Text = "";

	public float TimeToFadeIn = 0.25f;
	public float TimeToHold = 3.0f;
	public float TimeToFadeOut = 0.25f;

	private float currentTime = 0;

	public FadeState State;
	public enum FadeState  
	{
		FadeIn,
		FadeHold,
		FadeOut,
		Done,
	}
	
	// Use this for initialization
	void Start () {
		this.TextObject = this.GetComponentInChildren<TextMesh>();
		this.Particles = this.GetComponentInChildren<ParticleSystem>();
		this.TextProperty = Text;

		this.State = FadeState.FadeIn;

	}
	
	// Update is called once per frame
	void Update () {
		this.TextProperty = Text;

		this.currentTime += Time.deltaTime;

		switch ( State )
		{
		case FadeState.FadeIn:
		{
			var renderers = Util.FindRenderers(this.gameObject);
			float percent = currentTime / TimeToFadeIn;
			if ( percent >= 1.0f )
			{
				percent = 1.0f;
				this.State = FadeState.FadeHold;
				this.currentTime = 0.0f;
			}

			if ( this.Particles != null )
			{
				this.Particles.emissionRate = (300.0f * ( 1 - percent ));
			}

			for ( int i = 0; i < renderers.Count; i++ ) 
			{
				if ( renderers[i].material.HasProperty("_Color") )
				{
					var color = renderers[i].material.color;
					color.a = percent;
					renderers[i].material.color = color;
				}
			}
			break;
		}		
		case FadeState.FadeHold:
		{
			float percent = currentTime / TimeToHold;
			if ( percent >= 1.0f )
			{
				percent = 1.0f;
				this.State = FadeState.FadeOut;
				this.currentTime = 0.0f;
			}
			break;
		}		
		case FadeState.FadeOut:
		{
			float percent = currentTime / TimeToFadeOut;
			if ( percent >= 1.0f )
			{
				percent = 1.0f;
				this.State = FadeState.Done;
				this.currentTime = 0.0f;
			}

			if ( this.Particles != null )
			{
				this.Particles.emissionRate = (300.0f * ( percent ));
			}

			var renderers = Util.FindRenderers(this.gameObject);
			for ( int i = 0; i < renderers.Count; i++ ) 
			{
				if ( renderers[i].material.HasProperty("_Color") )
				{
					var color = renderers[i].material.color;
					color.a = 1.0f - percent;
					renderers[i].material.color = color;
				}
			}

			break;
		}
		case FadeState.Done:
		{
			bool okToKill = true;

			if ( Particles != null && Particles.particleCount != 0 )
			{
				this.Particles.emissionRate = 0;
				okToKill = false;
			}
			if ( okToKill )
			{
				GameObject.DestroyObject(this.gameObject);
			}
			break;
		}

		}
	}

	public string TextProperty { 
		set { 
			if ( this.TextObject != null && this.TextObject.text != value )
			{
				this.TextObject.text = value; 
			}
		}
		get { 
			if ( this.TextObject != null )
			{
				return this.TextObject.text; 
			}
			return "";
		}
	}

}
