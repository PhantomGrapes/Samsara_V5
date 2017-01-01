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
        if (col.GetComponent<Minion>() != null)
        {
            col.GetComponent<Minion>().beAttacked(player.attack);
            col.GetComponent<Minion>().beingAttacked = true;
        }
    }
}
