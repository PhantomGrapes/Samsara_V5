using UnityEngine;
using System.Collections;

public class E_AttackSlash : MonoBehaviour {

    public GameObject particleObject;
    public ParticleSystem particle;
    public GameObject parent;
    void E_AttackHS01Slash()
    {
        particle.GetComponent<Transform>().localScale = new Vector3(parent.GetComponent<Transform>().localScale.x * (-1), parent.GetComponent<Transform>().localScale.y, parent.GetComponent<Transform>().localScale.z * (-1));
        Destroy(Instantiate(particleObject, new Vector3(parent.GetComponent<Transform>().position.x, parent.GetComponent<Transform>().position.y, parent.GetComponent<Transform>().position.z - 0.1f), parent.GetComponent<Transform>().rotation), 0.5f);
    }

    void E_AttackHS02Slash()
    {
        particle.GetComponent<Transform>().localScale = new Vector3(parent.GetComponent<Transform>().localScale.x * (-1), parent.GetComponent<Transform>().localScale.y, parent.GetComponent<Transform>().localScale.z);
        Destroy(Instantiate(particleObject, new Vector3(parent.GetComponent<Transform>().position.x, parent.GetComponent<Transform>().position.y, parent.GetComponent<Transform>().position.z - 0.1f), parent.GetComponent<Transform>().rotation), 0.5f);
    }

    void E_AttackHS03Slash()
    {
        particle.startSize = 8;
        particle.GetComponent<Transform>().localScale = new Vector3(parent.GetComponent<Transform>().localScale.x * (-1), parent.GetComponent<Transform>().localScale.y, parent.GetComponent<Transform>().localScale.z * (-1));
        Destroy(Instantiate(particleObject, new Vector3(parent.GetComponent<Transform>().position.x, parent.GetComponent<Transform>().position.y, parent.GetComponent<Transform>().position.z - 0.1f), parent.GetComponent<Transform>().rotation), 0.5f);
    }
}
