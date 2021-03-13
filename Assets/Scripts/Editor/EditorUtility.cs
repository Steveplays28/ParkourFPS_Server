using UnityEditor;
using UnityEngine;

public static class EditorUtility
{
    [MenuItem("Tools/Editor Utility/Reset material of every mesh in current scene", false, 0)]
    public static void ResetAllMeshMaterialInScene()
    {
        Material _defaultMaterial = null;
        int _amount = 0;

        foreach (Material _material in Resources.FindObjectsOfTypeAll<Material>())
        {
            if (_material.name == "ProBuilderDefault")
            {
                _defaultMaterial = _material;
            }
        }
        if (_defaultMaterial == null)
        {
            Debug.Log("Could not find the Pro Builder Default material!");
            return;
        }

        foreach (MeshRenderer _meshRenderer in Object.FindObjectsOfType<MeshRenderer>(true))
        {
            _meshRenderer.sharedMaterial = _defaultMaterial;
            _amount++;
        }

        Debug.Log($"Reset {_amount} mesh's material in current scene!");
    }
}
