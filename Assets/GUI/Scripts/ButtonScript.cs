using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	public MenuAction OnClick;

	public string ShortCutKey = "Escape";

	public Sprite Inactive;
	public Sprite Normal;
	public Sprite Highlight;

	private SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
		sprite = this.GetComponent<SpriteRenderer>();

		if ( OnClick == null )
		{
			this.OnClick = GetComponent<MenuAction>();
		}
	}
	
	// Update is called once per frame
	void Update () {

		sprite.sprite = Normal;

		if ( Input.GetButton(ShortCutKey) )
		{
			sprite.sprite = Highlight;
		}

		if ( Input.GetButtonUp(ShortCutKey) )
		{
			if ( OnClick != null )
			{
				OnClick.SelectAction(null);
			}
		}

		
		//Get the position of the mouse in screen space:
		RaycastHit hit ;
		Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
		Ray ray = uiCam.ScreenPointToRay( Util.getMousePosition() );

		if ( this.collider.Raycast (ray, out hit, 100000000)) {
			sprite.sprite = Highlight;

			if ( Util.IsMouseUp(0) )
			{
				if ( OnClick != null )
				{
					OnClick.SelectAction(null);
				}
			}

		}





	}



}
