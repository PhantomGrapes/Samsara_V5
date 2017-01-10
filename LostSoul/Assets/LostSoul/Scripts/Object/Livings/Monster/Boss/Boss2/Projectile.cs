using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	public float speed;
	public Vector2 direction;
	MainCharacter target;
	Rigidbody2D rigi;
	bool initialVelocitySet = false;
	public float duration;
	// Use this for initialization
	void Start () {
		target = FindObjectOfType<MainCharacter> ();
		rigi = GetComponent<Rigidbody2D> ();
	}
	

	void FixedUpdate () {
		if (!initialVelocitySet) {
			rigi.velocity = GetVelocity ();
		}
		print (rigi.velocity);
	
	}

	public Vector2 GetVelocity(){
		initialVelocitySet = true;
		return (target.transform.position - this.transform.position).normalized * speed;
	}

	public IEnumerator DestroyProjectile(){
		yield return new WaitForSeconds (duration);
		this.gameObject.SetActive (false);
	}
}
