using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public GameObject inventoryPanel;
    private GameObject slotPanel;
    ItemDataBase database;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    int slotAmount;
    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        slotAmount = 32;
        database = GetComponent<ItemDataBase>();
        slotPanel = this.gameObject;
        //inventoryPanel = GameObject.Find("Inventory Panel");
        //slotPanel = inventoryPanel.transform.FindChild("SlotPanel").gameObject;
        for(int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            
            slots[i].transform.SetParent(slotPanel.transform);
            slots[i].transform.localScale = new Vector3(1, 1, 1);

        }
        GetComponent<SlotController>().Adjust();
        AddItem(1);
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemById(id);
        for(int i = 0; i < items.Count; i++)
        {
            if(items[i].Id == -1)
            {
                items[i] = itemToAdd;
                GameObject itemObj = Instantiate(inventoryItem);
                itemObj.transform.SetParent(slots[i].transform);
                itemObj.transform.position = Vector2.zero;
                itemObj.transform.localScale = Vector3.one;
                itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
                itemObj.name = itemToAdd.Name;
                return;
            }
        }
    }
}
