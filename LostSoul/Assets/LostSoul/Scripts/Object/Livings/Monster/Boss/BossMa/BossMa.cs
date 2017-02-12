using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class BossMa : MainCharacter {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	// Overriding functions in Maincharacter
	public override void giveDefaultDamageToEnemy()
	{
		//print("give damage");
		List<MainCharacter> playerList = new List<MainCharacter>();
		if (weaponEquiped)
		{
			switch (weaponEquiped.GetComponent<Weapon>().index)
			{
			case 1:
				playerList = defaultWeaponRange_1.playerList;
				break;
			case 3:
				playerList = defaultWeaponRange_3.playerList;
				break;
			case 4:
				playerList = defaultWeaponRange_4.playerList;
				break;
			case 5:
				playerList = defaultWeaponRange_5.playerList;
				break;
			}
		}
		foreach (MainCharacter target in playerList)
		{
			target.BeAttacked(attack);
			target.beingAttacked = true;
			//print(checkWeaponSkill5);

		}
	}


}
