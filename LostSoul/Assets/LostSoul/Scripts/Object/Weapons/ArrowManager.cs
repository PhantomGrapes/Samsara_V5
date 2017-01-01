using UnityEngine;
using System.Collections;

public class ArrowManager : MonoBehaviour {

    private MainCharacter player;
    public bool isOut = false;
    public Vector2 startPosition;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<MainCharacter>();
        GetComponent<Rigidbody2D>().gravityScale = 0f;
        GetComponent<BoxCollider2D>().isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector2.Distance(startPosition, transform.position) > player.arrowRange)
            Destroy(gameObject);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Minion>() != null)
        {
            other.GetComponent<Minion>().beAttacked(player.attack);
            other.GetComponent<Minion>().beingAttacked = true;
            Destroy(gameObject);
        }
    }
}
