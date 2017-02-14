using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class BossMa : MainCharacter {


	void Start()
	{
		rigi = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		weaponSprite = FindObjectOfType<CharacterWeaponManager>().GetComponent<SpriteRenderer>();
		rightArmUp = GameObject.Find("MainCharacter/Hip/Corp/RightArmUp");
		audioController = GetComponent<MainCharacterAudioController>();
		ban = GetComponent<ForbiddenStateController>();
		waRange = FindObjectOfType<WASkillController>();
		checkWeaponSkill5 = false;
		coolDown = FindObjectOfType<CoolDownController>();
		initGravity = rigi.gravityScale;

		anim.SetBool("Alive", alive);
		//defaultWeaponRange = FindObjectOfType<WeaponRangeController>();
	}

	/***
	void FixedUpdate()
	{
		// to check whether the ground overlap the circle on player's foot
		grounded = Physics2D.OverlapCircle(frontGroundCheck.position, groundCheckRadius, whatIsGround) || Physics2D.OverlapCircle(backGroundCheck.position, groundCheckRadius, whatIsGround);
		if (!checkJump)
			NormalizeSlope();
	}

	// Update is called once per frame
	void Update()
	{

		// update the weapon
		//if (weaponEquiped)
		//    weaponSprite.sprite = weaponEquiped.GetComponent<SpriteRenderer>().sprite;
		//else
		//    weaponSprite.sprite = null;

		// call function to pickup Weapon
		if (Input.GetKeyDown(KeyCode.G))
		{
			// pickup weapon if not equiping any
			if (anim.GetInteger("WeaponIndex") == 0)
			{
				PickUpWeapon();
			}
			else
			{
				// drop weapon if equiped
				DropWeapon();
			}
		}


		// to see whether the player is attacking

		if (anim.GetCurrentAnimatorStateInfo(0).IsTag("DefaultAttack"))
			checkAttack = true;
		else
			checkAttack = false;

		if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Roll"))
			checkRoll = true;
		else
			checkRoll = false;
		// to see whether the player is using weapon skill
		if (weaponEquiped && anim.GetCurrentAnimatorStateInfo(0).IsTag("WeaponSkill_" + weaponEquiped.GetComponent<Weapon>().index))
			checkWeaponSkill = true;
		else
			checkWeaponSkill = false;

		if (weaponEquiped && anim.GetCurrentAnimatorStateInfo(0).IsTag("WeaponSkill_3"))
			checkWARoll = true;
		else
			checkWARoll = false;

		// to see whether the player is being attacked

		if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Hit"))
			checkBeAttacked = true;
		else
			checkBeAttacked = false;

		// jump
		if (grounded)
		{
			checkDoubleJump = false;
			checkJump = false;
		}
		if (Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.S) && !checkWeaponSkill && grounded && ban.jump == 0 && alive)
		{
			Move(new Vector2(rigi.velocity.x, jumpForce));
			checkJump = true;
			rigi.gravityScale = initGravity;
			//print("1");
		}
		else if (Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.S) && !checkWeaponSkill && !grounded && !checkDoubleJump && ban.jump == 0 && alive)
		{
			Move(new Vector2(rigi.velocity.x, jumpForce));
			checkDoubleJump = true;
			anim.SetTrigger("DoubleJump");
			//print("2");
		}

		float velocity = 0;
		if (Input.GetKey(KeyCode.A) && !checkAttack && !checkWeaponSkill && ban.walk == 0)
			velocity = -movementSpeed;
		if (Input.GetKey(KeyCode.D) && !checkAttack && !checkWeaponSkill && ban.walk == 0)
			velocity = movementSpeed;
		bool roll = false;
		if (Input.GetKeyDown(KeyCode.U) && !checkAttack && !checkWeaponSkill && ban.roll == 0)
		{
			StartCoroutine(BanRoll(coolDown.coolDowns[1].coolDownLength));
			coolDown.coolDowns[1].currentCoolDown = 0f;
			roll = true;
			RollSpeed = -movementSpeed;
		}
		if (Input.GetKeyDown(KeyCode.O) && !checkAttack && !checkWeaponSkill && ban.roll == 0)
		{
			StartCoroutine(BanRoll(coolDown.coolDowns[1].coolDownLength));
			coolDown.coolDowns[1].currentCoolDown = 0f;
			roll = true;
			RollSpeed = movementSpeed;
		}
		if (checkRoll)
			velocity = RollSpeed;
		if (checkWARoll)
			velocity = WARollSpeed;
		if (!alive)
			velocity = 0f;
		//Move(NormalizeSlope(new Vector2(velocity, rigi.velocity.y)));
		Move(new Vector2(velocity, rigi.velocity.y));

		//print(rigi.velocity.x);
		if (roll && alive)
		{
			float rollLength = 0;
			rollLength = GetAnimLength("Roll");
			StartCoroutine(BanWalk(rollLength));
			//StartCoroutine(BanJump(rollLength));
			StartCoroutine(BanBeAttacked(rollLength));
			StartCoroutine(BanRoll(rollLength));
			StartCoroutine(IgnoreCollisionBetweenPlayerAndMonster(rollLength));
			anim.SetTrigger("Roll");
		}


		// check the direction of face
		if (rigi.velocity.x > 0)
			facingRight = false;
		else if (rigi.velocity.x < 0)
			facingRight = true;

		// to turn the face to correct direction
		Flipping();

		// check alive
		if (hp <= 0)
			alive = false;
		else
			alive = true;
		//print(Physics2D.GetIgnoreLayerCollision(8, 10));

		// weapon skill

		// WASkill range control
		if (checkWARoll && !waRange.gameObject.activeSelf)
			waRange.gameObject.SetActive(true);
		else if(!checkWARoll && waRange.gameObject.activeSelf)
			waRange.gameObject.SetActive(false);
		// manage skill attack input
		if (Input.GetKeyDown(KeyCode.K) && !checkAttack && !checkWeaponSkill && ban.skillAttack == 0)
		{
			if (weaponEquiped)
			{
				StartCoroutine(BanSkillAttack(coolDown.coolDowns[0].coolDownLength));
				coolDown.coolDowns[0].currentCoolDown = 0f;
				Move(new Vector2(0, 0));
				anim.SetTrigger("WeaponSkill_" + weaponEquiped.GetComponent<Weapon>().index);
				switch (weaponEquiped.GetComponent<Weapon>().index)
				{
				case 1:

					break;
				case 2:

					break;
				case 3:
					float WARollLength = 0;
					WARollLength = GetAnimLength("SkillWA");
					StartCoroutine(IgnoreCollisionBetweenPlayerAndMonster(WARollLength));
					StartCoroutine(BanBeAttacked(WARollLength));
					if (facingRight)
						WARollSpeed = -movementSpeed;
					else
						WARollSpeed = movementSpeed;
					break;
				case 4:

					break;
				case 5:
					StartCoroutine(BanSkillAttack(weaponSkill5Length));
					StartCoroutine(BanBeAttacked(weaponSkill5Length));
					StartCoroutine(WeaponSkill5());
					StartCoroutine(IgnoreCollisionBetweenPlayerAndMonster(weaponSkill5Length));
					break;
				case 6:

					break;
				}

			}
		}
		// animation control
		anim.SetFloat("XSpeed", Mathf.Abs(rigi.velocity.x));
		anim.SetFloat("YSpeed", Mathf.Abs(rigi.velocity.y));
		anim.SetBool("Grounded", grounded);
		anim.SetBool("Alive", alive);
		//attacks
		if (Input.GetKeyDown(KeyCode.J) && !checkAttack && !checkWeaponSkill && ban.attack == 0 && weaponEquiped != null)
		{
			Move(new Vector2(0, 0));
			switch (weaponEquiped.GetComponent<Weapon>().index)
			{
			case 1:
				DefaultAttack();
				break;
			case 2:
				DefaultAttack();
				break;
			case 3:
				DefaultAttack();
				break;
			case 4:
				DefaultAttack();
				break;
			case 5:
				DefaultAttack();
				break;
			case 6:
				DefaultAttack();
				break;
			}
		}

	}
	***/
		
	// Overriding functions in Maincharacter
	public override void giveDefaultDamageToEnemy()
	{
		//print("give damage");
		List<MainCharacter> playerList = new List<MainCharacter>();
		if (weaponEquiped)
		{
			switch (weaponEquiped.GetComponent<Weapon>().index)
			{
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
		foreach (MainCharacter target in playerList)
		{
			target.BeAttacked(attack);
			target.beingAttacked = true;
			//print(checkWeaponSkill5);

		}
	}

	// need to edit wave in Ma
	public void startDefaultBloodEffct()
	{
		List<MainCharacter> playerList = new List<MainCharacter>();
		if (weaponEquiped)
		{
			switch (weaponEquiped.GetComponent<Weapon>().index)
			{
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


	protected override IEnumerator WeaponSkill5()
	{
		MainCharacter[] playerList = FindObjectsOfType(typeof(MainCharacter)) as MainCharacter[];
		checkWeaponSkill5 = true;
		foreach(MainCharacter p in playerList)
		{
			p.GetComponent<Animator>().speed = 0f;
			p.GetComponent<Monster>().timeLock = true;
		}
		movementSpeed *= 2f;
		yield return new WaitForSecondsRealtime(weaponSkill5Length);
		foreach (MainCharacter p in playerList)
		{
			p.GetComponent<Animator>().speed = 1f;
			p.GetComponent<Monster>().timeLock = false;
		}
		movementSpeed /= 2f;
		checkWeaponSkill5 = false;
	}

}
