using System;
using UnityEngine;

	
public abstract class IWeapon : MonoBehaviour
{
	public string ItemId;
	public float CoolDown = 0.25f;
	public float CurrentCoolDown = 0;
	public bool Armed;
	public bool isPlayerWeapon;
	public GameObject LockOnTarget;

	public abstract GameObject Fire( GameObject Player);			
	public abstract GameObject Fire2( GameObject Player);
	public abstract GameObject Fire3( GameObject Player);
	
	public abstract void PutAway();

}

