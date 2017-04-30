using UnityEngine;
using System.Collections;

public class SlopeController : MonoBehaviour {
    public Collider2D player;
    public Collider2D bone;
    ChoiceDoorController door;
    FrontController front;
    InverseController inverse;
    public bool see;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<MainCharacter>().GetComponent<Collider2D>();
        bone = transform.GetChild(0).GetComponent<Collider2D>();
        door = transform.GetComponentInChildren<ChoiceDoorController>();
        front = transform.GetComponentInChildren<FrontController>();
        inverse = transform.GetComponentInChildren<InverseController>();
        //Physics2D.IgnoreCollision(player, bone, true);
    }
	
	// Update is called once per frame
	void Update () {
        if (!(door.playerStay || front.playerStay || inverse.playerStay))
        {
            //print("back to normal");
            Physics2D.IgnoreCollision(player.GetComponent<PolygonCollider2D>(), bone.GetComponent<EdgeCollider2D>(), false);
        }
        see = Physics2D.GetIgnoreCollision(player.GetComponent<PolygonCollider2D>(), bone.GetComponent<EdgeCollider2D>());
    }
}
