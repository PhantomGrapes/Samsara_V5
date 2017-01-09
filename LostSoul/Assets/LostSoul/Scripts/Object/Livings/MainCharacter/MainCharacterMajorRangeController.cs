using UnityEngine;
using System.Collections;

public class MainCharacterMajorRangeController : MonoBehaviour {

    MainCharacter player;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<MainCharacter>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = player.transform.position;
        transform.localScale = player.transform.localScale;
	}
}
