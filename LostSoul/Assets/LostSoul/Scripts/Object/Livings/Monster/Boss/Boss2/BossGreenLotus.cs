using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossGreenLotus : Monster {
	List<Projectile> projectiles;
	MainCharacter target;
	BattleZone battleZone;
	Animator anim;
	public float startBlinkDistance;
	public float blinkMinDistanceToPlayer;
	public float blinkDisappearDuration;

	bool canBlink = true;
	public float blinkCD;

	bool canToss = true;
	public float tossCD;

	Vector2 selfPosition;
	Vector2 targetPosition;



	// Use this for initialization
	void Start () {
		target = FindObjectOfType<MainCharacter> ();
		anim = GetComponent<Animator> ();
		battleZone = GetComponent<BattleZone> ();

		this.target = FindObjectOfType<MainCharacter> ();
		this.alive = true;
		// physical resistance formula
		this.physicalResistance = 1 - Mathf.Exp (-armor / 3);


		// attack interval formula
		this.attackInterval = 2 / this.attackSpeed;

		this.anim = GetComponent<Animator> ();
		GetComponent<Rigidbody2D> ().mass = 100f;
	}
	
	// Update is called once per frame
	void Update () {
		if (this != null) {
			BossFlipping ();
		}
	}

	void FixedUpdate(){
		anim.SetFloat ("xSpeed", Mathf.Abs (GetComponent<Rigidbody2D> ().velocity.x));
		selfPosition = GetComponent<Rigidbody2D> ().position;
		targetPosition = target.GetComponent<Rigidbody2D> ().position;
	}

	void DecideState(){
		if (canToss) {
			Toss ();
		}
		if (canBlink && Mathf.Abs ((selfPosition.x - targetPosition.x)) <= startBlinkDistance) {
			Blink ();
		}
	}

	public void Toss(){
		projectiles.Add(new Projectile ());
		Projectile rock = projectiles [-1];
		rock.transform.position = this.transform.position;
		rock.GetComponent<Rigidbody2D> ().velocity = rock.GetVelocity ();
		StartCoroutine (rock.DestroyProjectile ());
	}

	IEnumerator  TossCD(){
		canToss = false;
		yield return new WaitForSeconds (tossCD);
		canToss = true;
	}


	// Blink to random position in the room
	IEnumerator Blink(){
		anim.SetTrigger ("disappear");
		float new_x = 0f;
		float new_y = 0f;
		float targetX_max = Mathf.Min ((target.transform.position.x + blinkMinDistanceToPlayer), battleZone.x_max);
		float targetX_min = Mathf.Max ((target.transform.position.x - blinkMinDistanceToPlayer), battleZone.x_min);
		if (selfPosition.x <= battleZone.x_centre) {
			new_x = Random.Range (targetX_max, battleZone.x_max);
		} else {
			new_x = Random.Range (targetX_min, battleZone.x_min);
		}
		new_y = Random.Range (battleZone.y_max, battleZone.y_min);
		selfPosition = new Vector2 (new_x, new_y);
		yield return new WaitForSeconds (blinkDisappearDuration);
		anim.SetTrigger ("appear");

	}

	IEnumerator BlinkCD(){
		canBlink = false;
		yield return new WaitForSeconds (blinkCD);
		canBlink = true;
	}

	public void BossFlipping ()
	{
		Vector3 localScale = GetComponent<Transform> ().localScale;
		if (!facingRight)
			localScale.x = Mathf.Abs (localScale.x);
		else
			localScale.x = -1f * Mathf.Abs (localScale.x);
		GetComponent<Transform> ().localScale = localScale;
	}
}
