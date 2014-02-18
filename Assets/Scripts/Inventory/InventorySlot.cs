using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Inventory slot. An inventory system is made of slots.
/// Slots contain Inventory Items.
/// A slot can be empty, it can have a shortcut key.
/// You can drag it, and drop it, which will swap the items between
/// the drag source and drag target.
/// </summary>
public class InventorySlot : MonoBehaviour {

	public bool Selected;

	protected static InventorySlot ItemSlotInDrag;
	protected bool preDragging;
	protected bool dragging;
	protected Vector3 dragStart;


	public KeyCode ShortCutKey = KeyCode.None;

	public int IndexRow;
	public int IndexCol;
	public Quaternion rotation = Quaternion.identity;
	public float scale = 0.5f;

	/// <summary>
	/// Gets or sets the item.
	/// This is a live representation! The inventory item is a component of a child, so
	/// it must be connected and ACTIVE to get it.
	/// </summary>
	/// <value>The item.</value>
	public InventoryItem Item {
		get { 
			return this.GetComponentInChildren<InventoryItem>();
		}
		set {
			if ( value != null )
			{
				//Dettach the previous item:
				while ( this.transform.childCount > 0 )
				{
					this.transform.GetChild(0).parent = null;
				}

				//attach and reposition the new child:
				value.transform.parent = this.transform;
				value.transform.localPosition = Vector3.zero;
			}
		}
	}

	public event Action<InventorySlot> OnSelected;

	private TextMesh text;

	// Use this for initialization
	void Start () {
		this.text = this.GetComponentInChildren<TextMesh>();
		this.text.text = "";
	}

	public void NotifySelected() 
	{
		var handler = OnSelected;
		if ( handler != null )
		{
			handler(this);
		}
	}

	//Trying to add TOUCH support ( windows 8, tablets )

	bool lastTouch;
	bool IsMouseUp(int button)
	{
		bool touchOn = Input.touchCount != 0;
		if (lastTouch == true && touchOn == false) 
		{
			this.lastTouch = touchOn;
			return true;
		}
		return Input.GetMouseButtonUp (button);
	}

	bool IsMouseDown(int button)
	{
		bool touchOn = Input.touchCount != 0;
		if (lastTouch == false && touchOn) {
			return true;
		}
		return Input.GetMouseButtonDown (button);
	}

	bool IsMouse(int button)
	{
		bool touchOn = Input.touchCount != 0;
		return Input.GetMouseButton (button) || touchOn;
	}

	Vector3 getMousePosition () {
		bool touchOn = Input.touchCount != 0;
		if (touchOn)
		{
			return new Vector3( Input.touches[0].rawPosition.x, Input.touches[0].rawPosition.y, 0);
		}
		return Input.mousePosition;
	}

	// Update is called once per frame
	void Update () {

		if ( this.ShortCutKey != KeyCode.None )
		{
			this.text.text = this.ShortCutKey.ToString().Replace("Alpha", "");

		}


		//Get the position of the mouse in screen space:
		RaycastHit hit ;
		Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
		Ray ray = uiCam.ScreenPointToRay(getMousePosition());  

		//Check to see if we pressed the shortcut key for this item:
		if ( dragging == false && preDragging == false && Input.GetKeyDown(ShortCutKey) ) 
		{
			SetSelected(true);
			NotifySelected();		
		}

		//Pre-drag condition: This happens when we click and hold the mouse, but we haven't moved it far enough yet:
		if ( IsMouseDown (0)) {
			
			if ( this.Item != null && this.Item.Icon != null && this.collider.Raycast (ray, out hit, 100000000)) {
				//Debug.LogWarning("Pre - Dragging");
				InventorySlot.ItemSlotInDrag = null;
				dragStart = getMousePosition();
				preDragging = true;
			}
		}

		//We're still moving the mouse, haven't let go yet.
		if (preDragging && IsMouse(0)) {
			var diff = dragStart - getMousePosition();

			//We've moved far enough, lets trigger a drag rather than a press:
			if ( diff.magnitude > 5 ) {
				Debug.LogWarning("Dragging Started");
				dragging = true;
				preDragging = false;
				InventorySlot.ItemSlotInDrag = this;
			}
		}

		//We're going to place the icon at the mouse (If there is one there)
		if ( dragging ) 
		{
			if ( this.Item != null && this.Item.Icon != null )
			{
				this.Item.Icon.transform.position = new Vector3(ray.origin.x, ray.origin.y, this.Item.Icon.transform.position.z); 
			}
		}


		//Mouse Click: So the user let go before moving the mouse far enough
		//Lets consider this a CLICK, rather than a drag.
		if (preDragging && dragging == false && IsMouseUp(0) )
		{
			if ( this.collider.Raycast(ray, out hit, 100000000))
			{
				//Clicked on us!
				SetSelected(true);
				NotifySelected();
			} 
			else 
			{
				//Clicked on someone else.
				SetSelected(false);
			}
		}

		//On Mouse released: Well lets clean up, if it was a drag we need to swap some items:
		if (IsMouseUp (0))
		{
			Debug.LogWarning("Drag ended");

			//Moves the item being dragged to the new slot.
			if ( this.collider.Raycast(ray, out hit, 100000000))
			{
				//An item was dragged ONTO THIS SLOT!

				if ( InventorySlot.ItemSlotInDrag != null )
				{
					//Was it us dropping it on ourselves?
					if ( this != InventorySlot.ItemSlotInDrag )
					{
						//Take the item that is being dragged and give ours to the slot that was dragged to us.
						var temp = this.Item;
						this.Item = InventorySlot.ItemSlotInDrag.Item;
						InventorySlot.ItemSlotInDrag.Item = temp;

						//Snap lock:
						this.Item.Icon.transform.localPosition = Vector3.zero;
					}
				}

				InventorySlot.ItemSlotInDrag = null;
			}

			//We were the ones being dragged and now thats been handled, so reset our position and conditions.
			if ( dragging && this.Item != null ) 
			{
				this.Item.Icon.transform.localPosition = Vector3.zero;
			}

			preDragging = false;
			dragging = false;

		}

	}

	public void SetSelected(bool state) 
	{
		if ( state )
		{
			this.Selected = true;
			this.renderer.material.color = Color.red;
		} 
		else 
		{
			this.Selected = false;
			this.renderer.material.color = Color.white;
		}
	}
	



}
