using UnityEngine;
using System.Collections;

public class E_SkillHSSlash : MonoBehaviour {

    public GameObject particleObject;
    public ParticleSystem particle;
    public GameObject parent;
    void E_SkillHSSlashSpwan()
    {
        particle.GetComponent<Transform>().localScale = new Vector3(parent.GetComponent<Transform>().localScale.x * (-1), parent.GetComponent<Transform>().localScale.y, parent.GetComponent<Transform>().localScale.z * (-1));
        Destroy(Instantiate(particleObject, new Vector3(parent.GetComponent<Transform>().position.x, parent.GetComponent<Transform>().position.y, parent.GetComponent<Transform>().position.z - 0.1f), parent.GetComponent<Transform>().rotation), 0.5f);
    }
}
