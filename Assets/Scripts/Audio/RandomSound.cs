using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class RandomSound : MonoBehaviour {

	public AudioClip[] PossibleSounds;
	private AudioSource source;

	// Use this for initialization
	void Start () {
		this.source = GetComponent<AudioSource>();
	}

	public void PlayRandom()
	{
		int sound = Random.Range( 0, PossibleSounds.Length);
		source.clip = PossibleSounds[sound];
		source.Play();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
