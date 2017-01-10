using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class CoolDown
{
    public string name;
    public float coolDownLength;
    public float currentCoolDown;
    public Image icon;
}

public class CoolDownController : MonoBehaviour {

    public List<CoolDown> coolDowns;
	// Use this for initialization
	

	// Update is called once per frame
	void Update () {
	    foreach(CoolDown c in coolDowns)
        {
            if (c.currentCoolDown < c.coolDownLength)
            {
                c.currentCoolDown += Time.deltaTime;
                c.icon.fillAmount = c.currentCoolDown / c.coolDownLength;
            }
        }
	}
}
