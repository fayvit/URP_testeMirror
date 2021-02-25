using System.Collections.Generic;
using UnityEngine;

#if (UNITY_EDITOR)

public class MAST_MergeMeshes
{
    
    public GameObject MergeMeshes(GameObject source)
    {
        // Instantiate a new copy of the source GameObject so the original is not changed
        GameObject sourceParent = GameObject.Instantiate(source);
        
        // ---------------------------------------------------------------------------
        // Remove GameObjects to Exclude from the Merge and move them back later
        // ---------------------------------------------------------------------------
        
        // Create a GameObject to be the Parent of all models excluded from the Combine operation
        GameObject excludeFromMergeParent = new GameObject("Not Merged");
        
        // Get all child Transforms in the Source GameObject
        Transform[] sourceTransforms = sourceParent.GetComponentsInChildren<Transform>();
        
        // Move any GameObjects not included in the merge to a separate Parent GameObject
        for (int i = sourceTransforms.Length - 1; i >= 0; i--)
        {
            if (!IncludeInMerge(sourceTransforms[i].gameObject))
                sourceTransforms[i].parent = excludeFromMergeParent.transform;
        }
        // ---------------------------------------------------------------------------
        
        // Get all MeshFilters and MeshRenderers from the source GameObject's children
        MeshFilter[] sourceMeshFilters = sourceParent.GetComponentsInChildren<MeshFilter>();
        MeshRenderer[] sourceMeshRenderers = sourceParent.GetComponentsInChildren<MeshRenderer>();
        
        // Create a List containing all unique Materials in the GameObjects
        List<Material> uniqueMats = GetUniqueMaterialListFromMeshRendererArray(sourceMeshRenderers);
        
        // Create a GameObject with all SubMeshes combined into a single Mesh
        GameObject finalGameObject = CombineMeshesWithSubMeshes(uniqueMats, sourceMeshFilters, sourceMeshRenderers);
        
        // Name finalGameObject and make it the child of an empty parent
        GameObject finalGameObjectParent = new GameObject(sourceParent.name + " Merged");
        finalGameObject.transform.parent = finalGameObjectParent.transform;
        
        // If the Unmerged GameObject holder is isn't empty, make it a child of the empty parent
        if (excludeFromMergeParent.transform.childCount > 0)
            excludeFromMergeParent.transform.parent = finalGameObjectParent.transform;
        
        // If the Unmerged GameObject holder is empty, delete it
        else
            GameObject.DestroyImmediate(excludeFromMergeParent);
        
        // Delete unneeded GameObjects
        GameObject.DestroyImmediate(sourceParent);
        
        // Return the complete GameObject
        return finalGameObjectParent;
    }
    
    // Used by [MergeMeshes] to determine if a GameObject should be included in the Combine operation
    private bool IncludeInMerge(GameObject go)
    {
        // If prefab is not supposed to be included in the merge, don't include its material name
        MAST_Prefab_Component prefabComponent = go.GetComponent<MAST_Prefab_Component>();
        if (prefabComponent != null)
            return prefabComponent.includeInMerge;
        
        // If no MAST prefab component was attached, include it in the merge
        return true;
    }
    
    // ------------------------------------------------------------------------------------------------
    // Get Unique Material List from MeshRenderer Array
    // ------------------------------------------------------------------------------------------------
    public List<Material> GetUniqueMaterialListFromMeshRendererArray(MeshRenderer[] sourceMeshRenderers)
    {
        
        // Create a List containing all unique Materials in the GameObjects
        List<Material> uniqueMats = new List<Material>();
        
        bool foundMat;
        
        // Loop through each MeshRenderer
        for (int i = 0; i < sourceMeshRenderers.Length; i++)
        {
            // Loop through each MeshRenderer's SharedMaterials
            for (int j = 0; j < sourceMeshRenderers[i].sharedMaterials.Length; j++)
            {
                // Set Found Material flag to "False"
                foundMat = false;
                
                // Loop through all Materials in the Unique Material list
                for (int k = 0; k < uniqueMats.Count; k++)
                {
                    // If Material was found, set the Found Material flag to "True"
                    if (sourceMeshRenderers[i].sharedMaterials[j].name == uniqueMats[k].name)
                    {
                        foundMat = true;
                    }
                }
                
                // If Found Material flag is "True", add the Material to the Unique Material Array
                if (!foundMat)
                {
                    uniqueMats.Add (sourceMeshRenderers[i].sharedMaterials[j]);
                }
            }
        }
        
        return uniqueMats;
    }
    
    // ----------------------------------------------------------------------------------------------------
    // Combine Meshes that include multiple Materials
    // ----------------------------------------------------------------------------------------------------
    // In:      List<Material>      matsInCombine           Materials to include in the Combine operation.
    //                                                      Any Submesh using a Material not in this List
    //                                                      will be ignored.
    //
    //          MeshFilter[]        sourceMeshFilters       MeshFilters containing all Meshes to combine
    //
    //          MeshRenderer[]      sourceMeshRenderers     MeshRenderers containing all Mesh Materials.
    //                                                      This will line up with [sourceMeshFilters]. 
    // ----------------------------------------------------------------------------------------------------
    // Out:     GameObject containing all Meshes combined
    // ----------------------------------------------------------------------------------------------------
    private class MeshCombine
    {
        public List<CombineInstance> combineList;
    }
    
    public GameObject CombineMeshesWithSubMeshes(List<Material> matsInCombine, MeshFilter[] sourceMeshFilters, MeshRenderer[] sourceMeshRenderers)
    {
        // ---------------------------------------------------------------------------
        // Extract Meshes into Separate CombineInstances based on Material
        // ---------------------------------------------------------------------------
        
        // Create a MeshCombine Class Array the size of the uniqueMats List and initialize
        MeshCombine[] uniqueMatMeshCombine = new MeshCombine[matsInCombine.Count];
        for (int i = 0; i < matsInCombine.Count; i++)
        {
            uniqueMatMeshCombine[i] = new MeshCombine();
            uniqueMatMeshCombine[i].combineList = new List<CombineInstance>();
        }
        
        // Prepare variables
        CombineInstance combineInstance;
        
        // Loop through each MeshRenderer in sourceMeshRenderers
        for (int i = 0; i < sourceMeshRenderers.Length; i++)
        {
            // Loop through each Material in each MeshRenderer
            for (int j = 0; j < sourceMeshRenderers[i].sharedMaterials.Length; j++)
            {
                // Loop through each Material in the uniqueMats List
                for (int k = 0; k < matsInCombine.Count; k++)
                {
                    // If this Material matches the Material in the uniqueMats List
                    if (sourceMeshRenderers[i].sharedMaterials[j] == matsInCombine[k])
                    {
                        // Initialize a Combine Instance
                        combineInstance = new CombineInstance();
                        
                        // Copy this mesh to the Combine Instance
                        combineInstance.mesh = sourceMeshFilters[i].sharedMesh;
                        
                        // Set it to only include the Mesh with the specified material
                        combineInstance.subMeshIndex = j;
                        
                        // Transform to world matrix
                        combineInstance.transform = sourceMeshFilters[i].transform.localToWorldMatrix;
                        
                        // Add this CombineInstance to the appropriate CombineInstance List (by Material)
                        uniqueMatMeshCombine[k].combineList.Add(combineInstance);
                    }
                }
            }
        }
        
        // ---------------------------------------------------------------------------
        // Combine all Mesh Instances into a single GameObject
        // ---------------------------------------------------------------------------
        
        // Disable all Source GameObjects
        for (int i = 0; i < sourceMeshFilters.Length; i++)
        {
            sourceMeshFilters[i].gameObject.SetActive(false);
        }
        
        // Create the final GameObject that will hold all the other GameObjects
        GameObject finalGameObject = new GameObject("Merged Meshes");
        
        // Create a new GameObject Array the size of the All Materials List
        GameObject[] singleMatGameObject = new GameObject[matsInCombine.Count];
        
        // Combine meshes for each singleMatGameObject into one mesh
        CombineInstance[] finalCombineInstance = new CombineInstance[matsInCombine.Count];
        
        // Enable all Source GameObjects
        for (int i = 0; i < sourceMeshFilters.Length; i++)
        {
            sourceMeshFilters[i].gameObject.SetActive(true);
        }
        
        // Prepare mesh filter and mesh renderer arrays for the final combine
        MeshFilter[] meshFilter = new MeshFilter[matsInCombine.Count];
        MeshRenderer[] meshRenderer = new MeshRenderer[matsInCombine.Count];
        
        // Loop through each Material in the uniqueMats List
        for (int i = 0; i < matsInCombine.Count; i++)
        {
            // Initialize GameObject
            singleMatGameObject[i] = new GameObject();
            
            singleMatGameObject[i].name = matsInCombine[i].name;
            
            // Add a MeshRender and set the Material
            meshRenderer[i] = singleMatGameObject[i].AddComponent<MeshRenderer>();
            meshRenderer[i].sharedMaterial = matsInCombine[i];
            
            // Add a MeshFilter and add the Combined Meshes with this Material
            meshFilter[i] = singleMatGameObject[i].AddComponent<MeshFilter>();
            meshFilter[i].sharedMesh = new Mesh();
            meshFilter[i].sharedMesh.CombineMeshes(uniqueMatMeshCombine[i].combineList.ToArray());
            
            // Add this Mesh to the final Mesh Combine
            finalCombineInstance[i].mesh = meshFilter[i].sharedMesh;
            finalCombineInstance[i].transform = meshFilter[i].transform.localToWorldMatrix;
            
            // Hide the GameObject
            meshFilter[i].gameObject.SetActive(false);
            
            GameObject.DestroyImmediate(singleMatGameObject[i]);
        }
        
        // Add MeshFilter to final GameObject and Combine all Meshes
        MeshFilter finalMeshFilter = finalGameObject.AddComponent<MeshFilter>();
        finalMeshFilter.sharedMesh = new Mesh();
        finalMeshFilter.sharedMesh.CombineMeshes(finalCombineInstance, false, false);
        
        // Add MeshRenderer to final GameObject Attach Materials
        MeshRenderer finalMeshRenderer = finalGameObject.AddComponent<MeshRenderer>();
        finalMeshRenderer.sharedMaterials = matsInCombine.ToArray();
        
        return finalGameObject;
    }
    
}

#endif