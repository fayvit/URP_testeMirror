using System.IO;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)

public static class MAST_Interface_Data_Manager
{
    public static MAST_Interface_ScriptableObject state;
    
    // ---------------------------------------------------------------------------
    // Called during Interface OnEnable
    // ---------------------------------------------------------------------------
    public static void Initialize()
    {
        Get_Reference_To_Scriptable_Object();
    }
    
    // ---------------------------------------------------------------------------
    // Get or create the state scriptable object
    // ---------------------------------------------------------------------------
    private static void Get_Reference_To_Scriptable_Object()
    {
        // Get MAST Core path
        string statePath = MAST_Asset_Loader.GetMASTRootFolder() + "/Etc/MAST_Interface_State.asset";
        
        // Load the MAST Core scriptable object
        state = AssetDatabase.LoadAssetAtPath<MAST_Interface_ScriptableObject>(statePath);
        
        // If the Core scriptable object isn't found, create a new default one
        if (state == null)
        {
            state = ScriptableObject.CreateInstance<MAST_Interface_ScriptableObject>();
            AssetDatabase.CreateAsset(state, statePath);
        }
    }
    
    // ---------------------------------------------------------------------------
    // Save preferences to state scriptable object
    // ---------------------------------------------------------------------------
    
    public static void Save_Palette_Items(bool forceSave = false)
    {
        // Get or create a scriptable object to store the interface state data
        Get_Reference_To_Scriptable_Object();
        
        state.selectedPrefabPaletteFolderIndex = MAST_Prefab_Palette.selectedFolderIndex;
        
        Save_Changes_To_Disk();
    }
    
    public static void Restore_Palette_Items()
    {
        // Get or create a scriptable object to store the interface state data
        Get_Reference_To_Scriptable_Object();
        
        MAST_Prefab_Palette.LoadPrefabs(state.prefabPath, state.selectedPrefabPaletteFolderIndex);
        MAST_Material_Palette.LoadMaterials(state.materialPath, state.selectedMaterialPaletteFolderIndex);
        
    }
    
    // ---------------------------------------------------------------------------
    // Save grid state preferences to state scriptable object
    // ---------------------------------------------------------------------------
    public static void Save_Interface_State()
    {
        // Get or create a scriptable object to store the interface state data
        Get_Reference_To_Scriptable_Object();
        
        // Save grid exists state
        state.gridExists = MAST_Grid_Manager.gridExists;
        
        // Save selected draw tool and palette
        state.selectedBuildToolIndex = MAST_Settings.gui.toolbar.selectedDrawToolIndex;
        state.selectedPrefabIndex = MAST_Prefab_Palette.selectedItemIndex;
        
        // Save state changes to disk
        Save_Changes_To_Disk();
    }
    
    // ---------------------------------------------------------------------------
    // Load grid state preferences from state scriptable object
    // ---------------------------------------------------------------------------
    public static void Load_Interface_State()
    {
        // Get or scriptable object to store the interface state data
        Get_Reference_To_Scriptable_Object();
        
        // -----------------------------------------------
        // If there is no saved scriptable object
        // -----------------------------------------------
        if (state == null)
        {
            // Set grid exists to false
            MAST_Grid_Manager.gridExists = false;
            
            // Load selected draw tool and palette
            MAST_Settings.gui.toolbar.selectedDrawToolIndex = -1;
            MAST_Prefab_Palette.selectedItemIndex = -1;
        }
        
        // -----------------------------------------------
        // If there is a scriptable object
        // -----------------------------------------------
        else
        {
            // Load grid exists state
            MAST_Grid_Manager.gridExists = state.gridExists;
            
            // Load selected draw tool and palette
            MAST_Settings.gui.toolbar.selectedDrawToolIndex = state.selectedBuildToolIndex;
            MAST_Prefab_Palette.selectedItemIndex = state.selectedPrefabIndex;
        }
    }
    
    public static void Save_Changes_To_Disk()
    {
        // Save scriptable object changes
        // AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
        EditorUtility.SetDirty(state);
    }
}

#endif