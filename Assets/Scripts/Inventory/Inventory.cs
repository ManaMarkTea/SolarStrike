using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Inventory system.  Controls its Slots and Items.
/// </summary>
public class Inventory : MonoBehaviour {
	public class InventoryRow {
		public List<InventorySlot> Slots;
		public int Index;
	}

	public GameObject InventorySlot;
	public bool visible;
	public int selectedRow = 0;
	public int selectedCol = 0;
	public int Columns = 0;
	public int Rows = 0;
	public KeyCode OpenInventoryKey = KeyCode.Tab ;


	public List<InventoryRow> slots = new List<InventoryRow>();
	public InventorySlot SelectedItem;

	/// <summary>
	/// Swaps the item given with the item @ the given location.
	/// </summary>
	/// <returns>The item.</returns>
	/// <param name="row">Row.</param>
	/// <param name="col">Col.</param>
	/// <param name="item">Item.</param>
	public InventoryItem SwapItem( int row, int col, InventoryItem item)
	{
		var slot = slots[row].Slots[col];
		var oldItem = slot.Item;
		slot.Item = item;

		return oldItem;
	}

	/// <summary>
	/// In order to properly interact with the inventory it must be "active"
	/// </summary>
	/// <returns><c>true</c>, if temp active was set, <c>false</c> otherwise.</returns>
	public bool SetTempActive() 
	{
		bool tempActive = this.gameObject.activeSelf;
		for ( int i = 0; i < this.transform.childCount; i++)
		{
			tempActive = this.transform.GetChild(i).gameObject.activeSelf;
			this.transform.GetChild(i).gameObject.SetActive(true);
		}
		return tempActive;
	}

	public void RestoreTempActive(bool state)
	{
		for ( int i = 0; i < this.transform.childCount; i++)
		{
			this.transform.GetChild(i).gameObject.SetActive(state);
		}
	}

	/// <summary>
	/// Gets the next open slot, it will return null if inventory is full.
	/// </summary>
	/// <returns>The open slot.</returns>
	public InventorySlot GetOpenSlot()
	{
		InventorySlot output = null;

		bool state = SetTempActive() ;

		for( int r = 0; r < this.slots.Count; r++ )
		{
			for ( int c = 0; c < this.slots[r].Slots.Count; c++ )
			{
				var slot = this.slots[r].Slots[c];
				if ( slot.Item == null )
				{
					output = slot;
					RestoreTempActive(state );
					return output;
				}
			}
		}

		RestoreTempActive(state);
		return output;
	}

	/// <summary>
	/// Gets the item slot with an item of itemID.
	/// Useful for checking to see if you already have an item in this container.
	/// </summary>
	/// <returns>The item slot with itemid in it.</returns>
	/// <param name="ItemId">Item identifier.</param>
	public InventorySlot GetItemSlotWithItem( string ItemId )
	{
		InventorySlot output = null;
		for( int r = 0; r < this.slots.Count; r++ )
		{
			for ( int c = 0; c < this.slots[r].Slots.Count; c++ )
			{
				var slot = this.slots[r].Slots[c];
				if ( slot.Item != null && slot.Item.ItemId == ItemId )
				{
					output = slot;
					return output;
				}
			}
		}
		
		return output;
	}

	/// <summary>
	/// Gets the item slot at the given position;
	/// </summary>
	/// <returns>The item slot.</returns>
	/// <param name="row">Row.</param>
	/// <param name="col">Col.</param>
	public InventorySlot GetItemSlot( int row, int col)
	{	
		var slot = slots[row].Slots[col];
		return slot;
	}
	
	// Use this for initialization
	void Start () {

		//This creates a mapping of the inventory system.
		//This specifically looks for "RowX" then "SlotY"

		for ( int i = 0; i < this.transform.childCount; i++)
		{
			var rowObj = this.transform.GetChild(i);
			if ( rowObj.name.StartsWith("Row") )
			{
				int rowIndex = int.Parse(rowObj.name.Substring("Row".Length));
				var row = new InventoryRow();
				row.Slots = new List<global::InventorySlot>();
				row.Index = rowIndex;
				slots.Add(row);

				for ( int c = 0; c < rowObj.childCount; c++)
				{
					var cellObj = rowObj.GetChild(c);
					if ( cellObj.name.StartsWith("Slot") )
					{
						int cellIndex = int.Parse(cellObj.name.Substring("Slot".Length));

						//Create the slot here, attach it to the marker.
						var slotObj = GameObject.Instantiate(InventorySlot, cellObj.transform.position, Quaternion.identity) as GameObject;
						slotObj.transform.parent = cellObj.transform;

						//Create an Invetory slot on the new object:
						var slot = slotObj.GetComponent<InventorySlot>();
						slot.IndexRow = rowIndex;
						slot.IndexCol = cellIndex;

						//Name it accordingly:
						slotObj.name = "Inventory_" + rowIndex + "_" + cellIndex;
						row.Slots.Add(slot);

						//The "Equipment" bar is special, it gets shortcut keys!
						if ( this.gameObject.tag == "Equipment" ) {
							if ( c < 9 ) {
								slot.ShortCutKey = KeyCode.Alpha1 + c;
							} else if ( c == 9 ) {
								slot.ShortCutKey = KeyCode.Alpha0;
							} else if ( c == 10 ) {
								slot.ShortCutKey = KeyCode.Minus;
							} else if ( c == 11 ) {
								slot.ShortCutKey = KeyCode.Equals;
							}
						}

						//When the slot is selected, trigger it:
						slot.OnSelected += (global::InventorySlot obj) => { 
							this.SelectedItem = obj; 
							TriggerSelectionChanged();
						};

					}
				}

				//Sort it and store it:
				row.Slots.Sort( (obj1, obj2) => {
					return obj1.IndexCol - obj2.IndexCol;
				});

			}
		}

		//Sort it and store it:
		slots.Sort( (rowA, rowB) => {
			return rowA.Index - rowB.Index;
		});

		this.Rows = slots.Count;
		this.Columns = slots[0].Slots.Count;
	}

	/// <summary>
	/// Selects the offset at the given location.
	/// If you overflow a col, it will move down a row, 
	/// If u overflow a row, it will move over a col.
	/// </summary>
	/// <returns>The offset.</returns>
	/// <param name="row">Row.</param>
	/// <param name="col">Col.</param>
	public InventorySlot SelectOffset( int row, int col )
	{
		int activeRow = 0;
		int activeCol = 0;

		if ( SelectedItem != null )
		{
			activeCol = SelectedItem.IndexCol;
			activeRow = SelectedItem.IndexRow;			
		}

		activeCol += col;
		activeRow += row;

		if ( activeCol < 0 ) activeCol = Columns - (-activeCol % Columns);
		if ( activeRow < 0 ) activeRow = Rows - (-activeRow % Rows);

		int rowSkip = activeCol / Columns;
		int colSkip = activeRow / Rows;

		activeCol = (activeCol + colSkip) % Columns;
		activeRow = (activeRow + rowSkip) % Rows;

		return slots[activeRow].Slots[activeCol];
	}

	/// <summary>
	/// Triggers the selection change event, Namely, marking the item
	/// as selected, and in this case, it tells the player to activate that
	/// weapon / item.
	/// </summary>
	public void TriggerSelectionChanged()
	{
		for( int r = 0; r < this.slots.Count; r++ )
		{
			for ( int c = 0; c < this.slots[r].Slots.Count; c++ )
			{
				this.slots[r].Slots[c].SetSelected(false);
			}
		}

		//if ( this.tag == "Equipment") 
		{
			if ( this.SelectedItem != null && this.SelectedItem.Item != null )
			{
				var player = GameObject.FindGameObjectWithTag("Player") as GameObject;
				var weapons = player.GetComponent<WeaponScript>();
				weapons.ChangeWeapon( this.SelectedItem.Item.ItemId);

				this.SelectedItem.SetSelected(true);
			}
		}
	}

	// Update is called once per frame
	void Update () {

		//When the inventory screen is up, we want to unlock and show the mouse. & vice versa when its gone.
		if ( this.tag == "Inventory") {		
			if ( Input.GetKeyDown(OpenInventoryKey) ) {
				this.visible = !this.visible;

				if ( this.visible ) {
					Screen.showCursor = true;
					Screen.lockCursor = false;
				} else {
					Screen.showCursor = false;
					Screen.lockCursor = true;
				}

			} 

			MouseLook.Paused = this.visible;
		}

		//The "Equipment" bar is special, it can mouse wheel scroll.
		if ( this.tag == "Equipment") 
		{
			float wheel = Input.GetAxis("Mouse ScrollWheel");
			if ( wheel != 0 ) 
			{		
				if ( this.SelectedItem != null )
				{
					this.SelectedItem.Selected = false;
				}

				SelectedItem = SelectOffset(0, (wheel > 0 ? -1 : 1));
				TriggerSelectionChanged();
			}
		}

		//Update the rows as needed
		for ( int i = 0; i < this.transform.childCount; i++)
		{
			this.transform.GetChild(i).gameObject.SetActive(this.visible);
		}
	}
}
