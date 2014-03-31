using UnityEngine;
using System.Collections;

public class GMenuItem : MonoBehaviour {

	public MainMenuScript menu;
	GUIText text;
	Color normalColor;
	public Color NormalColor { get { return normalColor; } }
	public Color HighlightColor = Color.cyan;

	// Use this for initialization
	void Start () {
		this.text = this.GetComponent<GUIText>();
		this.normalColor = this.text.color;
	}

	public void DoActions() 
	{
		var actions = GetComponents<MenuAction>();
		foreach ( var action in actions)
		{
			action.SelectAction(this);
		}

	}

	// Update is called once per frame
	void Update () {
		
		bool mousedOver = text.GetScreenRect().Contains(Input.mousePosition);
		
		if ( mousedOver ) 
		{
			if ( Input.GetMouseButtonUp(0) ) 
			{
				DoActions();
			}
			else 
			{
				menu.SelectGui(this);
			}


		}
		
		
	}
}
