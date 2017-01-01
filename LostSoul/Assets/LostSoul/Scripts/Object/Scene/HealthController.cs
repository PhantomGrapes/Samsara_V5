using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

    private MainCharacter player;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<MainCharacter>();
	}
	
	// Update is called once per frame
	void Update () {
        float s = player.hp / player.maxHp;
        if (s < 0)
            s = 0;
        GetComponent<RectTransform>().localScale = new Vector2(s, 1);
	}
}
