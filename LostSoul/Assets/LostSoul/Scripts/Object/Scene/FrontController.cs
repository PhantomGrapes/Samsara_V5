using UnityEngine;
using System.Collections;

public class FrontController : MonoBehaviour {
    public bool playerStay;
    private SlopeController slop;
    // Use this for initialization
    void Start()
    {
        playerStay = false;
        slop = transform.parent.GetComponent<SlopeController>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<MainCharacter>() != null)
        {
            playerStay = true;
            
        }
    }

    void Update()
    {
        if (playerStay && Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space))
        {
            Physics2D.IgnoreCollision(slop.player.GetComponent<PolygonCollider2D>(), slop.bone.GetComponent<EdgeCollider2D>(), true);
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
