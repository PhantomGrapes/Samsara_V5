using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {
    MapMenuController map;
    StateCanvasController state;
    InventoryCanvasController inventory;

	// Use this for initialization
	void Start () {
        map = FindObjectOfType<MapMenuController>();
        state = FindObjectOfType<StateCanvasController>();
        inventory = FindObjectOfType<InventoryCanvasController>();
        map.gameObject.SetActive(false);
        state.gameObject.SetActive(true);
        inventory.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
