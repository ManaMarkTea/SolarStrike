using UnityEngine;
using System.Collections;

public class FindBlockType : QuestState {

	public string BlockToFind = "PowerCell";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		var player = GameObject.FindGameObjectWithTag("Player");
		
		if ( GenerateWorld.World == null ) return ;
		
		var powerCell = GenerateWorld.World.FindNearestBlockOfType( player.transform.position, "PowerCell");

		//If the player walks within X units of a power cell, thats good enough for us!
		if ( powerCell != null )
		{
			var dist = player.transform.position - powerCell.transform.position;
			if ( dist.magnitude < 4 )
			{
				var shipObj = GameObject.FindGameObjectWithTag("PhalanxShip");
				if ( shipObj != null )
				{
					var ship = shipObj.GetComponent<PhalanxShip>();
					ship.PowerCell = powerCell;

					this.Complete = true;
					NotifyComplete();
				}
				
			}
		}
		
	}
}
