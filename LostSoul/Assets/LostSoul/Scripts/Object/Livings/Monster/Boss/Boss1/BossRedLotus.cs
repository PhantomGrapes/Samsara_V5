using UnityEngine;
using System.Collections;

public class BossRedLotus : Monster
{


	public bool activated;
	Vector2 selfPosition;
	Vector2 targetPosition;
	float sADP = 0.8f;
	//startAttackingDistanceProportion

	public bool normalSkillAvailable = true;
	public bool ultimateSkillAvailable = true;
	float ultimateCooldown = 15f;
	float normalSkillCooldown = 10f;

	public FirePool firePool;
	//	float firePoolDuration = 5f;

	public FireBall fireBall;

	float defaultSpeed = 5f;
	float hasteSpeed = 10f;
	float hasteDuration = 2f;

	bool phase1 = true;
	bool phase2 = false;
	bool phase3 = false;




	// Use this for initialization
	void Start ()
	{
		this.target = FindObjectOfType<MainCharacter> ();
		this.alive = true;
		// physical resistance formula
		this.physicalResistance = 1 - Mathf.Exp (-armor / 3);

		this.fireBall = FindObjectOfType<FireBall> ();
		this.fireBall.gameObject.SetActive (false);

		this.firePool = FindObjectOfType<FirePool> ();
		this.firePool.gameObject.SetActive (false);
		// attack interval formula
		this.attackInterval = 2 / this.attackSpeed;

		anim = GetComponent<Animator> ();
		GetComponent<Rigidbody2D> ().mass = 100f;

		movementSpeed = 2f;
		jumpForce = 10f;

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
		CooldownChecker ();
		DecideState ();

		if (this.fireBall.destroyed) {
			StartCoroutine (DestroyFireBall ());
		}

	}

	void DecideState ()
	{
		if (hp <= 0) {
			Die ();
		} else if (this.hp > this.maxHp * 0.66f) {
			this.Phase1 ();
		} else if (this.hp <= this.maxHp * 0.66f && this.hp > this.maxHp * 0.33f) {
			this.Phase2 ();
		} else {
			this.Phase3 ();
		}

		if (ultimateSkillAvailable) {
			StartCoroutine (UltimateSkill ());
		} else {
			Aggressive ();
		}
	}

	void CooldownChecker ()
	{
		this.canAttack = !this.attacked;
	}

	void Phase1 ()
	{
		this.phase1 = true;
		this.phase2 = false;
		this.phase3 = false;
	}

	void Phase2 ()
	{
		this.phase1 = false;
		this.phase2 = true;
		this.phase3 = false;

	}

	void Phase3 ()
	{
		this.phase1 = false;
		this.phase2 = false;
		this.phase3 = true;
	}

	void Aggressive ()
	{

		Vector2 enemyDirection = this.targetPosition - selfPosition;
		float enemyDistance = enemyDirection.magnitude;

		if (normalSkillAvailable) {
			StartCoroutine (Haste ());
		}
		if (this.canAttack && enemyDistance < this.attackRange) {
			if ((enemyDirection.x > 0 && this.facingRight) || (enemyDirection.x < 0 && !this.facingRight)) {
				if (normalSkillAvailable) {
					print ("use normal skill");
					StartCoroutine (NormalSkill ());
				} else {
					if (canAttack) {
						StartCoroutine (this.Attack ());
					}
				}
			} else {
				if (enemyDirection.x > 0)
					this.MoveRight ();
				else
					this.MoveLeft ();
			}

		} else {
			if (enemyDirection.x > this.attackRange * sADP) {
				this.MoveRight ();
			} else if (enemyDirection.x < -this.attackRange * sADP) {
				this.MoveLeft ();
			} else {
				this.Idle ();
			}
		}
	}

	IEnumerator Attack ()
	{
		float animStartToDamage1 = 0.9f;
		float animStartToDamage2 = 0.9f;
		float animStartToDamage3 = 0.9f;
		float animDuration1 = 1.3f;
		float animDuration2 = 1.3f;
		float animDuration3 = 1.3f;
		bool firstAttack = false;
		bool secondAttack = false;
		bool thirdAttack = false;

		this.attacked = true;
		if (phase1) {
			anim.SetTrigger ("attack1");
			this.movementSpeed = 0;
			yield return new WaitForSeconds (animStartToDamage1);
			if (this.alive) {
				DefaultAttack (this.target);
			}
			this.movementSpeed = 1f;
			yield return new WaitForSeconds (animDuration1 - animStartToDamage1);
			this.movementSpeed = defaultSpeed;
		} else if (phase2) {
			// attack1
			anim.SetTrigger ("attack1");
			this.movementSpeed = 0;
			yield return new WaitForSeconds (animStartToDamage1);
			if (this.alive) {
				DefaultAttack (this.target);
			}
			this.movementSpeed = 1f;
			yield return new WaitForSeconds (animDuration1 - animStartToDamage1);

			// attack2
			anim.SetTrigger ("attack2");
			this.movementSpeed = 0;
			yield return new WaitForSeconds (animStartToDamage2);
			if (this.alive) {
				DefaultAttack (this.target);
			}
			this.movementSpeed = 1f;
			yield return new WaitForSeconds (animDuration2 - animStartToDamage2);

			this.movementSpeed = defaultSpeed;
			
		} else if (phase3) {
			// attack1
			anim.SetTrigger ("attack1");
			this.movementSpeed = 0;
			yield return new WaitForSeconds (animStartToDamage1);
			if (this.alive) {
				DefaultAttack (this.target);
			}
			this.movementSpeed = 1f;
			yield return new WaitForSeconds (animDuration1 - animStartToDamage1);

			// attack2
			anim.SetTrigger ("attack2");
			this.movementSpeed = 0;
			yield return new WaitForSeconds (animStartToDamage2);
			if (this.alive) {
				DefaultAttack (this.target);
			}
			this.movementSpeed = 1f;
			yield return new WaitForSeconds (animDuration2 - animStartToDamage2);


			// attack3
			anim.SetTrigger ("attack3");
			this.movementSpeed = 0;
			yield return new WaitForSeconds (animStartToDamage3);
			if (this.alive) {
				DefaultAttack (this.target);
			}
			this.movementSpeed = 1f;
			yield return new WaitForSeconds (animDuration3 - animStartToDamage3);

			this.movementSpeed = defaultSpeed;

		}


		yield return new WaitForSeconds (this.attackInterval);
		this.attacked = false;

	}

	IEnumerator Haste ()
	{
		this.movementSpeed = hasteSpeed;
		yield return new WaitForSeconds (hasteDuration);
		this.movementSpeed = defaultSpeed;
		normalSkillAvailable = false;
		yield return new WaitForSeconds (normalSkillCooldown - hasteDuration);
		normalSkillAvailable = true;
	}


	IEnumerator NormalSkill ()
	{
		normalSkillAvailable = false;
		// to be calibreated
		float animStartToDamage = 0.20f;
		float animDuration = 0.55f;

		this.attacked = true;
		this.movementSpeed = 1f;
		this.anim.SetTrigger ("normalSkill");
		yield return new WaitForSeconds (animStartToDamage);
		if (this.alive) {
			this.firePool.transform.position = this.targetPosition;
			this.firePool.gameObject.SetActive (true);
			this.firePool.activated = true;
//			yield return new WaitForSeconds (firePoolDuration);
//			this.firePool.gameObject.SetActive (false);

		}
		yield return new WaitForSeconds (animDuration - animStartToDamage);
		this.movementSpeed = defaultSpeed;
		yield return new WaitForSeconds (this.attackInterval - animDuration);
		this.attacked = false;

		yield return new WaitForSeconds (normalSkillCooldown - this.firePool.duration);
		print ("normal skill now available");
		normalSkillAvailable = true;
	}



	IEnumerator UltimateSkill ()
	{
		ultimateSkillAvailable = false;

		// to be calibreated
		float animStartToDamage = 1.3f;
		float animDuration = 3.3f;

		this.attacked = true;
		anim.SetTrigger ("ultimateSkill");
		this.movementSpeed = 0;
		yield return new WaitForSeconds (animStartToDamage);
		if (this.alive) {
			this.fireBall.gameObject.transform.position = this.targetPosition + new Vector2 (0, 10f);
			this.fireBall.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0f, 0f);
			this.fireBall.gameObject.SetActive (true);
		}
		yield return new WaitForSeconds (animDuration - animStartToDamage);
		this.movementSpeed = defaultSpeed;
		yield return new WaitForSeconds (this.attackInterval - animDuration);
		this.attacked = false;
		yield return new WaitForSeconds (ultimateCooldown);
		ultimateSkillAvailable = true;
	}

	IEnumerator DestroyFireBall ()
	{
		float animDuration = 0.3f;
		this.fireBall.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0f, -10f);
		this.fireBall.anim.SetTrigger ("destroy");
		yield return new WaitForSeconds (animDuration);
		this.fireBall.gameObject.SetActive (false);
		this.fireBall.destroyed = false;

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
