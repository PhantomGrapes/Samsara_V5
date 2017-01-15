using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleZone : MonoBehaviour {
	BoxCollider2D zone;
//	List<Monster> enemies;
//	MainCharacter player;
	public float enterBattleCountdown;
	bool inBattle = false;
	public float x_max;
	public float y_max;
	public float x_min;
	public float y_min;
	public float x_centre;
	public float y_centre;

	// Use this for initialization
	void Start () {

		// defines boundary 
		x_max = zone.bounds.max.x;
		x_min = zone.bounds.min.x;
		y_max = zone.bounds.max.y;
		y_min = zone.bounds.min.y;
		x_centre = zone.bounds.center.x;
		y_centre = zone.bounds.center.y;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator ActivateBattleZone(){
		yield return new WaitForSeconds (enterBattleCountdown);
		inBattle = true;
		// we can add codes that close the zone, summon minions, etc.
	}
}
