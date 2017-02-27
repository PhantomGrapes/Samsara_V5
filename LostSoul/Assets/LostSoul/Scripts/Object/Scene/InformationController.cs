using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InformationController : MonoBehaviour {
    public Text info;
    public Inventory inventory;
    public int itemSelected;
	// Use this for initialization
	void Start () {
        itemSelected = -1;
        info = GetComponent<Text>();
        info.text = "";
        inventory = FindObjectOfType<Inventory>();
	}
	
}
