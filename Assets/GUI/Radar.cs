using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Radar : MonoBehaviour {

	public Transform RadarPoint;
	public Transform RadarRotatePoint;

	public GameObject EnemyPoint;

	public Dictionary<GameObject, GameObject> radarMapping = new Dictionary<GameObject, GameObject>();


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RadarRotatePoint.rotation *= Quaternion.AngleAxis( Time.deltaTime * 360.0f, Vector3.down);

		var targets = GameObject.FindGameObjectsWithTag("Enemy");
		var player = GameObject.FindGameObjectWithTag("Player");

		for ( int i = 0; i < targets.Length; i++ )
		{
			var target = targets[i];

			var radarPos = target.transform.position - player.transform.position;
			radarPos *= 0.01f;
			radarPos.x *= -1.0f;
			radarPos.y = -1.0f;

			if ( radarMapping.ContainsKey(target) == false )
			{
				var targetPoint = GameObject.Instantiate(EnemyPoint, radarPos, Quaternion.identity) as GameObject; 
				radarMapping[target] = targetPoint;
				targetPoint.transform.parent = this.RadarPoint.transform;
				targetPoint.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
			}

			var radarPoint = radarMapping[target];
			radarPoint.transform.localPosition = radarPos;
			radarPoint.transform.rotation = Quaternion.identity;
			radarPoint.transform.RotateAround( this.RadarPoint.transform.position, Vector3.forward, player.transform.rotation.eulerAngles.y);

		}
	}
}
