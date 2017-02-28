using UnityEngine;
using System.Collections;

public class Observer:MonoBehaviour {
    public virtual void inventoryUpdate(ItemData o, string message) { }
    public virtual void inventoryUpdate(int o, string message) { }
}
