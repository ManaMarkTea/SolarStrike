using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Flicker light, flickers a light, its kreeeeeeeepy.
/// </summary>
public class FlickerLight : MonoBehaviour {
	
	public float min = 3;
	public float intensity = 5;
	public float flickerRate = 10;
	public float flickerVar = 5;

	private float flicker;

	// Use this for initialization
	void Start () {
		flicker = UnityEngine.Random.value * flickerVar - (flickerVar * 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		Light l = this.GetComponent<Light>();
		l.intensity = Math.Max ( (float)(Math.Cos( Time.fixedTime * flicker) + 1) * 0.5f * intensity , min );
		
	}
}
