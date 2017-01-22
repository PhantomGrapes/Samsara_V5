using UnityEngine;
using System.Collections;



public class Monster : Livings {


	public int expGivenUponDeath;
	public MainCharacter target;

	public Animator anim;

	public float alertDistance = 35;

	public bool flipLock = false;

	public bool timeLock = false;

	public void beAttacked (float damageDealt){
		this.hp -= damageDealt;
		anim.SetTrigger ("beAttacked");
	}

}
