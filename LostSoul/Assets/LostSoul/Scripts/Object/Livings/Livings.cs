﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Livings : MonoBehaviour
{


    public bool alive = true;
    public int lvl = 1;
    public int maxHp = 100;
    public float hp;
    public float attack;
    public float armor = 1;
    public float physicalResistance;
    public float magicalResistance = 0;
    public float movementSpeed;

    public float attackSpeed = 1f;
    public float attackInterval;
    public bool melee = true;
    public float attackRange;

    public double critChance = 0.01;
    public double evasion = 0.01;
    public float hpReg = 1;
    public bool facingRight = false;
    public List<Buff> buffs;
    public CharacterController controller;
    public float jumpForce;
    public bool attacked = false;
    public bool beingAttacked = false;
    public bool canAttack = true;





    public void Idle()
    {

        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);

    }

    public void Move(Vector2 moveDirection, bool isRoaming = false)
    {

        if (isRoaming)
        {

            GetComponent<Rigidbody2D>().velocity = moveDirection / 2;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = moveDirection;
        }
        //		transform.Translate (this.movementSpeed *Time.deltaTime * moveDirection);

    }

    public void MoveLeft()
    {
        Vector2 direction = new Vector2(-this.movementSpeed, GetComponent<Rigidbody2D>().velocity.y);

        this.Move(direction);
        facingRight = false;
    }

    public void MoveRight()
    {

        Vector2 direction = new Vector2(this.movementSpeed, GetComponent<Rigidbody2D>().velocity.y);

        this.Move(direction);
        facingRight = true;
    }

    public void MoveUp()
    {

        Vector2 direction = new Vector2(GetComponent<Rigidbody2D>().velocity.x, this.movementSpeed);
        this.Move(direction);
    }

    public void MoveDown()
    {
        Vector2 direction = new Vector2(GetComponent<Rigidbody2D>().velocity.x, -this.movementSpeed);

        this.Move(direction);
    }

    public void Jump()
    {
        Vector2 jumpForce = new Vector2(0, this.jumpForce);
        GetComponent<Rigidbody2D>().AddForce(jumpForce);
    }


    public void Die()
    {
        alive = false;

    }

	public void Deactivate(){
		Destroy (this.gameObject);
	}

    // This should be overriden in Maincharacter as we do implicitely chooce target
//    public void DefaultAttack(MainCharacter target)
//    {
//        // direction sensitive
//        Vector2 enemyDirection =
//            target.GetComponent<Rigidbody2D>().position
//            - GetComponent<Rigidbody2D>().position;
//        float enemyDistance = enemyDirection.magnitude;
//
//
//        if ((enemyDirection.x > 0 && facingRight) || (enemyDirection.x < 0 && !facingRight))
//        {
//            // correct direction case
//            if (enemyDistance < attackRange)
//            {
//                target.BeAttacked(this.attack * (1 - target.physicalResistance));
//            }
//        }
//    }


    //new function to change the direction of face according to facingRight
    public void Flipping()
    {
        Vector3 localScale = GetComponent<Transform>().localScale;
        if (facingRight)
            localScale.x = Mathf.Abs(localScale.x);
        else
            localScale.x = -1f * Mathf.Abs(localScale.x);
        GetComponent<Transform>().localScale = localScale;
    }

}