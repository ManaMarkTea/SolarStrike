using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementSystem : MonoBehaviour {

	public Achievement AchievementPrefab;

	public List<IAchievementCondition> conditions;

	// Use this for initialization
	void Start () {
		this.conditions = new List<IAchievementCondition>();

		var allItems = this.GetComponents<IAchievementCondition>();
		for ( int i = 0; i < allItems.Length; i++ )
		{
			conditions.Add(allItems[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		for ( int i = 0; i < conditions.Count; i++ )
		{
			if (  conditions[i].Unlocked == false && conditions[i].IsUnlocked() ) 
			{
				conditions[i].Unlocked = true;
				var plaque = GameObject.Instantiate(AchievementPrefab, Vector3.zero, Quaternion.identity) as Achievement;
				plaque.transform.parent = this.transform;
				plaque.GetComponent<Achievement>().Text = conditions[i].Text;

			}
		}
	}
}
