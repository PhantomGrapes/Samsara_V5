using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryMainWeapon : Observer {
    Inventory inventory;
    public int current;
    MainCharacter player;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        current = -1;
        player = FindObjectOfType<MainCharacter>();
    }

    public override void inventoryUpdate(ItemData o, string message) {
        if(message == "isMainWeapon")
        {
            // don't care about weapon delete case
            if (o.getIsMainWeapon())
            {
                if (GetComponent<Image>() == null)
                    this.gameObject.AddComponent<Image>();
                Item weapon = inventory.database.FetchItemById(o.id);
                GetComponent<Image>().sprite = weapon.Sprite;
                current = o.id;
                // need to add animation for seconde weapon
                player.anim.SetInteger("WeaponIndex", o.id > 6?o.id-6:o.id);
                player.weaponSprite.sprite = weapon.RealSprite;
                player.attack = weapon.attack;
            }
            /*
            // update if item is no more main weapon
            if (o.id == current && !o.getIsMainWeapon())
            {
                if(GetComponent<Image>() != null)
                    Destroy(GetComponent<Image>());
                current = -1;
            }

            if (o.id != current && o.getIsMainWeapon())
            {
                if (GetComponent<Image>() != null)
                    Destroy(GetComponent<Image>());
                Image image = this.gameObject.AddComponent<Image>();
                image.sprite = inventory.database.FetchItemById(o.id).Sprite;
                current = o.id;
            }
            */
        }
    }
    public override void inventoryUpdate(int o, string message) { }
}
