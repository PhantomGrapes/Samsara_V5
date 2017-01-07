using UnityEngine;
using System.Collections;

public class ArrowManager : MonoBehaviour {

    private MainCharacter player;
    public bool isOut = false;
    public Vector2 startPosition;
    public float currentAngle;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<MainCharacter>();
        GetComponent<BoxCollider2D>().isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector2.Distance(startPosition, transform.position) > player.arrowRange)
            Destroy(gameObject);
        currentAngle = Mathf.Atan2(GetComponent<Rigidbody2D>().velocity.y, GetComponent<Rigidbody2D>().velocity.x)*Mathf.Rad2Deg;
        if (GetComponent<Rigidbody2D>().velocity.x < 0)
            currentAngle += 180;
        transform.rotation = Quaternion.Euler(0, 0,currentAngle);
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
