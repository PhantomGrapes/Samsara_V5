using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMeleeWeaponCollider : MonoBehaviour {

	private int counter = 0;

	public List<MainCharacter> enemyList = new List<MainCharacter>();

	// Use this for initialization

	void Start () {

	}

	void FixedUpdate()
	{
		foreach (MainCharacter player in enemyList)
		{
			if (player == null)
				enemyList.Remove(player);
		}
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<MainCharacter>() != null && !enemyList.Contains(other.GetComponent<MainCharacter>()))
		{
			enemyList.Add(other.GetComponent<MainCharacter>());

		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.GetComponent<MainCharacter>() != null && enemyList.Contains(other.GetComponent<MainCharacter>()))
		{
			enemyList.Remove(other.GetComponent<MainCharacter>());

		}
	}
}
