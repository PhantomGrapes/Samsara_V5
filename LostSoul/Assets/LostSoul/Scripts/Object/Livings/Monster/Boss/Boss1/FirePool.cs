using UnityEngine;
using System.Collections;

public class FirePool : MonoBehaviour
{
	MainCharacter player;
	public Animator anim;
	//	public bool destroyed = false;
	bool burning = false;
	bool canBurn = true;
	public bool activated = false;
	public float duration;
	public float burnInterval;
	public float firePoolDamage;

	//	// used to fixed the position of firePool
	//	bool positionFixed = false;
	//	Vector2 position;

	void Start ()
	{
		player = FindObjectOfType<MainCharacter> ();
		this.anim = FindObjectOfType<Animator> ();
	}

	void Update ()
	{
		if (canBurn) {
			if (burning) {
				StartCoroutine (Burn ());
			}
		}
	}

	void OnTriggerStay2D (Collider2D col)
	{
		if (col.GetComponent<MainCharacter> () != null) {
			burning = true;
		}
	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (col.GetComponent<MainCharacter> () != null) {
			burning = false;
		}
	}

	public IEnumerator Deactivate ()
	{

		yield return new WaitForSeconds (this.duration);
		this.gameObject.SetActive (false);
		this.activated = false;
	}

	IEnumerator Burn ()
	{
		canBurn = false;
		player.BeAttacked (firePoolDamage);
		yield return new WaitForSeconds (this.burnInterval);
		canBurn = true;
	}

}
