using UnityEngine;
using System.Collections;

public class ForbiddenStateController : MonoBehaviour {
    // action list
    public int walk = 0;
    public int attack = 0;
    public int jump = 0;
    public int skillAttack = 0;
    public int beAttacked = 0;
    public int roll = 0;

    void Start()
    {
        walk = 0;
        attack = 0;
        jump = 0;
        skillAttack = 0;
        beAttacked = 0;
    }
    
}
