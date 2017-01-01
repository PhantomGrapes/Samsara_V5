using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	public int index;
	public float level;

	void Update(){

		if (this.transform.position.y <= level) {
			this.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 5f);
		}
		if (this.transform.position.y >= (level+10)) {
			this.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
		}
	}
}
