using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;

#endif

public class Utility_Mesh : MonoBehaviour
{

#if UNITY_EDITOR 
    public string newMeshName;

    [ContextMenu("Combine Meshes and Materials")]
    void TestCombine()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        //Get unique materials
        List<Material> materialsList = new List<Material>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (!materialsList.Contains(renderer.sharedMaterial))
            {
                materialsList.Add(renderer.sharedMaterial);
            }
        }
        Material[] materials = materialsList.ToArray();
        List<CombineInstance> combineInstancesList = new List<CombineInstance>();
        for (int i = 0; i < materials.Length; i++)
        {
            List<CombineInstance> subMeshCombineInstances = new List<CombineInstance>();
            for (int j = 0; j < meshFilters.Length; j++)
            {
                if (meshRenderers[j].sharedMaterial == materials[i])
                {
                    CombineInstance combineInstance = new CombineInstance
                    {
                        mesh = meshFilters[j].sharedMesh,
                        transform = meshFilters[j].transform.localToWorldMatrix
                    };
                    subMeshCombineInstances.Add(combineInstance); meshFilters[j].gameObject.SetActive(false);
                }
            }
            if (subMeshCombineInstances.Count > 0)
            {
                Mesh subMesh = new Mesh();
                subMesh.CombineMeshes(subMeshCombineInstances.ToArray(), true, true);
                CombineInstance combinedSubMeshInstance = new CombineInstance { mesh = subMesh, transform = Matrix4x4.identity };
                combineInstancesList.Add(combinedSubMeshInstance);
            }
        }
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        //Set to 32 - bit index format
        combinedMesh.CombineMeshes(combineInstancesList.ToArray(), false, false);

        //string assetPath = "Assets/CombinedMeshes/" + gameObject.name + ".asset"; 
        //AssetDatabase.CreateAsset(combinedMesh, assetPath); 
        //AssetDatabase.SaveAssets();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = combinedMesh;
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterials = materials; gameObject.SetActive(true);
    }
#endif
#if UNITY_EDITOR
    [ContextMenu("Center Vertices")]

    void CenterMeshVertices()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter != null)
        {

            Mesh Original = filter.sharedMesh;
            Mesh mesh = Instantiate(Original);
            Vector3[] vertices = mesh.vertices;
            Vector3 center = Vector3.zero;
            for (int i = 0; i < vertices.Length; i++)
            {
                center += vertices[i];
            }
            center /= vertices.Length;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] -= center;
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            filter.mesh = mesh;

            string path = "Assets/Models/" + gameObject.name + ".asset";
            Debug.Log("Saving mesh to: " + path);
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

#endif
//#if UNITY_EDITOR
//    [ContextMenu("Simplify_Mesh")]
//    void SimplifyMesh()
//    {
//        MeshFilter filter = GetComponent<MeshFilter>();
//        if (filter != null)
//        {
//            DisplayMeshDate();
//            Mesh mesh = filter.sharedMesh;         
//            mesh.Optimize();

//            DisplayMeshDate();
//        }
//    }
//#endif
#if UNITY_EDITOR
    [CustomEditor(typeof(Utility_Mesh))]
    [ContextMenu("Log Mesh Data")]
    void DisplayMeshDate()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter != null)
        {
            Vector3[] vertices = filter.sharedMesh.vertices;
            Debug.Log("Vertices: " + vertices.Length);
        }

    }
#endif
}
