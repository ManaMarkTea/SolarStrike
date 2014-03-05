using UnityEngine;
using System.Collections;

public class PlaceAtScreenPos : MonoBehaviour {

	public Vector2 ScreenPos;

	// Use this for initialization
	void Start () {
		var camObj = GameObject.FindGameObjectWithTag("UICamera");
		if ( camObj != null )
		{
			Camera uiCam = camObj.GetComponent<Camera>();
			Vector3 pos2d = uiCam.WorldToViewportPoint( Vector3.zero);
			Vector3 pos = uiCam.ViewportToWorldPoint( new Vector3( ScreenPos.x, ScreenPos.y, pos2d.z ) );
			this.transform.position = pos;
		}

	}
	
	// Update is called once per frame
	void Update () {
		var camObj = GameObject.FindGameObjectWithTag("UICamera");
		if ( camObj != null )
		{
			Camera uiCam = camObj.GetComponent<Camera>();
			Vector3 pos2d = uiCam.WorldToViewportPoint( Vector3.zero);
			Vector3 pos = uiCam.ViewportToWorldPoint( new Vector3( ScreenPos.x, ScreenPos.y, pos2d.z ) );
			this.transform.position = pos;
		}

	}
}
