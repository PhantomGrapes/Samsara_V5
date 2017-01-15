using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	public float speed;
	public float damage;
	Vector2 direction;
	Vector2 reflectedDirection;
	MainCharacter target;
	Rigidbody2D rigi;
	bool reflectionDecided = false;
	bool initialVelocitySet = false;
	public float duration;
	// Use this for initialization
	void Start ()
	{
		target = FindObjectOfType<MainCharacter> ();
		rigi = GetComponent<Rigidbody2D> ();
	}


	void FixedUpdate ()
	{
		if (!initialVelocitySet) {
			rigi.velocity = GetVelocity ();
		}
		direction = rigi.velocity.normalized;
		if (!reflectionDecided) {
			// cast a ray from the current point in the direction of the movement
			Vector2 origin = new Vector2 (this.transform.position.x + this.direction.x, this.transform.position.y + this.direction.y);
			RaycastHit2D hit = Physics2D.Raycast (origin, this.direction);

			if (hit.collider != null) {
				print (hit.collider.tag);
				// if the collider met is a reflector
				if (hit.collider.gameObject.tag == "Reflectors") {
					print ("1");
					// define the normal vector
					Vector2 normal = hit.normal;
					// defiine the reflected direction
					reflectedDirection = Vector2.Reflect (this.direction, normal);
					reflectionDecided = true;
				}
			}
		}

	
	}

	public Vector2 GetVelocity ()
	{
		initialVelocitySet = true;
		return (target.transform.position - this.transform.position).normalized * speed;
	}

	public IEnumerator DestroyProjectile ()
	{
		yield return new WaitForSeconds (duration);
		this.gameObject.SetActive (false);
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		if (col.gameObject.tag == "Reflectors") {
			this.rigi.velocity = this.reflectedDirection * this.speed;
			this.reflectionDecided = false;
		}
		if (col.collider.GetComponent<MainCharacter> () != null) {
			target.BeAttacked (damage);
			target.beingAttacked = true;
			this.gameObject.SetActive (false);
		}


	}
}
