using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TeleportButtonController : MonoBehaviour {

    public TeleportController teleport;
    private Button teleportButton;
	// Use this for initialization
	void Start () {
        teleportButton = GetComponent<Button>();

	}
	
	// Update is called once per frame
	void Update () {
        teleportButton.interactable = teleport.active;

	}
}
