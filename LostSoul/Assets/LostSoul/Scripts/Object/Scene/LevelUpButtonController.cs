using UnityEngine;
using System.Collections;

public class LevelUpButtonController : MonoBehaviour {
    InformationController infoBoard;
	// Use this for initialization
	void Start () {
        infoBoard = FindObjectOfType<InformationController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (infoBoard.itemSelected < 0 || infoBoard.itemSelected > 5)
            this.gameObject.SetActive(false);
	}
}
