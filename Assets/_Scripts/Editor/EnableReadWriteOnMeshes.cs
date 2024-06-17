using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnableReadWriteOnMeshes : MonoBehaviour
{
    [MenuItem("Tools/Enable Read/Write On All Meshes")]
    private static void EnableReadWrite()
    {
        string[] guids = AssetDatabase.FindAssets("t:Mesh");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer != null)
            {
                importer.isReadable = true;
                AssetDatabase.ImportAsset(path);
            }
        }
        Debug.Log("Read/Write Enabled on all meshes.");
    }
}
