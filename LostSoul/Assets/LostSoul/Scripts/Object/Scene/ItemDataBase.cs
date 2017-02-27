using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ItemDataBase : MonoBehaviour {
    List<Item> database = new List<Item>();
    JsonData itemData;

    void Start()
    {
        itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/LostSoul/Scripts/Object/Scene/Items.json"));
        ConstructItemDatabase();

        //print(FetchItemById(0).Description);
    }

    public Item FetchItemById(int id)
    {
        for(int i = 0; i < database.Count; i++)
        {
            if (database[i].Id == id)
                return database[i];
        }
        return null;
    }

    void ConstructItemDatabase()
    {
        for(int i = 0; i < itemData.Count; i++)
        {
            database.Add(new Item((int)itemData[i]["id"], itemData[i]["name"].ToString(), itemData[i]["description"].ToString(), (bool)itemData[i]["stackable"]));
        }
    }
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool stackable { get; set; }
    public Sprite Sprite { get; set; }

    public Item(int id, string name, string description, bool stackable)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Sprite = Resources.Load<Sprite>("UIComponents/" + name);
        this.stackable = stackable;
    }

    public Item()
    {
        this.Id = -1;
    }
}