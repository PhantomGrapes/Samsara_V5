using UnityEngine;
using System.Collections;

public class UpdateEffect : MonoBehaviour {

    public GameObject myMesh;

	void Start () {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var sh = ps.shape;
        sh.enabled = true;
        sh.shapeType = ParticleSystemShapeType.Mesh;
        sh.mesh = myMesh;
	}

}
