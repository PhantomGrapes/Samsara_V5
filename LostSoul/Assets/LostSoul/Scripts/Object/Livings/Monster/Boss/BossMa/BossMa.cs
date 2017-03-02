using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class BossMa : MainCharacter
{

	Vector2 targetPosition;
	Vector2 selfPosition;
	public MainCharacter target;
	protected float attackRange;
	bool facingPlayer;

	void Start ()
	{	
		target = FindObjectOfType<MainCharacter> ();
		base.Start ();

	}


	void FixedUpdate ()
	{
		// to check whether the ground overlap the circle on player's foot
		grounded = Physics2D.OverlapCircle (frontGroundCheck.position, groundCheckRadius, whatIsGround) || Physics2D.OverlapCircle (backGroundCheck.position, groundCheckRadius, whatIsGround);
		if (!checkJump)
			NormalizeSlope ();



//		CheckStatus ();
		LocalCheckStatus ();

		DecideStrategy ();


	}

	void Update ()
	{
		Flipping ();
	}



	void DecideStrategy ()
	{
		// defensive
//		if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("Attack1HS")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("Attack2HS")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("Attack3HS")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("HSSkill")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("JumpAttackHS1")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("AttackBAA")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("SkillBAA")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("AttackBAAP02")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("AttackBAAP03")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("AttackBAAP01")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("AttackWA")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("SkillWA")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("AttackSS")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("AttackDagger")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("SkillDagger")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("SkillCB")) {
//
//		} else if (target.anim.GetCurrentAnimatorStateInfo (0).IsTag ("AttackCB")) {
//
//		}


		if (target.checkAttack) {

			Retreat ();
		} else {

			Offensive ();
		}
	}

	/// <summary>
	/// states
	/// </summary>
	void Offensive ()
	{
		Vector2 enemyDirection = targetPosition - selfPosition;
		float enemyDistance = enemyDirection.magnitude;
		if (enemyDistance > attackRange && facingPlayer) {
			Approach ();
		} else {
			NormalAttack ();
		}

	}

	void Defensive ()
	{


	}


	/// <summary>
	/// movements
	/// </summary>
	void Approach ()
	{
		if (targetPosition.x > selfPosition.x) {
			MoveRight ();
		} else {
			MoveLeft ();
		}

	}

	void Retreat ()
	{
		if (targetPosition.x < selfPosition.x) {
			MoveRight ();
		} else {
			MoveLeft ();
		}
	}

	protected void LocalCheckStatus ()
	{
		// list of cooldowns

		// distance to player
		selfPosition = GetComponent<Rigidbody2D> ().position;
		targetPosition = target.GetComponent<Rigidbody2D> ().position;


		// distance to border


		// attack range
		if (target.inventory.mainWeapon.current != -1) {
			switch (target.inventory.mainWeapon.current) {
			case 1:
				attackRange = defaultWeaponRange_1.range;
				break;
			case 3:
				attackRange = defaultWeaponRange_3.range;
				break;
			case 4:
				attackRange = defaultWeaponRange_4.range;
				break;
			case 5:
				attackRange = defaultWeaponRange_5.range;
				break;
			}
		}
		facingPlayer = (targetPosition.x < selfPosition.x && !facingRight) || (targetPosition.x > selfPosition.x && facingRight);
	}

	// Overriding functions in Maincharacter
	public override void giveDefaultDamageToEnemy ()
	{
		//print("give damage");
		List<MainCharacter> playerList = new List<MainCharacter> ();
		if (target.inventory.mainWeapon.current != -1) {
			switch (target.inventory.mainWeapon.current) {
			case 1:
				playerList = defaultWeaponRange_1.playerList;
				break;
			case 3:
				playerList = defaultWeaponRange_3.playerList;
				break;
			case 4:
				playerList = defaultWeaponRange_4.playerList;
				break;
			case 5:
				playerList = defaultWeaponRange_5.playerList;
				break;
			}
		}
		foreach (MainCharacter player in playerList) {
			player.BeAttacked (attack);
			player.beingAttacked = true;
			//print(checkWeaponSkill5);

		}
	}

	// need to edit wave in Ma
	public void startDefaultBloodEffct ()
	{
		List<MainCharacter> playerList = new List<MainCharacter> ();
		if (target.inventory.mainWeapon.current != -1) {
			switch (target.inventory.mainWeapon.current) {
			case 1:
				playerList = defaultWeaponRange_1.playerList;
				break;
			case 2:
				playerList = defaultWeaponRange_3.playerList;
				break;
			case 4:
				playerList = defaultWeaponRange_4.playerList;
				break;
			case 5:
				playerList = defaultWeaponRange_5.playerList;
				break;
			}
		}
		/*foreach (Monster target in playerList)
        {
            Instantiate(target.bloodParticle, target.transform.position, target.transform.rotation);
        }*/
	}

	// need to edit arrow in Ma


	protected override IEnumerator WeaponSkill5 ()
	{
		MainCharacter[] playerList = FindObjectsOfType (typeof(MainCharacter)) as MainCharacter[];
		checkWeaponSkill5 = true;
		foreach (MainCharacter p in playerList) {
			p.GetComponent<Animator> ().speed = 0f;
			p.GetComponent<Monster> ().timeLock = true;
		}
		movementSpeed *= 2f;
		yield return new WaitForSecondsRealtime (weaponSkill5Length);
		foreach (MainCharacter p in playerList) {
			p.GetComponent<Animator> ().speed = 1f;
			p.GetComponent<Monster> ().timeLock = false;
		}
		movementSpeed /= 2f;
		checkWeaponSkill5 = false;
	}


}
