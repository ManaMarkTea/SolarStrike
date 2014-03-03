using UnityEngine;
using System.Collections;

public abstract class IAchievementCondition : MonoBehaviour {

	public string ID;
	public string Text;
	public bool Unlocked;

	public abstract bool IsUnlocked();




}
