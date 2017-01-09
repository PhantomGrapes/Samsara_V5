using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
    public GameObject myPart;
    public string myPartName;
    public string myPartDesc;
    public string myPartSkill;
    public int myPartID;
    public int myPartHP;
    public int myPartAttack;
    public int myPartSpeed;
    public PartType myPartType;

    public enum PartType
    {
        Corp,
        Move,
        Weapon,
        Support,
        Com,
        Other1,
        Other2
    }

    // Use this for initialization
    void Start () {
        myPartName = myPart.GetComponent<Item>().itemName;
        myPartDesc = myPart.GetComponent<Item>().itemDesc;
        myPartSkill = myPart.GetComponent<Item>().itemSkill;
        myPartID = myPart.GetComponent<Item>().itemID;
        myPartHP = myPart.GetComponent<Item>().itemHP;
        myPartAttack = myPart.GetComponent<Item>().itemAttack;
        myPartSpeed = myPart.GetComponent<Item>().itemSpeed;
        myPartType = (PartType)myPart.GetComponent<Item>().itemType;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
