using UnityEditor;
using UnityEngine;

public class EditorUtility
{
    [MenuItem("Tools/Editor Utility/Reset material of every mesh in current scene", false, 0)]
    public static void ResetAllMeshMaterialInScene()
    {
        int _amount = 0;
        Material _material = new Material(Shader.Find("Standard"));

        foreach (MeshRenderer _meshRenderer in Object.FindObjectsOfType<MeshRenderer>(true))
        {
            _meshRenderer.material = _material;
            _amount++;
        }

        Debug.Log($"Reset {_amount} mesh's material in current scene!");
    }
}
