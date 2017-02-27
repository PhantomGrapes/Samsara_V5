using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public GameObject inventoryPanel;
    private GameObject slotPanel;
    public ItemDataBase database;
    public GameObject inventorySlot;
    public GameObject inventoryItem;
    public int itemSelected;

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
        AddItem(0);
        AddItem(0);
        AddItem(12);
        AddItem(12);
        AddItem(12);
        AddItem(12);
        AddItem(12);
        AddItem(12);
        AddItem(13);
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemById(id);
        int position = ItemPositionInInventory(itemToAdd);
        if (position > -1) {
            if (!itemToAdd.stackable)
                return;
            ItemData data = slots[position].transform.GetChild(0).GetComponent<ItemData>();
            data.amount++;
            data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
            return;
        }


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
                ItemData data = itemObj.GetComponent<ItemData>();
                data.item = itemObj;
                data.id = itemToAdd.Id;
                data.amount++;
                data.transform.GetChild(0).GetComponent<Text>().text = "1";
                return;
            }
        }
    }

    public void delItemById(int id)
    {
        Item itemToAdd = database.FetchItemById(id);
        int position = ItemPositionInInventory(itemToAdd);
        if (position == -1)
            return;
        ItemData data = slots[position].transform.GetChild(0).GetComponent<ItemData>();
        if(data.amount > 1)
        {
            data.amount--;
            data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
            return;
        }
        for(int i = position + 1; i < slots.Count; i++)
        {
            items[i - 1] = items[i];
            Transform oldItem = slots[i - 1].transform.GetChild(0);
            if (items[i].Id == -1)
            {
                Destroy(oldItem.gameObject);
                break;
            }
            Transform newItem = slots[i].transform.GetChild(0);
            oldItem.GetComponent<ItemData>().id = newItem.GetComponent<ItemData>().id;
            oldItem.GetComponent<ItemData>().amount = newItem.GetComponent<ItemData>().amount;
            oldItem.GetComponent<ItemData>().item = newItem.GetComponent<ItemData>().item;
            oldItem.GetChild(0).GetComponent<Text>().text = newItem.GetChild(0).GetComponent<Text>().text;
            oldItem.GetComponent<Image>().sprite = newItem.GetComponent<Image>().sprite;
            oldItem.name = newItem.name;
        }
    }

    int ItemPositionInInventory(Item item)
    {
        for(int i = 0; i< items.Count; i++)
        {
            if (items[i].Id == item.Id)
                return i;
        }
        return -1;
    }
}
