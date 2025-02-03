using System.Collections.Generic;
using UnityEngine;

public class EditorObject: MonoBehaviour {

    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    private List<Material[]> originalMaterials = new List<Material[]>(); // Store the original materials for each renderer

    private void Awake() {
        SetupRendererList();
        CacheOriginalMaterials();
    }

    public void SetMaterial(Material material) {
        foreach (var renderer in renderers) {
            renderer.material = material;
        }
    }

    public void RestoreOriginalMaterials() {
        for (int i = 0; i < renderers.Count; i++) {
            renderers[i].materials = originalMaterials[i];
        }
    }

    private void SetupRendererList() {
        renderers.Clear();
        renderers.AddRange(GetComponentsInChildren<MeshRenderer>());
    }

    private void CacheOriginalMaterials() {
        originalMaterials.Clear();
        foreach (var renderer in renderers) {
            originalMaterials.Add(renderer.materials);
        }
    }

}
