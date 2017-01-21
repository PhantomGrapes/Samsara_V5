using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossGreenLotus : Monster
{
	public Projectile stone;
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
	void Start ()
	{
		target = FindObjectOfType<MainCharacter> ();
		anim = GetComponent<Animator> ();
		battleZone = FindObjectOfType<BattleZone> ();
		this.alive = true;
		// physical resistance formula
		this.physicalResistance = 1 - Mathf.Exp (-armor / 3);


		// attack interval formula
		this.attackInterval = 2 / this.attackSpeed;

		this.anim = GetComponent<Animator> ();
		GetComponent<Rigidbody2D> ().mass = 100f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (this != null) {
			BossFlipping ();
		}
	}

	void FixedUpdate ()
	{
		anim.SetFloat ("xSpeed", Mathf.Abs (GetComponent<Rigidbody2D> ().velocity.x));
		selfPosition = GetComponent<Rigidbody2D> ().position;
		targetPosition = target.GetComponent<Rigidbody2D> ().position;

		DecideState ();

	}

	void DecideState ()
	{
		if (canToss) {
			anim.SetTrigger ("toss");
			// to be put in anim
//			Toss ();
			StartCoroutine (TossCD ());
		}

		if (canBlink && Mathf.Abs ((selfPosition.x - targetPosition.x)) <= startBlinkDistance) {
			anim.SetTrigger ("disappear");
//			Blink ();
			StartCoroutine (BlinkCD ());

		}

		if (targetPosition.x > selfPosition.x) {
			this.facingRight = true;
		} else {
			this.facingRight = false;
		}


		if (hp <= 0) {
			this.alive = false;
			anim.SetTrigger ("die");
		}

	}

	public void Toss ()
	{

		Projectile newStone = (Projectile)Instantiate (stone);
//		newStone.transform.position = this.transform.position;
		newStone.transform.position = new Vector2(selfPosition.x , selfPosition.y+3f);
		newStone.gameObject.SetActive (true);
	}

	IEnumerator  TossCD ()
	{
		canToss = false;
		yield return new WaitForSeconds (tossCD);
		canToss = true;
	}


	// Blink to random position in the room
	public void Blink ()
	{
		float new_x = 0f;
		float new_y = 0f;
		float targetX_max = Mathf.Min ((targetPosition.x + blinkMinDistanceToPlayer), battleZone.x_max);
		float targetX_min = Mathf.Max ((targetPosition.x - blinkMinDistanceToPlayer), battleZone.x_min);
		if (selfPosition.x <= battleZone.x_centre) {
			new_x = Random.Range (targetX_max, battleZone.x_max);
		} else {
			new_x = Random.Range (targetX_min, battleZone.x_min);
		}
		new_y = Random.Range (battleZone.y_max, battleZone.y_min);
		print ("before: "+selfPosition);
		print ("supposed to be(x): "+new_x);
		print ("supposed to be(y): "+new_y);
		this.transform.position = new Vector2 (new_x, new_y);
		print ("turns out to be: "+selfPosition);
		anim.SetTrigger ("appear");

	}

	IEnumerator BlinkCD ()
	{
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
