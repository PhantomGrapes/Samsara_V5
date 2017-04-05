using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Timers;

public class Minion : Monster
{

	public bool activated;
	float roamRange = 10;
	Vector2 selfPosition;
	Vector2 targetPosition;
	private float retreatPercent = 0.3f;
	private bool retreating = false;

	float originalSpeed;

	//startAttackingDistanceProportion

	public bool isRoaming = false;

	Vector2 roamTo;

	public GameObject bloodParticle;

	EnemyMeleeWeaponCollider minionWeaponCollider;



	// Use this for initialization
	public void Start ()
	{
		MainCharacter[] targets = FindObjectsOfType<MainCharacter> ();
		if (targets.Length == 1) {
			target = targets [0];
		} else {
			foreach (MainCharacter p in targets) {
				if (p.CompareTag ("Player")) {
					target = p;
					break;
				}
			}
		}

		this.alive = true;
		// physical resistance formula
		this.physicalResistance = 1 - Mathf.Exp (-armor / 3);

		// attack interval formula
		this.attackInterval = 2 / this.attackSpeed;

		anim = GetComponent<Animator> ();
		GetComponent<Rigidbody2D> ().mass = 100f;    // make the minion very heavy so that it cannot be pushed back

		originalSpeed = this.movementSpeed;
		jumpForce = 10f;

		minionWeaponCollider = FindObjectOfType<EnemyMeleeWeaponCollider> ();
	}

	// Update is called once per frame
	void Update ()
	{


		if (this != null) {

			MinionFlipping ();
		}
		anim.SetFloat ("xSpeed", Mathf.Abs (GetComponent<Rigidbody2D> ().velocity.x));
		selfPosition = GetComponent<Rigidbody2D> ().position;
		targetPosition = target.GetComponent<Rigidbody2D> ().position;
		CooldownChecker ();
		DecideState ();
	}

	void FixedUpdate ()
	{


		if (this.timeLock) {
			this.movementSpeed = 0f;
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (0f, 0f);

		} else {
			this.movementSpeed = originalSpeed;
		}

		if (this.moveLock) {
			this.movementSpeed = 0f;
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (0f, 0f);
		} else {
			this.movementSpeed = originalSpeed;
		}

	}


	void DecideState ()
	{
		if (alive) {
			// check if alive first
			if (hp <= 0) {
				Die ();
				this.anim.SetBool ("alive", false);
			}
//			else if (hp < maxHp * retreatPercent) {
//				isRoaming = false;              // if low heath, retreat
//				Retreat ();
//			}
			else if (Vector2.Distance (selfPosition, targetPosition) > alertDistance) {
				Roam ();     // if far, roam
			} else {
				isRoaming = false;  // else be aggressive
				Aggressive ();
			}
		} else {
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (0f, 0f);
			StartCoroutine (RemoveMinion ());

		}
	}

	// States of the minion: roaming, aggresive, retreat
	void Roam ()
	{
		bool reached = true;

		// if reached, set new target position
		isRoaming = true;
		if (reached) {
			float newX = Random.Range (-roamRange, roamRange);
			if (Mathf.Abs (newX) < roamRange * 0.5)
				newX = 0;
			roamTo = new Vector2 (selfPosition.x + newX, selfPosition.y);
			reached = false;

		}
		// move towards target position unless reached
		if (roamTo.x + 0.1 < selfPosition.x
		    || roamTo.x - 0.1 > selfPosition.x) {
			if (roamTo.x > selfPosition.x)
				MoveRight ();
			else
				MoveLeft ();
		} else {
			reached = true;
			this.Idle ();
		}
	}


	void Retreat ()
	{
		this.retreating = true;
		Vector2 dir = selfPosition - targetPosition;
		if (dir.x > 0)
			MoveRight ();
		else
			MoveLeft ();

	}

	void Aggressive ()
	{

		Vector2 enemyDirection = this.targetPosition - selfPosition;

		float enemyDistance = enemyDirection.magnitude;
		if (this.canAttack && (enemyDistance < this.attackRange)) {
			if ((enemyDirection.x > 0 && this.facingRight) || (enemyDirection.x < 0 && !this.facingRight)) {
				Attack ();
				MoveLock ();
			} 

		} else{
			if (enemyDirection.x > 0) {
				this.MoveRight ();
			} else {
				this.MoveLeft ();
			}
		}
	}

	void Attack ()
	{
		anim.SetTrigger ("attack");
//		this.beingAttacked = false;
	}

	public void DefaultAttack ()
	{

//		if (!this.beingAttacked && !this.retreating) {
		if (minionWeaponCollider.GetPlayer () != null) {
			minionWeaponCollider.GetPlayer ().BeAttacked (this.attack);
			minionWeaponCollider.GetPlayer ().beingAttacked = true;
		} 
//		}
	}


	void CooldownChecker ()
	{

//		this.canAttack = !this.attacked;

	}




	public void MinionFlipping ()
	{
		if (!flipLock) {
			Vector3 localScale = GetComponent<Transform> ().localScale;
			if (!facingRight)
				localScale.x = Mathf.Abs (localScale.x);
			else
				localScale.x = -1f * Mathf.Abs (localScale.x);
			GetComponent<Transform> ().localScale = localScale;
		}
	}


	// block for delayed removal of minions


	IEnumerator RemoveMinion ()
	{	
		var weaponRanges = FindObjectsOfType<WeaponRangeController> ();
		foreach (WeaponRangeController w in weaponRanges) {
			w.enemyList.Remove (this);
		}
		Destroy (GetComponent<Collider2D> ());
		yield return new WaitForSeconds (1f);
		Destroy (gameObject);
	}
}