﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponRangeController : MonoBehaviour {

    private int counter = 0;

    public List<Monster> enemyList = new List<Monster>();
 
    // Use this for initialization

	void Start () {

	}

    void FixedUpdate()
    {
        foreach (Monster Monster in enemyList)
        {
            if (Monster == null)
                enemyList.Remove(Monster);
        }
    }
	
	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Monster>() != null && !enemyList.Contains(other.GetComponent<Monster>()))
        {
            enemyList.Add(other.GetComponent<Monster>());

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Monster>() != null && enemyList.Contains(other.GetComponent<Monster>()))
        {
            enemyList.Remove(other.GetComponent<Monster>());

        }
    }
}
