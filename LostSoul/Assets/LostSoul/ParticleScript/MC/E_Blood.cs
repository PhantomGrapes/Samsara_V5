using UnityEngine;
using System.Collections;

public class E_Blood : MonoBehaviour {

    public GameObject particleObject;
    public ParticleSystem particle;
    public GameObject parent;
    void E_BloodParticle()
    {
        particle.GetComponent<Transform>().localScale = new Vector3(parent.GetComponent<Transform>().localScale.x * (-1), parent.GetComponent<Transform>().localScale.y, parent.GetComponent<Transform>().localScale.z * (-1));
        Destroy(Instantiate(particleObject,parent.GetComponent<Transform>().position, parent.GetComponent<Transform>().rotation), 0.5f);
    }
}
