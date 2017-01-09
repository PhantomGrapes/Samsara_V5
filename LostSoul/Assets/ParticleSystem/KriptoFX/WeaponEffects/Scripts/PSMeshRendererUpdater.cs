using UnityEngine;
using System.Collections;

public class PSMeshRendererUpdater : MonoBehaviour
{
    public GameObject MeshObject;

    public void UpdateMeshEffect()
    {
        UpdatePSMesh(MeshObject);
        AddMaterialToMesh(MeshObject);
    }

    public void UpdateMeshEffect(GameObject go)
    {
        UpdatePSMesh(go);
        AddMaterialToMesh(go);
    }

    private void UpdatePSMesh(GameObject go)
    {
        var ps = GetComponentsInChildren<ParticleSystem>();
        var meshRend = go.GetComponentInChildren<MeshRenderer>();
        var skinMeshRend = go.GetComponentInChildren<SkinnedMeshRenderer>();
        foreach (var particleSys in ps) {
            particleSys.transform.gameObject.SetActive(false);
            var sh = particleSys.shape;
            if (sh.enabled) {
                if (meshRend!=null) {
                    sh.shapeType = ParticleSystemShapeType.MeshRenderer;
                    sh.meshRenderer = meshRend;
                }
                if (skinMeshRend!=null) {
                    sh.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
                    sh.skinnedMeshRenderer = skinMeshRend;
                }
            }
            particleSys.transform.gameObject.SetActive(true);
        }
    }

    private void AddMaterialToMesh(GameObject go)
    {
        var meshRend = go.GetComponentInChildren<MeshRenderer>(); 
        var skinMeshRend = go.GetComponentInChildren<SkinnedMeshRenderer>();
        var meshMatEffects = GetComponentsInChildren<WFX_MeshMaterialEffect>();
        foreach (var meshMatEff in meshMatEffects) {
            if (meshRend!=null) {
                meshRend.sharedMaterials = AddToSharedMaterial(meshRend.sharedMaterials, meshMatEff, meshMatEff.IsFirstMaterial);
            }
            if (skinMeshRend!=null) {
                skinMeshRend.sharedMaterials = AddToSharedMaterial(skinMeshRend.sharedMaterials, meshMatEff, meshMatEff.IsFirstMaterial);
            }
        }
    }

    Material[] AddToSharedMaterial(Material[] sharedMaterials, WFX_MeshMaterialEffect meshMatEff, bool isFirst)
    {
        meshMatEff.Material = new Material(meshMatEff.Material);
        meshMatEff.Material.name = meshMatEff.Material.name + " (Instance)";
        if (isFirst) {
            return new[] { meshMatEff.Material };
        }
        var matsCopyNew = new Material[sharedMaterials.Length + 1];
        sharedMaterials.CopyTo(matsCopyNew, 0);

        matsCopyNew[matsCopyNew.Length - 1] = meshMatEff.Material;
        return matsCopyNew;
    }
}