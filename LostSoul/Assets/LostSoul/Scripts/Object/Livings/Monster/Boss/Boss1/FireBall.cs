using UnityEngine;
using System.Collections;

public class FireBall : MonoBehaviour {
	MainCharacter player;
	public Animator anim;
	public bool destroyed = false;

	void Start(){
		player = FindObjectOfType<MainCharacter> ();
		this.anim = FindObjectOfType<Animator> ();
	}
	void OnTriggerEnter2D(Collider2D col){
		if (col.GetComponent<MainCharacter> () != null) {
			player.hp -= 30f;
			player.anim.SetTrigger ("BeAttacked");	
		}

		destroyed = true;
	}


}
