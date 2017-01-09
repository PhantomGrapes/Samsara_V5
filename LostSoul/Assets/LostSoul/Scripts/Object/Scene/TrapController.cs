using UnityEngine;
using System.Collections;

public class TrapController : MonoBehaviour {
    private MainCharacter player;

    void Start()
    {
        player = FindObjectOfType<MainCharacter>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<MainCharacter>() != null)
        {
            player.BeAttacked(player.maxHp + 1);
        }
    }
}
