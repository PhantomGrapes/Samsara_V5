using UnityEngine;
using System.Collections;

public class WASkillController : MonoBehaviour {

    private MainCharacter player;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<MainCharacter>();
	}

    void Update()
    {

    }

    // give damage to minion
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Monster>() != null)
        {
            //print("give damage");
            col.GetComponent<Monster>().beAttacked(player.attack);
            col.GetComponent<Monster>().beingAttacked = true;
        }
    }
}
