using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Observable : MonoBehaviour {
    public Observer[] observers;

    public void initializeBroadcast()
    {
        observers = FindObjectsOfType<Observer>();
    }

    protected void notifyChanges(ItemData o, string message)
    {
        if(message == "amount")
            print(o.id +"   "+ observers.Length);
        for (int i = 0; i < observers.Length; i++)
        {
            observers[i].inventoryUpdate(o, message);
        }
    }

    protected void notifyChanges(int o, string message)
    {
        for (int i = 0; i < observers.Length; i++)
        {
            observers[i].inventoryUpdate(o, message);
        }
    }
}
