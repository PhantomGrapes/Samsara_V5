using UnityEngine;
using System.Collections;

public class ChoiceDoorController : MonoBehaviour {
    public bool playerStay;
    private SlopeController slop;
	// Use this for initialization
	void Start () {
        playerStay = false;
        slop = transform.parent.GetComponent<SlopeController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (playerStay)
        {
            //print(Physics2D.GetIgnoreCollision(slop.player.GetComponent<PolygonCollider2D>(), slop.bone.GetComponent<EdgeCollider2D>()));
            if (Input.GetKey(KeyCode.S)) {
                print("ignore");
                Physics2D.IgnoreCollision(slop.player.GetComponent<PolygonCollider2D>(), slop.bone.GetComponent<EdgeCollider2D>(), true);
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<MainCharacter>() != null)
        {
            playerStay = true;
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<MainCharacter>() != null)
        {
            playerStay = false;
        }
    }
}
