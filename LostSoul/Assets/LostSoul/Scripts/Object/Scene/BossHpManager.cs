using UnityEngine;
using System.Collections;

public class BossHpManager : MonoBehaviour {

    private BossRedLotus redLotus;
    private BossGreenLotus lei;
    public string bossName;
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
    }

    // Update is called once per frame
    void Update()
    {
        float s=0;
        switch (bossName)
        {
            case "RedLotus":
                s = redLotus.hp / redLotus.maxHp;
                break;
            case "GreenLotus":
                s = lei.hp / lei.maxHp;
                break;
        }
        if (s < 0)
            s = 0;
        GetComponent<RectTransform>().localScale = new Vector2(s, 1);
        print(s);
    }
}
