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
    float sADP = 0.9f;
    //startAttackingDistanceProportion

    public bool isRoaming = false;

    Vector2 roamTo;

    public GameObject bloodParticle;

	EnemyMeleeWeaponCollider minionWeaponCollider;



    // Use this for initialization
    public void Start()
    {
        this.target = FindObjectOfType<MainCharacter>();
        this.alive = true;
        // physical resistance formula
        this.physicalResistance = 1 - Mathf.Exp(-armor / 3);

        // attack interval formula
        this.attackInterval = 2 / this.attackSpeed;

        anim = GetComponent<Animator>();
        GetComponent<Rigidbody2D>().mass = 100f;    // make the minion very heavy so that it cannot be pushed back

        movementSpeed = 2f;
		originalSpeed = this.movementSpeed;
        jumpForce = 10f;

		minionWeaponCollider = FindObjectOfType<EnemyMeleeWeaponCollider> ();
    }

    // Update is called once per frame
    void Update()
    {

        if (this != null)
        {

            MinionFlipping();
        }
    }

    void FixedUpdate()
    {
        anim.SetFloat("xSpeed", Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x));
        selfPosition = GetComponent<Rigidbody2D>().position;
        targetPosition = target.GetComponent<Rigidbody2D>().position;
        CooldownChecker();
        DecideState();

		if (this.timeLock) {
			this.movementSpeed = 0f;
		} else {
			this.movementSpeed = originalSpeed;
		}

    }


    void DecideState()
    {
        if (alive)
        {

            // check if alive first
            if (hp <= 0)
            {
                Die();
                this.anim.SetBool("alive", false);
            }
            else if (hp < maxHp * retreatPercent)
            {
                isRoaming = false;              // if low heath, retreat
                Retreat();
            }
            else if (Vector2.Distance(selfPosition, targetPosition) > alertDistance)
            {
                Roam();     // if far, roam
            }
            else
            {
                isRoaming = false;  // else be aggressive
                Aggressive();
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            StartCoroutine(RemoveMinion());

        }
    }

    // States of the minion: roaming, aggresive, retreat
    void Roam()
    {
        bool reached = true;

        // if reached, set new target position
        isRoaming = true;
        if (reached)
        {
            float newX = Random.Range(-roamRange, roamRange);
            if (Mathf.Abs(newX) < roamRange * 0.5)
                newX = 0;
            roamTo = new Vector2(selfPosition.x + newX, selfPosition.y);
            reached = false;

        }
        // move towards target position unless reached
        if (roamTo.x + 0.1 < selfPosition.x
            || roamTo.x - 0.1 > selfPosition.x)
        {
            if (roamTo.x > selfPosition.x)
                MoveRight();
            else
                MoveLeft();
        }
        else
        {
            reached = true;
            this.Idle();
        }
    }


    void Retreat()
    {
		this.retreating = true;
        Vector2 dir = selfPosition - targetPosition;
        if (dir.x > 0)
            MoveRight();
        else
            MoveLeft();

    }

    void Aggressive()
    {

        Vector2 enemyDirection = this.targetPosition - selfPosition;

        float enemyDistance = enemyDirection.magnitude;

        if (this.canAttack && enemyDistance < this.attackRange)
        {
            if ((enemyDirection.x > 0 && this.facingRight) || (enemyDirection.x < 0 && !this.facingRight))
            {
                StartCoroutine(this.Attack());
            }
            else
            {
                if (enemyDirection.x > 0)
                    this.MoveRight();
                else
                    this.MoveLeft();
            }

        }
        else
        {
            if (enemyDirection.x > this.attackRange * sADP)
            {
                this.MoveRight();
            }
            else if (enemyDirection.x < -this.attackRange * sADP)
            {
                this.MoveLeft();
            }
            else
            {
                this.Idle();
            }
        }
    }

    IEnumerator Attack()
    {
        float animStartToDamage = 1f;
        float animDuration = 1.3f;

        this.attacked = true;
        anim.SetTrigger("attack");
		print ("attacking");
        float originSpeed = this.movementSpeed;
        this.movementSpeed = 1f;
        yield return new WaitForSeconds(animStartToDamage);
		if (!this.beingAttacked && !this.retreating)
        {
			print ("Attacking!");
            DefaultAttack();
        }
        this.beingAttacked = false;
        yield return new WaitForSeconds(animDuration - animStartToDamage);
        this.movementSpeed = originSpeed;
        yield return new WaitForSeconds(this.attackInterval - animDuration);
        this.attacked = false;

    }

	public void DefaultAttack(){

		if (!this.beingAttacked && !this.retreating) {
			print ("give damage");
			foreach (MainCharacter player in minionWeaponCollider.enemyList) {
				print ("one player!");
				player.BeAttacked (this.attack);
				player.beingAttacked = true;
			}
		}

	}


    void CooldownChecker()
    {

        this.canAttack = !this.attacked;

    }




    public void MinionFlipping()
    {
        Vector3 localScale = GetComponent<Transform>().localScale;
        if (!facingRight)
            localScale.x = Mathf.Abs(localScale.x);
        else
            localScale.x = -1f * Mathf.Abs(localScale.x);
        GetComponent<Transform>().localScale = localScale;
    }


    // block for delayed removal of minions


    IEnumerator RemoveMinion()
    {	
		Destroy (GetComponent<Collider2D> ());
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}