using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemData : Observable {

    public int id;
    int amount;
    bool isMainWeapon;
    bool isSecondWeapon;
    float attack;
    Inventory inventory;
    

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(TaskOnClick);
        inventory = FindObjectOfType<Inventory>();
        initializeBroadcast();
    }

    
    void TaskOnClick()
    {
        inventory.setItemSelected(id);
    }

    public int getAmount()
    {
        return amount;
    }

    public void setAmount(int newAmount)
    {
        amount = newAmount;
        notifyChanges(this, "amount");
        transform.GetChild(0).GetComponent<Text>().text = amount.ToString();
    }

    public bool getIsMainWeapon()
    {
        return isMainWeapon;
    }

    public void setIsMainWeapon(bool newIsMainWeapon)
    {
        isMainWeapon = newIsMainWeapon;
        notifyChanges(this, "isMainWeapon");
    }

    public bool getIsSecondWeapon()
    {
        return isSecondWeapon;
    }

    public void setIsSecondWeapon(bool newIsSecondWeapon)
    {
        isSecondWeapon = newIsSecondWeapon;
        notifyChanges(this, "isSecondWeapon");
    }

    public float getAttack()
    {
        return attack;
    }

    public void setAttack(float newAttack)
    {
        attack = newAttack;
        notifyChanges(this, "attack");
    }
}
