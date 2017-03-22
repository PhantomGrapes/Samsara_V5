using UnityEngine;
using System.Collections;

public class Monster : Livings {


	public int expGivenUponDeath;
	public MainCharacter target;

	public Animator anim;

	public float alertDistance = 35;

	public bool flipLock = false;
	public bool timeLock = false;
	public bool moveLock = false;
	public bool attackLock = false;

	public void BeAttacked (float damageDealt){
		this.hp -= damageDealt;
		anim.SetTrigger ("beAttacked");
	}

	public void FlipLock(){
		flipLock = true;
	}

	public void FlipUnlock(){
		flipLock = false;
	}

	public void MoveLock(){
		moveLock = true;
	}

	public void MoveUnlock(){
		moveLock = false;
	}

	public void AttackLock(){
		attackLock = true;
	}

	public void AttackUnlock(){
		attackLock = false;
	}

}
