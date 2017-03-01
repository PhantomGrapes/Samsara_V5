using UnityEngine;
using System.Collections;

public class ItemPickUpRangeController : MonoBehaviour {

    MainCharacter player;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<MainCharacter>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.GetComponent<ItemToBePickedUp>() != null)
        {
            player.itemToBePickedUp = col.GetComponent<ItemToBePickedUp>();
        }
    }

    // set the item to be unavailable for pickup when they no longer collide
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<ItemToBePickedUp>() != null)
        {
            player.itemToBePickedUp = null;
        }
    }
}
