using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveRangeController : MonoBehaviour {

    private int counter = 0;

    public List<Minion> enemyList = new List<Minion>();
	public List<MainCharacter> playerList = new List<MainCharacter> ();
    private MainCharacter player;



    // Use this for initialization

    void Start()
    {
        player = FindObjectOfType<MainCharacter>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Minion>() != null && !enemyList.Contains(other.GetComponent<Minion>()))
        {
            enemyList.Add(other.GetComponent<Minion>());
            other.GetComponent<Minion>().BeAttacked(player.attack);
        }
    }
}
