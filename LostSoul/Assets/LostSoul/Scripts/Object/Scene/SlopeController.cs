using UnityEngine;
using System.Collections;

public class SlopeController : MonoBehaviour {
    public Collider2D bone, player;
    public ChoiceDoorController door;
    public FrontController front;
    public InverseController inverse;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<MainCharacter>().GetComponent<Collider2D>(); ;
        //Physics2D.IgnoreCollision(player, bone, true);
    }
	
    void FixedUpdate()
    {
        
        print(Physics2D.GetIgnoreCollision(player.GetComponent<PolygonCollider2D>(), bone.GetComponent<EdgeCollider2D>()));
    }
	// Update is called once per frame
	void Update () {
        if (!(door.playerStay || front.playerStay || inverse.playerStay))
        {
            //print("back to normal");
            Physics2D.IgnoreCollision(player.GetComponent<PolygonCollider2D>(), bone.GetComponent<EdgeCollider2D>(), false);
        }
        
    }
}
