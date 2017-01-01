using UnityEngine;
using System.Collections;

public class FirePool : MonoBehaviour {
	MainCharacter player;
	public Animator anim;
	public bool destroyed = false;
	bool burning = false;
	public bool activated = false;
	public float duration = 5f;

	void Start(){
		player = FindObjectOfType<MainCharacter> ();
		this.anim = FindObjectOfType<Animator> ();
	}

	void FixedUpdate(){
		if(burning){
			player.hp -= 0.1f;
		}

		if (activated) {
			activated = false;
			Deactivate ();
		}


	}

	void OnTriggerStay2D(Collider2D col){
		if (col.GetComponent<MainCharacter> () != null) {
			burning = true;
		}
		destroyed = true;
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.GetComponent<MainCharacter> () != null) {
			burning = false;
		}
	}

	IEnumerator Deactivate(){

		yield return new WaitForSeconds (this.duration);
		this.gameObject.SetActive (false);
		//			this.firePool.gameObject.SetActive (false);
	}



}
