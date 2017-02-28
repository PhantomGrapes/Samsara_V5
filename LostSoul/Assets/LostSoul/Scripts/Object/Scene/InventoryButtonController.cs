using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryButtonController : Observer {
    LevelUpButtonController levelUp;
    MakeEssenceButtonController makeEssence;
    MainWeaponController mainWeapon;
    SecondWeaponController secondWeapon;
	// Use this for initialization
	void Start () {
        levelUp = FindObjectOfType<LevelUpButtonController>();
        makeEssence = FindObjectOfType<MakeEssenceButtonController>();
        mainWeapon = FindObjectOfType<MainWeaponController>();
        secondWeapon = FindObjectOfType<SecondWeaponController>();
        inventoryUpdate(-1, "itemSelected");
	}

    public override void inventoryUpdate(ItemData o, string message) { }
    public override void inventoryUpdate(int o, string message) {
        if(message == "itemSelected")
        {
            if(o == -1 || o == 13 || o == 14)
            {
                levelUp.gameObject.SetActive(false);
                makeEssence.gameObject.SetActive(false);
                mainWeapon.gameObject.SetActive(false);
                secondWeapon.gameObject.SetActive(false);
            }
            if(o == 12)
            {
                levelUp.gameObject.SetActive(false);
                makeEssence.gameObject.SetActive(true);
                mainWeapon.gameObject.SetActive(false);
                secondWeapon.gameObject.SetActive(false);
            }
            if (o > -1 && o < 6)
            {
                levelUp.gameObject.SetActive(true);
                makeEssence.gameObject.SetActive(false);
                mainWeapon.gameObject.SetActive(true);
                secondWeapon.gameObject.SetActive(true);
            }
            if(o > 5 && o < 12)
            {
                levelUp.gameObject.SetActive(false);
                makeEssence.gameObject.SetActive(false);
                mainWeapon.gameObject.SetActive(true);
                secondWeapon.gameObject.SetActive(true);
            }
        }
    }

}
