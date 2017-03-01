using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class Inventory : Observable {

    public GameObject inventoryPanel;
    private GameObject slotPanel;
    public ItemDataBase database;
    public GameObject inventorySlot;
    public GameObject inventoryItem;
    private int itemSelected;
    public int tauxSoulEssence = 5;
    public InventoryMainWeapon mainWeapon;
    public InventorySecondWeapon secondWeapon;
    bool initialized;
    int slotAmount;
    public List<Item> items;
    public List<GameObject> slots;
    MainCharacter player;


    void Start()
    {
        initialized = false;
        itemSelected = -1;
        slotAmount = 32;
        GetComponent<ItemDataBase>().Start();
        database = GetComponent<ItemDataBase>();
        mainWeapon = FindObjectOfType<InventoryMainWeapon>();
        secondWeapon = FindObjectOfType<InventorySecondWeapon>();
        slotPanel = FindObjectOfType<SlotController>().gameObject;
        items = new List<Item>();
        slots = new List<GameObject>();
        player = FindObjectOfType<MainCharacter>();
        initializeBroadcast();
        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));

            slots[i].transform.SetParent(slotPanel.transform);
            slots[i].transform.localScale = new Vector3(1, 1, 1);

        }
        slotPanel.GetComponent<SlotController>().Adjust();
        AddItem(13);
        AddItem(13);
        AddItem(13);
        AddItem(13);
        AddItem(13);
        AddItem(13);
        AddItem(14);
        AddItem(14);
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemById(id);
        int position = itemPositionInInventory(itemToAdd.Id);
        if (position > -1) {
            if (!itemToAdd.stackable)
                return;
            ItemData data = slots[position].transform.GetChild(0).GetComponent<ItemData>();
            data.setAmount(data.getAmount() + 1);
            return;
        }

        for(int i = 0; i < items.Count; i++)
        {
            if(items[i].Id == -1)
            {
                items[i] = itemToAdd;
                GameObject itemObj = Instantiate(inventoryItem);
                itemObj.transform.SetParent(slots[i].transform);
                itemObj.transform.position = slots[i].transform.position;
                itemObj.transform.localScale = Vector3.one;
                itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
                itemObj.name = itemToAdd.Name;
                ItemData data = itemObj.GetComponent<ItemData>();
                data.id = itemToAdd.Id;
                data.initializeBroadcast();
                data.setAmount(data.getAmount() + 1);
                data.setAttack(itemToAdd.attack);
                data.setIsMainWeapon(false);
                data.setIsSecondWeapon(false);
                return;
            }
        }
    }

    public void delItemById(int id)
    {
        Item itemToAdd = database.FetchItemById(id);
        int position = itemPositionInInventory(itemToAdd.Id);
        if (position == -1)
            return;
        ItemData data = slots[position].transform.GetChild(0).GetComponent<ItemData>();
        data.setAmount(data.getAmount() - 1);
        if (data.getAmount() >= 1)
            return;
        if (getItemSelected() == items[position].Id)
            setItemSelected(-1);
        for (int i = position + 1; i < slots.Count; i++)
        {
            items[i - 1] = items[i];
            ItemData oldItem = slots[i - 1].transform.GetChild(0).GetComponent<ItemData>();
            if (items[i].Id == -1)
            {
                Destroy(oldItem.gameObject);
                break;
            }
            ItemData newItem = slots[i].transform.GetChild(0).GetComponent<ItemData>();
            oldItem.id = newItem.id;
            oldItem.setAmount(newItem.getAmount());
            oldItem.setAttack(newItem.getAttack());
            oldItem.setIsMainWeapon(newItem.getIsMainWeapon());
            oldItem.setIsSecondWeapon(newItem.getIsSecondWeapon());
            oldItem.GetComponent<Image>().sprite = newItem.GetComponent<Image>().sprite;
            oldItem.name = newItem.name;
        }
        
    }


    public void eventMakeEssence()
    {
        int soulPos;
        soulPos = itemPositionInInventory(13);
        if (soulPos == -1)
            return;

        if (slots[soulPos].transform.GetChild(0).GetComponent<ItemData>().getAmount() >= tauxSoulEssence)
        {
            for (int i = 0; i < tauxSoulEssence; i++)
            {
                delItemById(13);

            }
            AddItem(14);
        }
    }

    public void eventSetMainWeapon()
    {
        //annual old main weapon if exist
        if(mainWeapon.current != -1)
            slots[itemPositionInInventory(mainWeapon.current)].transform.GetChild(0).GetComponent<ItemData>().setIsMainWeapon(false);

        //add new main weapon
        slots[itemPositionInInventory(itemSelected)].transform.GetChild(0).GetComponent<ItemData>().setIsMainWeapon(true);
    }

    public void eventSetMainWeaponById(int id)
    {
        //annual old main weapon if exist
        if (mainWeapon.current != -1)
            slots[itemPositionInInventory(mainWeapon.current)].transform.GetChild(0).GetComponent<ItemData>().setIsMainWeapon(false);

        //add new main weapon
        slots[itemPositionInInventory(id)].transform.GetChild(0).GetComponent<ItemData>().setIsMainWeapon(true);
    }

    public void eventSetSecondWeapon()
    {
        //annual old second weapon if exist
        if(secondWeapon.current != -1)
            slots[itemPositionInInventory(secondWeapon.current)].transform.GetChild(0).GetComponent<ItemData>().setIsSecondWeapon(false);
        //add new second weapon
        slots[itemPositionInInventory(itemSelected)].transform.GetChild(0).GetComponent<ItemData>().setIsSecondWeapon(true);
    }

    public void eventSetSecondWeaponById(int id)
    {
        //annual old second weapon if exist
        if (secondWeapon.current != -1)
            slots[itemPositionInInventory(secondWeapon.current)].transform.GetChild(0).GetComponent<ItemData>().setIsSecondWeapon(false);
        //add new second weapon
        slots[itemPositionInInventory(id)].transform.GetChild(0).GetComponent<ItemData>().setIsSecondWeapon(true);
    }

    public void eventLevelUp()
    {
        int esPosition = itemPositionInInventory(14);
        if (esPosition == -1)
            return;
        delItemById(14);

        int position = itemPositionInInventory(itemSelected);
        items[position] = database.FetchItemById(itemSelected + 6);
        slots[position].transform.GetChild(0).GetComponent<Image>().sprite = items[position].Sprite;
        ItemData data = slots[position].transform.GetChild(0).GetComponent<ItemData>();
        data.id = items[position].Id;
        data.name = items[position].Name;
        data.setAttack(items[position].attack);
        data.setIsMainWeapon(data.getIsMainWeapon());
        data.setIsSecondWeapon(data.getIsSecondWeapon());
        setItemSelected(data.id);
    }

    public int itemPositionInInventory(int id)
    {
        for(int i = 0; i< items.Count; i++)
        {
            if (items[i].Id == id)
                return i;
        }
        return -1;
    }

    public int getItemSelected()
    {
        return itemSelected;
    }

    public void setItemSelected(int s)
    {
        itemSelected = s;
        notifyChanges(s, "itemSelected");
    }
}
