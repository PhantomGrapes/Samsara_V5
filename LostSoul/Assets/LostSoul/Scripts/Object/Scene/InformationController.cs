using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InformationController : Observer {
    public Text info;
    Inventory inventory;
	// Use this for initialization
	void Start () {
        info = GetComponent<Text>();
        inventory = FindObjectOfType<Inventory>();
        info.text = "";
	}
    public override void inventoryUpdate(ItemData o, string message) { }
    public override void inventoryUpdate(int o, string message)
    {
        if(message == "itemSelected")
        {
            if (o == -1)
            {
                info.text = "";
                return;
            }
            info.text = inventory.database.FetchItemById(o).Description;
        }
    }
}