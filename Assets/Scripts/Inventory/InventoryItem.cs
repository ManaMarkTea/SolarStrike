using UnityEngine;
using System.Collections;

/// <summary>
/// Inventory item. Reprents an item that can be placed into inventory slot.
/// The Item is a String that represents an item.  The game must understand how
/// to interpret this data.  The _icon is the visual represtation of the item.
/// </summary>
public class InventoryItem : MonoBehaviour {

	public string ItemId;
	private GameObject _icon;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if ( _icon != null )
		{
			//We dont' want the "icon" for a turret shooting! 
			//Since we cheated on icons, and used actual "mini" weapons. check for weapons and disarm them.
			IWeapon weap = _icon.GetComponent<IWeapon>();
			if ( weap == null )
			{
				weap = _icon.GetComponentInChildren<IWeapon>();
			}
			if ( weap != null )
			{
				weap.Armed = false;
			}

			var emitter = _icon.GetComponentInChildren<ParticleSystem>();
			if ( emitter != null )
			{
				emitter.emissionRate = 0;
			}

		}
	}

	/// <summary>
	/// Gets or sets the icon.
	/// Setting an icon will create a copy of the original since 
	/// we need to connect it to the GUI layer.
	/// </summary>
	/// <value>The icon.</value>
	public GameObject Icon 
	{ 
		get 
		{
			return _icon;
		}
		set 
		{
			if ( _icon != null )
			{
				_icon = null;
				Destroy(_icon);
			} 
			
			if( value != null )
			{
				_icon = Instantiate(value, this.transform.position, Quaternion.identity) as GameObject;
				_icon.gameObject.transform.parent = this.transform;
				_icon.SetActive(true);

				Util.SetLayer(_icon.transform, LayerMask.NameToLayer("GUI"));
				Util.DestroyAllColliders(_icon);

			}
			
			this.Update();
		} 
	}

}
