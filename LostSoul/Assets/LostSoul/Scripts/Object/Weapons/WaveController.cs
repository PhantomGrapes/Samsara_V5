using UnityEngine;
using System.Collections;

public class WaveController : MonoBehaviour {

    public float waveVelocity = 20f;
    private MainCharacter player;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<MainCharacter>();
        if (player.facingRight)
            GetComponent<Rigidbody2D>().velocity = new Vector2(waveVelocity, 0f);
        else
            GetComponent<Rigidbody2D>().velocity = new Vector2(-waveVelocity, 0f);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
