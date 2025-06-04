using System.Collections.Generic;
using UnityEngine;

public class TransparentSwitcher  : MonoBehaviour
{
    // 원래 머티리얼 저장용 딕셔너리
    private static Dictionary<MeshRenderer, Material[]> originalMaterials = new Dictionary<MeshRenderer, Material[]>();
    public Material transparentMat;

    //투명한 마테리얼로 전환
    public void SwitchToTransparent(GameObject targetObj)
    {
        if (targetObj == null || transparentMat == null) return;

        MeshRenderer[] renderers = targetObj.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {

            if (!originalMaterials.ContainsKey(renderer))
            {
                originalMaterials[renderer] = renderer.materials;
            }
            int matCount = renderer.materials.Length;
            Material[] newMats = new Material[matCount];

            for (int i = 0; i < matCount; i++)
            {
                newMats[i] = transparentMat;
            }

            renderer.materials = newMats;
        }
    }

    //원래 마테리얼로 전환
    public void SwitchToOrigin(GameObject targetObj)
    {
        if (targetObj == null) return;

        MeshRenderer[] renderers = targetObj.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mr in renderers)
        {
            if (originalMaterials.ContainsKey(mr))
            {
                mr.materials = originalMaterials[mr];
                originalMaterials.Remove(mr);
            }
        }
    }
}