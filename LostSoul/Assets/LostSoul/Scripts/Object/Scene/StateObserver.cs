using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StateObserver : Observer {
    public int itemId;
    Inventory inventory;

	// Use this for initialization
	void Start () {
        inventory = FindObjectOfType<Inventory>();
	}

    public override void inventoryUpdate(ItemData o, string message) {
        print("notify medicine");
        if(message == "amount" && o.id == itemId)
        {
            GetComponent<Transform>().GetChild(0).GetComponent<Text>().text = o.getAmount().ToString();
        }
    }
    public override void inventoryUpdate(int o, string message) { }
}
