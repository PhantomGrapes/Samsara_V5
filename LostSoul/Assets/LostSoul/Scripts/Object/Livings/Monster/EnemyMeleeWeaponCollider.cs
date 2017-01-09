using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMeleeWeaponCollider : MonoBehaviour {

	static MainCharacter player;

	public MainCharacter GetPlayer(){
		return player;
	}

	// Use this for initialization

	void Start () {

	}

	void FixedUpdate()
	{
//		foreach (MainCharacter player in enemyList)
//		{
//			if (player == null)
//				enemyList.Remove(player);
//		}
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<MainCharacter>() != null)
		{
			player = other.GetComponent<MainCharacter>();

		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.GetComponent<MainCharacter>() != null )
		{
			player = null;

		}
	}
}
