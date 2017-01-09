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
    private bool grounded;
    // a circle attached to player's foot 
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask whatIsGround;


    public GameObject weaponToBePickedUp;
    public GameObject weaponEquiped;


    // control camera
    public Camera playerCamera;
    // distance between camera and player
    public float xOffset = 0f;
    public float yOffset = 9f;

    // rigibody2d of player
    private Rigidbody2D rigi;

    // animation 
    public Animator anim;

    //attack
    public WeaponRangeController defaultWeaponRange_1;
    public WeaponRangeController defaultWeaponRange_3;
    public WeaponRangeController defaultWeaponRange_4;
    public WeaponRangeController defaultWeaponRange_5;
    public bool checkAttack = false;

    //arrow attack
    public int BAAAttackState = 0;
    public GameObject arrow;
    public GameObject rightArmUp;
    public List<GameObject> arrowList = new List<GameObject>();


    //weapon skill
    public GameObject wave;
    public bool checkWeaponSkill = false;
    public WASkillController waRange;
    public float weaponSkill5Length = 3f;

    //beAttacked
    public bool checkBeAttacked;
    //public List<string> forbiddenStateList = new List<string>();
    //public List<string> stateBeforeBeAttacked = new List<string>();

    //changeWeapon
    private SpriteRenderer weaponSprite;

    // play audio
    public MainCharacterAudioController audioController;

    // ban controller
    public ForbiddenStateController ban;

    //roll 
    public bool checkRoll;
    public float RollSpeed;
    public bool checkWARoll;
    public float WARollSpeed;

    public void Interact()
    {
        // to do
    }

    public void Dash()
    {
        // to do
    }

    public void Smash()
    {
        // to do
    }

    public void UseMedKit()
    {
        // to do
    }

    public void UseItem1()
    {
        // to do
    }

    public void UseItem2()
    {
        // to do
    }

    public void UseItem3()
    {
        // to do
    }

    public void UseItem4()
    {
        // to do
    }

    public void UseItem5()
    {
        // to do
    }

    public void UseItem6()
    {
        // to do
    }

    public void WeaponDefaultSkill()
    {
        // to do
    }

    public void DefaultAttack()
    {
        anim.SetTrigger("DefaultAttack");
    }


    IEnumerator BanWalk(float banTime)
    {
        ban.walk += 1;
        yield return new WaitForSeconds(banTime);
        ban.walk -= 1;
    }

    IEnumerator BanAttack(float banTime)
    {
        ban.attack += 1;
        yield return new WaitForSeconds(banTime);
        ban.attack -= 1;
    }

    IEnumerator BanJump(float banTime)
    {
        ban.jump += 1;
        yield return new WaitForSeconds(banTime);
        ban.jump -= 1;
    }

    IEnumerator BanSkillAttack(float banTime)
    {
        ban.skillAttack += 1;
        yield return new WaitForSeconds(banTime);
        ban.skillAttack -= 1;
    }

    IEnumerator BanBeAttacked(float banTime)
    {
        ban.beAttacked += 1;
        yield return new WaitForSeconds(banTime);
        ban.beAttacked -= 1;
    }

    IEnumerator BanRoll(float banTime)
    {
        ban.roll += 1;
        yield return new WaitForSeconds(banTime);
        ban.roll -= 1;
    }

    public float GetAnimLength(string animName)
    {
        float length = 0;
        RuntimeAnimatorController ac = anim.runtimeAnimatorController;    //Get Animator controller
        for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
        {
            if (ac.animationClips[i].name == animName)        //If it has the same name as your clip
            {
                length = ac.animationClips[i].length;
            }
        }
        return length;
    }

    public void BeAttacked(float damage)
    {
        if (ban.beAttacked == 0)
        {
            hp -= damage;
            BAAAttackState = 0;
            foreach (GameObject arrowBullet in arrowList)
                Destroy(arrowBullet);

            // using forbidden list(old)
            //foreach (string state in forbiddenStateList)
            //    stateBeforeBeAttacked.Add(state);
            //stateBeforeBeAttacked.Add("alreadyLoadedState");

            //using ienumerator
            float banTime = GetAnimLength("Hit");
            StartCoroutine(BanWalk(banTime));
            StartCoroutine(BanAttack(banTime));
            StartCoroutine(BanJump(banTime));
            StartCoroutine(BanBeAttacked(banTime));
            StartCoroutine(BanSkillAttack(banTime));

            anim.SetTrigger("BeAttacked");
        }

    }

    public void AdditionalSkill1()
    {
        // to do
    }

    public void AdditionalSkill2()
    {
        // to do
    }

    public void AdditionalSkill3()
    {
        // to do
    }

    public void AdditionalSkill4()
    {
        // to do
    }


    public void PickUp()
    {
        // to do
    }



    void FixedUpdate()
    {
        // to check whether the ground overlap the circle on player's foot
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private void Move(Vector2 speed)
    {
        rigi.velocity = speed;
    }

    public void giveDefaultDamageToEnemy()
    {
        //print("give damage");
        List<Minion> enemyList = new List<Minion>();
        if (weaponEquiped)
        {
            switch (weaponEquiped.GetComponent<Weapon>().index)
            {
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
        foreach (Minion target in enemyList)
        {
            target.beAttacked(attack);
            target.beingAttacked = true;
        }
    }

    public void GiveShockWave()
    {
        wave.transform.localScale = transform.localScale;
        Instantiate(wave, new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z), transform.rotation);
    }
    public void startDefaultBloodEffct()
    {
        List<Minion> enemyList = new List<Minion>();
        if (weaponEquiped)
        {
            switch (weaponEquiped.GetComponent<Weapon>().index)
            {
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
        foreach (Minion target in enemyList)
        {
            Instantiate(target.bloodParticle, target.transform.position, target.transform.rotation);
        }
    }

    public void startArrow()
    {
        GameObject arrowBullet = Instantiate(arrow, weaponSprite.transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject;
        arrowBullet.GetComponent<ArrowManager>().startPosition = arrowBullet.transform.position;
        if (facingRight)
            arrowBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(-arrowSpeed, 0f);
        else
        {
            arrowBullet.transform.localScale = new Vector3(-1, 1, 1);
            arrowBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(arrowSpeed, 0f);
        }
    }


    public void start10Arrow()
    {
        List<GameObject> arrowList = new List<GameObject>();
        float startAngle = 30f;
        float endAngle = 60f;
        float currentAngle;
        float thisArrowSpeed;
        for(int i=0; i<10; i++)
        {
            arrowList.Add(Instantiate(arrow, weaponSprite.transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject);
            arrowList[i].GetComponent<ArrowManager>().startPosition = arrowList[i].transform.position;
            currentAngle = Mathf.Deg2Rad* (startAngle + (endAngle - startAngle) / ((i + 1) * 1f));
            thisArrowSpeed = arrowSpeed*Mathf.Sqrt(10-i)/2;
        if (facingRight)
            {
                currentAngle = currentAngle*Mathf.Rad2Deg + 90;
                arrowList[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle));
                currentAngle = currentAngle * Mathf.Deg2Rad;
                arrowList[i].GetComponent<Rigidbody2D>().velocity = new Vector2(-Mathf.Abs(thisArrowSpeed * Mathf.Cos(currentAngle)), Mathf.Abs(thisArrowSpeed * Mathf.Sin(currentAngle)));
            }
            else
            {
                arrowList[i].transform.localScale = new Vector3(-1, 1, 1);
                arrowList[i].GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Abs(thisArrowSpeed * Mathf.Cos(currentAngle)), Mathf.Abs(thisArrowSpeed * Mathf.Sin(currentAngle)));
            }
            arrowList[i].GetComponent<Rigidbody2D>().gravityScale = 1f;
        }
    }

    IEnumerator IgnoreCollisionBetweenPlayerAndMinion(float time)
    {
        Physics2D.IgnoreLayerCollision(8, 10, true);
        yield return new WaitForSeconds(time);
        Physics2D.IgnoreLayerCollision(8, 10, false);

    }

    IEnumerator WeaponSkill5()
    {
        Minion[] minionList = FindObjectsOfType(typeof(Minion)) as Minion[];
        foreach(Minion m in minionList)
        {
            m.GetComponent<Animator>().speed = 0f;
            //m.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            //m.GetComponent<Rigidbody2D>().isKinematic = true;
        }
        movementSpeed *= 10f;
        //movementSpeed *= weaponSkill5Scale;
        //anim.speed *= weaponSkill5Scale;
        yield return new WaitForSecondsRealtime(weaponSkill5Length);
        foreach (Minion m in minionList)
        {
            m.GetComponent<Animator>().speed = 1f;
            //m.GetComponent<Rigidbody2D>().isKinematic = false;
        }
        movementSpeed /= 10f;
        //Time.timeScale *= weaponSkill5Scale;
        //anim.speed /= weaponSkill5Scale;
        //movementSpeed /= weaponSkill5Scale;
    }

    // Use this for initialization
    void Start()
    {
        //playerCamera = FindObjectOfType<Camera>();
        rigi = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        weaponSprite = FindObjectOfType<CharacterWeaponManager>().GetComponent<SpriteRenderer>();
        rightArmUp = GameObject.Find("MainCharacter/Hip/Corp/RightArmUp");
        audioController = GetComponent<MainCharacterAudioController>();
        ban = GetComponent<ForbiddenStateController>();
        waRange = FindObjectOfType<WASkillController>();

        anim.SetBool("Alive", alive);
        //defaultWeaponRange = FindObjectOfType<WeaponRangeController>();
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


        // set the position of camera
        playerCamera.transform.position = new Vector3(GetComponent<Transform>().position.x + xOffset, GetComponent<Transform>().position.y + yOffset, playerCamera.transform.position.z);

        // movements
        if (Input.GetKeyDown(KeyCode.Space) && !checkWeaponSkill && grounded && ban.jump == 0)
        {
            Move(new Vector2(rigi.velocity.x, jumpForce));
        }
        float velocity = 0;
        if (Input.GetKey(KeyCode.A) && !checkAttack && !checkWeaponSkill && ban.walk == 0)
            velocity = -movementSpeed;
        if (Input.GetKey(KeyCode.D) && !checkAttack && !checkWeaponSkill && ban.walk == 0)
            velocity = movementSpeed;
        bool roll = false;
        if (Input.GetKeyDown(KeyCode.U) && !checkAttack && !checkWeaponSkill && ban.roll == 0)
        {
            roll = true;
            RollSpeed = -movementSpeed;
        }
        if (Input.GetKeyDown(KeyCode.O) && !checkAttack && !checkWeaponSkill && ban.roll == 0)
        {
            roll = true;
            RollSpeed = movementSpeed;
        }
        if (checkRoll)
            velocity = RollSpeed;
        if (checkWARoll)
            velocity = WARollSpeed;
        Move(new Vector2(velocity, rigi.velocity.y));
        if (roll)
        {
            float rollLength = 0;
            rollLength = GetAnimLength("Roll");
            StartCoroutine(BanWalk(rollLength));
            //StartCoroutine(BanJump(rollLength));
            StartCoroutine(BanBeAttacked(rollLength));
            StartCoroutine(BanRoll(rollLength));
            StartCoroutine(IgnoreCollisionBetweenPlayerAndMinion(rollLength));
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
                        StartCoroutine(IgnoreCollisionBetweenPlayerAndMinion(WARollLength));
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
                        StartCoroutine(WeaponSkill5());
                        StartCoroutine(IgnoreCollisionBetweenPlayerAndMinion(weaponSkill5Length));
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
    // code for picking up weapon from the ground or drop it
    // set the weapon to be available for pickup when collide
//    void OnTriggerStay2D(Collider2D col)
//    {
//        if (col.gameObject.CompareTag("Weapon"))
//        {
//            //print ("1");
//            weaponToBePickedUp = col.gameObject;
//        }
//        else
//        {
//            //print ("2");
//        }
//    }
//
//    // set the weapon to be unavailable for pickup when they no longer collide
//    void OnTriggerExit2D(Collider2D col)
//    {
//        if (col.gameObject.CompareTag("Weapon"))
//        {
//            weaponToBePickedUp = null;
//        }
//    }

    void PickUpWeapon()
    {
        if (weaponToBePickedUp != null)
        {
            anim.SetInteger("WeaponIndex", weaponToBePickedUp.GetComponent<Weapon>().index);
            weaponEquiped = weaponToBePickedUp;
            weaponToBePickedUp.SetActive(false);
            switch (weaponToBePickedUp.GetComponent<Weapon>().index)
            {
                case 1:
                    weaponSprite.sprite = weaponEquiped.GetComponent<SpriteRenderer>().sprite;
                    break;
                case 2:
                    weaponSprite.sprite = weaponEquiped.GetComponent<SpriteRenderer>().sprite;
                    break;

            }
        }
        else
        {
            print("No weapon on the ground!");
        }
    }

    void DropWeapon()
    {
        anim.SetInteger("WeaponIndex", 0);
        this.weaponEquiped.transform.position = this.transform.position;
        this.weaponEquiped.SetActive(true);
        switch (weaponEquiped.GetComponent<Weapon>().index)
        {
            case 1:
                weaponSprite.sprite = null;
                break;
            case 2:
                weaponSprite.sprite = null;
                break;

        }
        this.weaponEquiped = null;
    }

    // audio fonctions
    void playHit()
    {
        audioController.audioHit.Play();
    }

    void playDead()
    {
        audioController.audioDead.Play();
    }

    void playHeavySword1()
    {
        audioController.audioHeavySword1.Play();
    }
    void playHeavySword2()
    {
        audioController.audioHeavySword2.Play();
    }
    void playArrow1()
    {
        audioController.audioArrow1.Play();
    }
    void StopArrow1()
    {
        audioController.audioArrow1.Stop();
    }
    void playArrow2()
    {
        audioController.audioArrow2.Play();
    }
    void playHeavySwordSkill1()
    {
        audioController.audioHeavySwordSkill1.Play();
    }

}