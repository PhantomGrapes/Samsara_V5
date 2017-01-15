using UnityEngine;
using System.Collections;

public class MeshSpriteEffect : MonoBehaviour {

    public GameObject MeshObject;
    
    void Start()
    {
        UpdatePSMesh(MeshObject);
    }
    private void UpdatePSMesh(GameObject go)
    {
        var ps = GetComponentsInChildren<ParticleSystem>();
        var meshRend = go.GetComponentInChildren<MeshRenderer>();
        var skinMeshRend = go.GetComponentInChildren<SkinnedMeshRenderer>();
        foreach (var particleSys in ps)
        {
            particleSys.transform.gameObject.SetActive(false);
            var sh = particleSys.shape;
            if (sh.enabled)
            {
                if (meshRend != null)
                {
                    sh.shapeType = ParticleSystemShapeType.MeshRenderer;
                    sh.meshRenderer = meshRend;
                }
                if (skinMeshRend != null)
                {
                    sh.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
                    sh.skinnedMeshRenderer = skinMeshRend;
                }
            }
            particleSys.transform.gameObject.SetActive(true);
        }
    }
}
