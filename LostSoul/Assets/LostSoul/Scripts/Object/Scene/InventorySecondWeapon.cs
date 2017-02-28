using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventorySecondWeapon : Observer {
    Inventory inventory;
    public int current;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        current = -1;
    }

    public override void inventoryUpdate(ItemData o, string message)
    {
        if (message == "isSecondWeapon")
        {
            // don't care about weapon delete case
            if (o.getIsSecondWeapon())
            {
                if (GetComponent<Image>() == null)
                    this.gameObject.AddComponent<Image>();
                GetComponent<Image>().sprite = inventory.database.FetchItemById(o.id).Sprite;
                current = o.id;
            }
            /*
            // update if item is no more second weapon
            if (o.id == current && !o.getIsSecondWeapon())
            {
                if(GetComponent<Image>() != null)
                    Destroy(GetComponent<Image>());
                current = -1;
            }

            if (o.id != current && o.getIsSecondWeapon())
            {
                if(GetComponent<Image>() != null)
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
