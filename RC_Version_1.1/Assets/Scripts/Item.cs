using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

    public string itemName;
    public string itemDesc;
    public string itemSkill;

    public int itemID;
    public int itemHP;
    public int itemAttack;
    public int itemSpeed;
    public ItemType itemType;

    public enum ItemType
    {
        Corp,
        Move,
        Weapon,
        Support,
        Com,
        Other1,
        Other2
    }
}
