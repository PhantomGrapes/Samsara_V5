using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCharacter : Livings
{

	public int maxMana;
	public int mana;
	public int exp;
	public float manaReg;
	public int anger;
	public int energy;
	public float cdReduction;
	public float luck;
	public float arrowRange = 3f;
	public float arrowSpeed = 10f;
	// Weapon equipedWeapon; to do
	// Armor equipedArmor; to do

	// to test whether the player is on the gound
	protected bool grounded;
	protected bool checkDoubleJump;
	protected bool checkJump;
	protected float initGravity;
	// a circle attached to player's foot
	public Transform frontGroundCheck;
	public Transform backGroundCheck;
	public float groundCheckRadius;
	public LayerMask whatIsGround;


//	public ParticleSystem[] parSys;


	//public GameObject weaponToBePickedUp;
	//public GameObject weaponEquiped;

	public ItemToBePickedUp itemToBePickedUp;
	public Inventory inventory;



	// control camera
	public Camera playerCamera;
	// distance between camera and player
	public float xOffset = 0f;
	public float yOffset = 9f;

	// rigibody2d of player
	protected Rigidbody2D rigi;

	// animation
	public Animator anim;

	//attack
	public WeaponRangeController defaultWeaponRange_1;
	public WeaponRangeController defaultWeaponRange_3;
	public WeaponRangeController defaultWeaponRange_4;
	public WeaponRangeController defaultWeaponRange_5;
	protected bool checkAttack;

	//arrow attack
	public GameObject arrow;
	public GameObject rightArmUp;


	//weapon skill
	public GameObject wave;
	public bool checkWeaponSkill = false;
	public WASkillController waRange;
	public float weaponSkill5Length = 3f;
	public bool checkWeaponSkill5;
    bool checkWeaponSkill6;

	// cool down
	public CoolDownController coolDown;

	//beAttacked
	public bool checkBeAttacked;
	//public List<string> forbiddenStateList = new List<string>();
	//public List<string> stateBeforeBeAttacked = new List<string>();

	//changeWeapon
	public SpriteRenderer weaponSprite;

	// play audio
	public MainCharacterAudioController audioController;

	// ban controller
	public ForbiddenStateController ban;

	//roll
	public bool checkRoll;
	protected bool roll = false;
	protected float velocity = 0f;
	public float RollSpeed;
	public bool checkWARoll;
	public float WARollSpeed;

	// UI control
	public bool playerOnTeleport = false;
	public bool playerOnNPC = false;

	

	public void DefaultAttack ()
	{
		anim.SetTrigger ("DefaultAttack");
	}


	protected IEnumerator BanWalk (float banTime)
	{
		ban.walk += 1;
		yield return new WaitForSeconds (banTime);
		ban.walk -= 1;
	}

	protected IEnumerator BanAttack (float banTime)
	{
		ban.attack += 1;
		yield return new WaitForSeconds (banTime);
		ban.attack -= 1;
	}

	protected IEnumerator BanJump (float banTime)
	{
		ban.jump += 1;
		yield return new WaitForSeconds (banTime);
		ban.jump -= 1;
	}

	protected IEnumerator BanSkillAttack (float banTime)
	{
		ban.skillAttack += 1;
		yield return new WaitForSeconds (banTime);
		ban.skillAttack -= 1;
	}

	protected IEnumerator BanBeAttacked (float banTime)
	{
		ban.beAttacked += 1;
		yield return new WaitForSeconds (banTime);
		ban.beAttacked -= 1;
	}

	protected IEnumerator BanRoll (float banTime)
	{
		ban.roll += 1;
		yield return new WaitForSeconds (banTime);
		ban.roll -= 1;
	}

	public float GetAnimLength (string animName)
	{
		float length = 0;
		RuntimeAnimatorController ac = anim.runtimeAnimatorController;    //Get Animator controller
		for (int i = 0; i < ac.animationClips.Length; i++) {                 //For all animations
			if (ac.animationClips [i].name == animName) {        //If it has the same name as your clip
				length = ac.animationClips [i].length;
			}
		}
		return length;
	}

	public void BeAttacked (float damage)
	{
		if (ban.beAttacked == 0) {
			hp -= damage;

			// using forbidden list(old)
			//foreach (string state in forbiddenStateList)
			//    stateBeforeBeAttacked.Add(state);
			//stateBeforeBeAttacked.Add("alreadyLoadedState");

			//using ienumerator
			float banTime = GetAnimLength ("Hit");
			StartCoroutine (BanWalk (banTime));
			StartCoroutine (BanAttack (banTime));
			StartCoroutine (BanJump (banTime));
			StartCoroutine (BanBeAttacked (banTime));
			StartCoroutine (BanSkillAttack (banTime));

			anim.SetTrigger ("BeAttacked");
		}

	}

	protected void Move (Vector2 speed)
	{
		rigi.velocity = speed;
	}

	public virtual void giveDefaultDamageToEnemy ()
	{
		//print("give damage");
		List<Monster> enemyList = new List<Monster> ();
		if (inventory.mainWeapon.current != -1) {
			switch (inventory.mainWeapon.current) {
			case 1:
				enemyList = defaultWeaponRange_1.enemyList;
				break;
			case 3:
				enemyList = defaultWeaponRange_3.enemyList;
				break;
			case 4:
				enemyList = defaultWeaponRange_4.enemyList;
				break;
			case 5:
				enemyList = defaultWeaponRange_5.enemyList;
				break;
			}
		}
		if (enemyList.Count > 2) {
			for (int i = 0; i < 2; i++) {
				enemyList [i].BeAttacked (attack);
			}
		} else {
			foreach (Monster target in enemyList) {
				target.BeAttacked (attack);
				//			target.beingAttacked = true;
				//print(checkWeaponSkill5);

			}
		}

	}

	public void GiveShockWave ()
	{
		wave.transform.localScale = transform.localScale;
		Instantiate (wave, new Vector3 (transform.position.x, transform.position.y - 2f, transform.position.z), transform.rotation);
	}

	public virtual void startDefaultBloodEffct ()
	{
		List<Monster> enemyList = new List<Monster> ();
		if (inventory.mainWeapon.current != -1) {
			switch (inventory.mainWeapon.current) {
			case 1:
				enemyList = defaultWeaponRange_1.enemyList;
				break;
			case 2:
				enemyList = defaultWeaponRange_3.enemyList;
				break;
			case 4:
				enemyList = defaultWeaponRange_4.enemyList;
				break;
			case 5:
				enemyList = defaultWeaponRange_5.enemyList;
				break;
			}
		}
		/*foreach (Monster target in enemyList)
		{
			Instantiate(target.bloodParticle, target.transform.position, target.transform.rotation);
		}*/
	}

	public void startArrow ()
	{
		GameObject arrowBullet = Instantiate (arrow, weaponSprite.transform.position, Quaternion.Euler (new Vector3 (0, 0, 0))) as GameObject;
		arrowBullet.GetComponent<ArrowManager> ().startPosition = arrowBullet.transform.position;
		if (facingRight) {
			arrowBullet.transform.localScale = new Vector3 (1, 1, 1);
			arrowBullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (-arrowSpeed, 0f);
		} else {
			arrowBullet.transform.localScale = new Vector3 (-1, 1, 1);
			arrowBullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (arrowSpeed, 0f);
		}
		//print(arrowBullet.transform.localScale);
	}


	public void start10Arrow ()
	{
		List<GameObject> arrowList = new List<GameObject> ();
		float startAngle = 30f;
		float endAngle = 60f;
		float currentAngle;
		float thisArrowSpeed;
		for (int i = 0; i < 10; i++) {
			arrowList.Add (Instantiate (arrow, weaponSprite.transform.position, Quaternion.Euler (new Vector3 (0, 0, 0))) as GameObject);
			arrowList [i].GetComponent<ArrowManager> ().startPosition = arrowList [i].transform.position;
			currentAngle = Mathf.Deg2Rad * (startAngle + (endAngle - startAngle) / ((i + 1) * 1f));
			thisArrowSpeed = arrowSpeed * Mathf.Sqrt (10 - i) / 2;
			if (facingRight) {
				arrowList [i].transform.localScale = new Vector3 (1, 1, 1);
				currentAngle = currentAngle * Mathf.Rad2Deg + 90;
				arrowList [i].transform.rotation = Quaternion.Euler (new Vector3 (0, 0, currentAngle));
				currentAngle = currentAngle * Mathf.Deg2Rad;
				arrowList [i].GetComponent<Rigidbody2D> ().velocity = new Vector2 (-Mathf.Abs (thisArrowSpeed * Mathf.Cos (currentAngle)), Mathf.Abs (thisArrowSpeed * Mathf.Sin (currentAngle)));
			} else {
				arrowList [i].transform.localScale = new Vector3 (-1, 1, 1);
				arrowList [i].GetComponent<Rigidbody2D> ().velocity = new Vector2 (Mathf.Abs (thisArrowSpeed * Mathf.Cos (currentAngle)), Mathf.Abs (thisArrowSpeed * Mathf.Sin (currentAngle)));
			}
			arrowList [i].GetComponent<Rigidbody2D> ().gravityScale = 1f;
		}
	}

	protected IEnumerator IgnoreCollisionBetweenPlayerAndMonster (float time)
	{
		Physics2D.IgnoreLayerCollision (8, 10, true);
		Physics2D.IgnoreLayerCollision (8, 11, true);
		yield return new WaitForSeconds (time);
		Physics2D.IgnoreLayerCollision (8, 10, false);
		Physics2D.IgnoreLayerCollision (8, 11, false);
	}

	protected virtual IEnumerator WeaponSkill5 ()
	{
		Monster[] MonsterList = FindObjectsOfType (typeof(Monster)) as Monster[];
		checkWeaponSkill5 = true;
		foreach (Monster m in MonsterList) {
			m.GetComponent<Animator> ().speed = 0f;
			m.GetComponent<Monster> ().timeLock = true;
		}
		movementSpeed *= 2f;
		yield return new WaitForSecondsRealtime (weaponSkill5Length);
		foreach (Monster m in MonsterList) {
			m.GetComponent<Animator> ().speed = 1f;
			m.GetComponent<Monster> ().timeLock = false;
		}
		movementSpeed /= 2f;
		checkWeaponSkill5 = false;
	}

	public void CBSkill ()
	{
		if (facingRight) {
			Move (new Vector2 (rigi.velocity.x + movementSpeed * 0.5f / Time.deltaTime, rigi.velocity.y));
		} else {
			Move (new Vector2 (rigi.velocity.x - movementSpeed * 0.5f / Time.deltaTime, rigi.velocity.y));
		}
	}

	// normalize velocity while climbing slopes
	protected void NormalizeSlope ()
	{
		Vector2 vel = rigi.velocity;
		float xSign = 0, ySign = 0;
		if (grounded) {
			RaycastHit2D[] hits = Physics2D.RaycastAll (frontGroundCheck.position, -Vector2.up, 1f, whatIsGround);
			foreach (RaycastHit2D hit in hits) {
				if (hit.collider != null && Mathf.Abs (hit.normal.x) > 0.1f && !hit.collider.isTrigger && !Physics2D.GetIgnoreCollision (hit.collider.GetComponent<EdgeCollider2D> (), GetComponent<PolygonCollider2D> ())) {
					float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
					//print(slopeAngle);
					//print(rigi.gravityScale);
					//print(grounded);
					rigi.gravityScale = 0f;
					xSign = Mathf.Sign (vel.x);
					//ySign = Mathf.Sign(vel.y);
					if (Mathf.Abs (vel.x) < 0.01f)
						xSign = facingRight ? -1 : 1;
					//if (Mathf.Abs(vel.y) < 0.01f)
					ySign = (hit.normal.x > 0 && facingRight) || (hit.normal.x < 0 && !(facingRight)) ? 1 : -1;
       
					var norm = Mathf.Sqrt (vel.x * vel.x + vel.y * vel.y);

					rigi.velocity = new Vector2 (norm * Mathf.Abs (Mathf.Cos (slopeAngle * Mathf.Deg2Rad)) * xSign, norm * Mathf.Abs (Mathf.Sin (slopeAngle * Mathf.Deg2Rad)) * ySign);
					return;
				}
			}
		}
		rigi.gravityScale = initGravity;
		return;
	}
	// Use this for initialization
	protected void Start ()
	{

		//playerCamera = FindObjectOfType<Camera>();
		rigi = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		weaponSprite = FindObjectOfType<CharacterWeaponManager> ().GetComponent<SpriteRenderer> ();
		rightArmUp = GameObject.Find ("MainCharacter/Hip/Corp/RightArmUp");
		audioController = GetComponent<MainCharacterAudioController> ();
		ban = GetComponent<ForbiddenStateController> ();
		waRange = FindObjectOfType<WASkillController> ();
		checkWeaponSkill5 = false;
		coolDown = FindObjectOfType<CoolDownController> ();
		inventory = FindObjectOfType<Inventory> ();
		//print (coolDown);
		initGravity = rigi.gravityScale;




		anim.SetBool ("Alive", alive);
		//defaultWeaponRange = FindObjectOfType<WeaponRangeController>();


	}


	void FixedUpdate ()
	{
//		parSys = FindObjectsOfType<ParticleSystem>();
		// to check whether the ground overlap the circle on player's foot
		grounded = Physics2D.OverlapCircle (frontGroundCheck.position, groundCheckRadius, whatIsGround) || Physics2D.OverlapCircle (backGroundCheck.position, groundCheckRadius, whatIsGround);
        // weapon skill 6 need to move backward while character face forward, so don't need flip
        if (!checkWeaponSkill6)
            Flipping();
        if (!checkJump)
        {
            NormalizeSlope();
        }
    }



	// Update is called once per frame
	void Update ()
	{
        
        CheckStatus();
        // tab change weapon
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            changeWeapon();
        }

        // use medicine
        if (Input.GetKeyDown(KeyCode.Q))
        {
            useMedicine();
        }

        // seperated from useSkill
        SetAxeAttackRange();
        /*
		 * key controls
		 */
        // call function to pickup or drop Weapon
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpItem();
        }

        // integrated jump and double jump to one function
        if (Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.S))
        {
            Jump();
        }

        // roll, move left or right, roll left or right, or normal attack
        if (velocity != 0)
            Move(new Vector2(velocity, rigi.velocity.y));
        else if (Input.GetKeyDown(KeyCode.J))
        {
            NormalAttack();
        }
        else if (Input.GetKeyDown(KeyCode.K) && !checkAttack && !checkWeaponSkill && ban.skillAttack == 0)
        {
            useSkill();
        }
 
        else if (Input.GetKeyDown(KeyCode.U) && !Input.GetKey(KeyCode.O))
        {
            RollLeft();
        }
        else if (Input.GetKeyDown(KeyCode.O) && !Input.GetKey(KeyCode.U))
        {
            RollRight();
        }
		else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
		{
			MoveLeft();
		}
		else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
		{
			MoveRight();
		}
        else
        {
            Move(new Vector2(0f, rigi.velocity.y));
        }
    }

//	void FlipParSys(){
//		foreach (ParticleSystem par in parSys) {
//			if (!facingRight && par.CompareTag("ParticleSysFlip")) {
//				var tempX = par.transform.localScale.x;
//				var tempY = par.transform.localScale.y;
//				var tempZ = par.transform.localScale.z;
//				par.transform.localScale = new Vector3 (-tempX, tempY, tempZ);
//			} 
//		}
//	}

	protected void CheckStatus ()
	{
		velocity = 0f;
		roll = false;
		if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("DefaultAttack"))
			checkAttack = true;
		else
			checkAttack = false;

		if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("Roll"))
			checkRoll = true;
		else
			checkRoll = false;

		// to see whether the player is using weapon skill
		if (inventory.mainWeapon.current != -1 && anim.GetCurrentAnimatorStateInfo (0).IsTag ("WeaponSkill_" + inventory.mainWeapon.current)) {
			checkWeaponSkill = true;
		} else
			checkWeaponSkill = false;

		if (inventory.mainWeapon.current != -1 && anim.GetCurrentAnimatorStateInfo (0).IsTag ("WeaponSkill_3"))
			checkWARoll = true;
		else
			checkWARoll = false;

        if (inventory.mainWeapon.current != -1 && anim.GetCurrentAnimatorStateInfo(0).IsTag("WeaponSkill_6"))
            checkWeaponSkill6 = true;
        else
            checkWeaponSkill6 = false;

        // to see whether the player is being attacked

        if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("Hit"))
			checkBeAttacked = true;
		else
			checkBeAttacked = false;

		// jump
		if (grounded) {
			checkDoubleJump = false;
			checkJump = false;
		}

		// roll
		if (checkRoll)
			velocity = RollSpeed;
		if (checkWARoll)
			velocity = WARollSpeed;
		if (!alive)
			velocity = 0f;
        

		if (hp <= 0)
			alive = false;
		else
			alive = true;

		// animation control
		anim.SetFloat ("XSpeed", Mathf.Abs (rigi.velocity.x));
		anim.SetFloat ("YSpeed", Mathf.Abs (rigi.velocity.y));
		anim.SetBool ("Grounded", grounded);
		anim.SetBool ("Alive", alive);
	}

	protected override void MoveLeft ()
	{
		if (!checkAttack && !checkWeaponSkill && ban.walk == 0) {
			velocity = -movementSpeed;
		}
		Move (new Vector2 (velocity, rigi.velocity.y));

	}

	protected override void MoveRight ()
	{
		if (!checkAttack && !checkWeaponSkill && ban.walk == 0) {
			velocity = movementSpeed;
		}
		Move (new Vector2 (velocity, rigi.velocity.y));

	}

	protected void RollLeft ()
	{
		if (!checkAttack && !checkWeaponSkill && ban.roll == 0) {
			StartCoroutine (BanRoll (coolDown.coolDowns [1].coolDownLength));
			coolDown.coolDowns [1].currentCoolDown = 0f;
			roll = true;
			RollSpeed = -movementSpeed;
            Roll();
        }

	}

	protected void RollRight ()
	{
		if (!checkAttack && !checkWeaponSkill && ban.roll == 0) {
			StartCoroutine (BanRoll (coolDown.coolDowns [1].coolDownLength));
			coolDown.coolDowns [1].currentCoolDown = 0f;
			roll = true;
			RollSpeed = movementSpeed;
            Roll();
        }
		

	}

	protected void Roll ()
	{
		Move (new Vector2 (velocity, rigi.velocity.y));
		if (roll && alive) {
			float rollLength = 0;
			rollLength = GetAnimLength ("Roll");
			StartCoroutine (BanWalk (rollLength));
			//StartCoroutine(BanJump(rollLength));
			StartCoroutine (BanBeAttacked (rollLength));
			StartCoroutine (BanRoll (rollLength));
			StartCoroutine (IgnoreCollisionBetweenPlayerAndMonster (rollLength));
			anim.SetTrigger ("Roll");
		}


	}


	protected override void Jump ()
	{
		if (!checkWeaponSkill && grounded && ban.jump == 0 && alive) {
			Move (new Vector2 (rigi.velocity.x, jumpForce));
			checkJump = true;
			rigi.gravityScale = initGravity;
		} else if (!checkWeaponSkill && !grounded && !checkDoubleJump && ban.jump == 0 && alive) {
			Move (new Vector2 (rigi.velocity.x, jumpForce));
			checkDoubleJump = true;
			anim.SetTrigger ("DoubleJump");
		}

	}

	protected void useMedicine ()
	{
		int id = inventory.database.FetchItemByName ("Medicine").Id;
		int pos = inventory.itemPositionInInventory (id);
		if (pos == -1)
			return;
		inventory.delItemById (id);
		hp = hp + inventory.items [pos].attack > maxHp ? maxHp : hp + inventory.items [pos].attack;
	}

	protected void SetAxeAttackRange ()
	{
		// WASkill range control
		if (checkWARoll && !waRange.gameObject.activeSelf)
			waRange.gameObject.SetActive (true);
		else if (!checkWARoll && waRange.gameObject.activeSelf)
			waRange.gameObject.SetActive (false);
	}

    protected void useSkill()
    {
        // put into SkillAttack()
        if (inventory.mainWeapon.current != -1)
        {
            StartCoroutine(BanSkillAttack(coolDown.coolDowns[0].coolDownLength));
            coolDown.coolDowns[0].currentCoolDown = 0f;
            Move(new Vector2(0, 0));
            anim.SetTrigger("WeaponSkill_" + inventory.mainWeapon.current);
            switch (inventory.mainWeapon.current)
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
	/*float CalculateAngleToMouse(GameObject source)
	{
		var mousePos = Input.mousePosition;
		var objectPos = Camera.main.WorldToScreenPoint(source.transform.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;
		return Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
	}*/

	protected void PickUpWeapon ()
	{
		inventory.AddItem (itemToBePickedUp.id);
		if (inventory.mainWeapon.current == -1) {
			inventory.eventSetMainWeaponById (itemToBePickedUp.id);
            
		} else if (inventory.secondWeapon.current == -1)
			inventory.eventSetSecondWeaponById (itemToBePickedUp.id);
		itemToBePickedUp.gameObject.SetActive (false);
	}

	protected void PickUpSoul ()
	{
		inventory.AddItem (itemToBePickedUp.id);
		itemToBePickedUp.gameObject.SetActive (false);
	}

	protected void PickUpItem ()
	{
		if (itemToBePickedUp == null)
			return;
		if (itemToBePickedUp.id == inventory.database.FetchItemByName ("Soul").Id)
			PickUpSoul ();
		else if (itemToBePickedUp.id > 0 && itemToBePickedUp.id < 7)
			PickUpWeapon ();
	}
	protected void NormalAttack ()
	{
		if (!checkAttack && !checkWeaponSkill && ban.attack == 0 && inventory.mainWeapon.current != -1) {
			// put into defaultAttack()
			Move (new Vector2 (0, 0));
			switch (inventory.mainWeapon.current) {
			case 1:
				DefaultAttack ();
				break;
			case 2:
				DefaultAttack ();
				break;
			case 3:
				DefaultAttack ();
				break;
			case 4:
				DefaultAttack ();
				break;
			case 5:
				DefaultAttack ();
				break;
			case 6:
				DefaultAttack ();
				break;
			}
		}
	}

	protected void OnCheckAttack(){

		this.checkAttack = true;
		print (this.checkAttack);
	}

	protected void OffCheckAttack(){
		this.checkAttack = false;
		print (this.checkAttack);
	}

	public bool GetCheckAttack(){
		return this.checkAttack;
	}

	void changeWeapon ()
	{
		int mainWeapon = inventory.mainWeapon.current;
		int secondWeapon = inventory.secondWeapon.current;

		if (mainWeapon == -1 || secondWeapon == -1)
			return;
		inventory.eventSetMainWeaponById (secondWeapon);
		inventory.eventSetSecondWeaponById (mainWeapon);
	}
	// audio fonctions
	protected void playHit ()
	{
		audioController.audioHit.Play ();
	}

	protected void playDead ()
	{
		audioController.audioDead.Play ();
	}

	protected void playHeavySword1 ()
	{
		audioController.audioHeavySword1.Play ();
	}

	protected void playHeavySword2 ()
	{
		audioController.audioHeavySword2.Play ();
	}

	protected void playArrow1 ()
	{
		audioController.audioArrow1.Play ();
	}

	protected void StopArrow1 ()
	{
		audioController.audioArrow1.Stop ();
	}

	protected void playArrow2 ()
	{
		audioController.audioArrow2.Play ();
	}

	protected void playHeavySwordSkill1 ()
	{
		audioController.audioHeavySwordSkill1.Play ();
	}



}
