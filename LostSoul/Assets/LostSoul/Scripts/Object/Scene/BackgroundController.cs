using UnityEngine;
using System.Collections;

public class BackgroundController : MonoBehaviour {

    // attach object to Maincharacter
    private MainCharacter player;
    public bool followPlayer = true;
    public float xOffset = 0;
    public float yOffset;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<MainCharacter>();
    }

	// Update is called once per frame
	void Update ()
    {
	    if (followPlayer)
        {
            transform.position = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + yOffset, transform.position.z);
        }
	}
}
