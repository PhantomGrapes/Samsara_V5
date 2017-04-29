using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossHpUIManager : MonoBehaviour {

    private BossRedLotus redLotus;
    private BossGreenLotus lei;
    public string bossName;
    private List<Transform> children;
    // Use this for initialization
    void Start()
    {
        switch (bossName)
        {
            case "RedLotus":
                redLotus = FindObjectOfType<BossRedLotus>();
                break;
            case "GreenLotus":
                lei = FindObjectOfType<BossGreenLotus>();
                break;
        }
        children = new List<Transform>();
        children.Add(transform.GetChild(0));
        children.Add(transform.GetChild(1));
        children.Add(transform.GetChild(2));
    }

    void UpdateChildren(bool isActive)
    {
        foreach (Transform c in children)
        {
            c.gameObject.SetActive(isActive);
        }
    }
    // Update is called once per frame
    void Update()
    {
        switch (bossName)
        {
            case "RedLotus":
                if (redLotus.inBattle)
                {
  
                    UpdateChildren(true);
                }
                else
                    UpdateChildren(false);
                break;
            case "GreenLotus":
                if (lei.inBattle)
                    UpdateChildren(true);
                else
                    UpdateChildren(false);
                break;
        }
    }
}
