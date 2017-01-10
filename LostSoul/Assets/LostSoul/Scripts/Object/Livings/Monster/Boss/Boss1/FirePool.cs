using UnityEngine;
using System.Collections;

public class FirePool : MonoBehaviour {
	MainCharacter player;
	public Animator anim;
	public bool destroyed = false;
	bool burning = false;
	public bool activated = false;
	public float duration;
	public float burnInterval;
	public float firePoolDamage;

//	// used to fixed the position of firePool
//	bool positionFixed = false;
//	Vector2 position;

	void Start(){
		player = FindObjectOfType<MainCharacter> ();
		this.anim = FindObjectOfType<Animator> ();
	}

	void FixedUpdate(){
		if(burning){
			StartCoroutine(Burn ());
		}

//		print (this.transform.position);
		print (this.transform.localPosition);
//		if (activated) {
//			FixPosition ();
//		} else {
//			positionFixed = false;
//		}


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

	public IEnumerator Deactivate(){

		yield return new WaitForSeconds (this.duration);
		this.gameObject.SetActive (false);
		this.activated = false;
	}

	IEnumerator Burn(){
		yield return new WaitForSeconds (this.burnInterval);
			player.BeAttacked(firePoolDamage);

	}

//	void FixPosition(){
//
//		if (!positionFixed) {
//			position = this.transform.position;
//			positionFixed = true;
//		}
//		this.transform.position = position;
//	}



}
