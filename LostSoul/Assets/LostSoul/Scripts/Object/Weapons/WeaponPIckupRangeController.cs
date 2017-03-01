using UnityEngine;
using System.Collections;

public class WeaponPIckupRangeController : MonoBehaviour {

	MainCharacter player;

	// Use this for initialization
	void Start () {

		player = FindObjectOfType<MainCharacter> ();


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Weapon"))
		{
			//print ("1");
			//player.weaponToBePickedUp = col.gameObject;
		}
		else
		{
			//print ("2");
		}
	}

	// set the weapon to be unavailable for pickup when they no longer collide
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Weapon"))
		{
			//player.weaponToBePickedUp = null;
		}
	}
}
