using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponRangeController : MonoBehaviour {

    private int counter = 0;

    public List<Minion> enemyList = new List<Minion>();
 
    // Use this for initialization

	void Start () {

	}

    void FixedUpdate()
    {
        foreach (Minion minion in enemyList)
        {
            if (minion == null)
                enemyList.Remove(minion);
        }
    }
	
	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Minion>() != null && !enemyList.Contains(other.GetComponent<Minion>()))
        {
            enemyList.Add(other.GetComponent<Minion>());

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Minion>() != null && enemyList.Contains(other.GetComponent<Minion>()))
        {
            enemyList.Remove(other.GetComponent<Minion>());

        }
    }
}
