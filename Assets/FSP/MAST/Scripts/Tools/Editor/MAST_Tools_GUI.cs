using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

public static class MAST_Tools_GUI
{  
    [SerializeField] private static MAST_MergeMeshes MergeMeshesClass;
    private static MAST_MergeMeshes MergeMeshes
    {
        get
        {
            // Initialize MergeMeshes Class if needed and return MergeMeshesClass
            if(MergeMeshesClass == null)
                MergeMeshesClass = new MAST_MergeMeshes();
            
            return MergeMeshesClass;
        }
    }
    
    [SerializeField] private static MAST_Prefab_Creator_Window PrefabCreator;
    
    [SerializeField] private static MAST_Assembly_Creator_Window AssemblyCreator;
    
    [SerializeField] private static Vector2 scrollPos;
    
    // ---------------------------------------------------------------------------
    #region Preferences Interface
    // ---------------------------------------------------------------------------
    public static void DisplayToolsGUI()
    {
        GUILayout.BeginVertical("MAST Toolbar BG");  // Begin entire window vertical layout
        
        // Verical scroll view for palette items
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        // ------------------------------------
        // Open PrefabCreator Window Button
        // ------------------------------------
        GUILayout.Space(5f);
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Generate Prefabs from your own models.  Substitute and consolidate materials used during the process.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Open Prefab Creator window", "Open Prefab Creator window")))
        {
            // If PrefabCreator window is closed, show and initialize it
            if (PrefabCreator == null)
            {
                PrefabCreator = (MAST_Prefab_Creator_Window)EditorWindow.GetWindow(
                    typeof(MAST_Prefab_Creator_Window),
                    false, "MAST Prefab Creator");
                
                
                PrefabCreator.minSize = new Vector2(800, 250);
            }
            
            // If PrefabCreator window is open, close it
            else
            {
                EditorWindow.GetWindow(typeof(MAST_Prefab_Creator_Window)).Close();
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Add MAST Script to Prefabs
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("This will add a MAST script to each prefab.  The script is used to describe the type of object to the MAST editor.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Add MAST Script to Prefabs",
            "Create Prefabs from all models in the selected folder.")))
        {
            // Show choose folder dialog
            string chosenPath = EditorUtility.OpenFolderPanel("Choose the Folder that Contains your Prefabs",
                MAST_Interface_Data_Manager.state.prefabPath, "");
            
            // If a folder was chosen "Cancel was not clicked"
            if (chosenPath != "")
            {
                // Save the path the user chose
                MAST_Interface_Data_Manager.state.prefabPath = chosenPath;
                MAST_Interface_Data_Manager.Save_Changes_To_Disk();
                
                // Convert to project local path "Assets/..."
                string assetPath = chosenPath.Replace(Application.dataPath, "Assets");
                
                // Loop through each Prefab in folder
                foreach (GameObject prefab in MAST_Asset_Loader.GetPrefabsInFolder(assetPath))
                {
                    // Add MAST Prefab script if not already added
                    if (!prefab.GetComponent<MAST_Prefab_Component>())
                        prefab.AddComponent<MAST_Prefab_Component>();
                }
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------------
        // Create Assembly Button
        // ----------------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Create a Prefab (Assembly) from the current scene selection.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Open Create Assembly Window",
            "Create a Prefab from the selection.  The final prefab will be moved to and anchored at 0,0,0")))
        {
            // If SelectionToPrefab window is closed, show and initialize it
            if (AssemblyCreator == null)
            {
                AssemblyCreator = (MAST_Assembly_Creator_Window)EditorWindow.GetWindow(
                    typeof(MAST_Assembly_Creator_Window),
                    false, "MAST Assembly Creator");
                
                
                AssemblyCreator.minSize = new Vector2(400, 400);
            }
            
            // If SelectionToPrefab window is open, close it
            else
            {
                EditorWindow.GetWindow(typeof(MAST_Assembly_Creator_Window)).Close();
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Remove MAST Components Button
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Remove all MAST Components that were attached to the children of the selected GameObject during placement.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Remove MAST Components",
            "Remove any MAST Component code attached to gameobjects during placement")))
        {
            if (EditorUtility.DisplayDialog("Are you sure?",
                "This will remove all MAST components attached to '" + Selection.activeGameObject.name + "'",
                "Remove MAST Components", "Cancel"))
            {
                // Loop through all top-level children of targetParent
                foreach (MAST_Prefab_Component prefabComponent
                    in Selection.activeGameObject.transform.GetComponentsInChildren<MAST_Prefab_Component>())
                {
                    // Remove the SMACK_Prefab_Component script
                    GameObject.DestroyImmediate(prefabComponent);
                }
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Merge Meshes by Material Button
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Merge all meshes in the selected GameObject, and place them in a new GameObject.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Merge Meshes",
            "Merge all meshes by material name, resulting in one mesh for each material")))
        {
            if (EditorUtility.DisplayDialog("Are you sure?",
                "This will combine all meshes in '" + Selection.activeGameObject.name +
                "' and save them to a new GameObject.  The original GameObject will not be affected.",
                "Merge Meshes", "Cancel"))
            {
                
                GameObject targetParent = MergeMeshes.MergeMeshes(Selection.activeGameObject);
                targetParent.name = Selection.activeGameObject.name + "_Merged";
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        EditorGUILayout.EndScrollView();
        
        GUILayout.EndVertical();
    }
    #endregion
    // ---------------------------------------------------------------------------
}

#endif