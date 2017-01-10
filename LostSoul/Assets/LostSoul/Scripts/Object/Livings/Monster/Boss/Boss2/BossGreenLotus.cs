using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossGreenLotus : Monster {
	List<Projectile> projectiles;
	MainCharacter target;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Toss(){
		projectiles.Add(new Projectile ());
		Projectile rock = projectiles [-1];
		rock.transform.position = this.transform.position;
		rock.GetComponent<Rigidbody2D> ().velocity = rock.GetVelocity ();
		StartCoroutine (rock.DestroyProjectile ());
	}
}
