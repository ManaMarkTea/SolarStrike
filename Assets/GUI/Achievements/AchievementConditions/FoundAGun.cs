using UnityEngine;
using System.Collections;

public class FoundAGun : IAchievementCondition {
	
	public override bool IsUnlocked ()
	{
		bool complete = false;

		var equipObj = GameObject.FindGameObjectWithTag("Equipment");
		var inventoryObj = GameObject.FindGameObjectWithTag("Inventory");
		
		var equip = equipObj.GetComponent<Inventory>();
		var inv = inventoryObj.GetComponent<Inventory>();
		
		if ( equip.GetItemSlotWithItem("Gun_Pulse") != null ){
			complete = true;
		}
		if ( inv.GetItemSlotWithItem("Gun_Pulse") != null ){
			complete = true;
		}

		return complete;

	}

}
