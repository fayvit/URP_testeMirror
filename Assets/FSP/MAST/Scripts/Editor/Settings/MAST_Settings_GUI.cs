using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

public static class MAST_Settings_GUI
{
    // Have preferences changed?
    [SerializeField] private static bool prefChanged = false;
    
    // GameObject reference for target parent of all placed prefeabs
    [SerializeField] private static GameObject targetParent = null;
    
    private static int tab = 0;
    
    [SerializeField] private static Vector2 placementScrollPos = new Vector2();
    [SerializeField] private static Vector2 gridScrollPos = new Vector2();
    [SerializeField] private static Vector2 hotkeyScrollPos = new Vector2();
    
    private static bool placementOffsetFoldout = true;
    private static bool placementRotationFoldout = true;
    private static bool placementRandomizerFoldout = true;
    
    private static bool guiToolbarFoldout = true;
    private static bool guiPaletteFoldout = true;
    private static bool guiGridFoldout = true;
    
    // ---------------------------------------------------------------------------
    // Initialize
    // ---------------------------------------------------------------------------
    public static void Initialize()
    {
        // Get target parent from saved target parent name
        targetParent = GameObject.Find(MAST_Settings.placement.targetParentName);
        
        // If target parent doesn't exist, create it and named it "MAST_Holder"
        if (targetParent == null)
        {
            targetParent = new GameObject();
            targetParent.transform.position = new Vector3(0, 0, 0);
            MAST_Settings.placement.targetParentName = "MAST_Holder";
            targetParent.name = MAST_Settings.placement.targetParentName;
        }
    }
    
// ---------------------------------------------------------------------------
#region Preferences Interface
// ---------------------------------------------------------------------------
    public static void DisplaySettingsGUI(GUISkin guiSkin)
    {
        // If target parent is deleted, create a new one
        if (targetParent == null)
            Initialize();
        
        GUILayout.BeginVertical("MAST Toolbar BG");  // Begin entire window vertical layout
        
        GUILayout.Space(5f);
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        // Start polling for changes
        EditorGUI.BeginChangeCheck ();
        
        string[] tabCaptions = {"Placement", "GUI", "Hotkeys"};
        
        tab = GUILayout.Toolbar (tab, tabCaptions);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        switch (tab) {
            case 0:
                PlacementGUI();
                break;
            case 1:
                GUIGUI(guiSkin);
                break;
            case 2:
                HotkeyGUI();
                break;
        }
        
        GUILayout.EndVertical();
        
        // If changes to UI value ocurred, update
        if (EditorGUI.EndChangeCheck ()) {
            prefChanged = true;
            PreferencesChanged();
        }
    }
#endregion
// ---------------------------------------------------------------------------

// ---------------------------------------------------------------------------
#region Placement Tab GUI
// ---------------------------------------------------------------------------
    private static void PlacementGUI()
    {
        // Verical scroll view for palette items
        placementScrollPos = EditorGUILayout.BeginScrollView(placementScrollPos);
        
        // ----------------------------------
        // Placement Destination
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        targetParent = (GameObject)EditorGUILayout.ObjectField(
            new GUIContent("Placement Destination",
            "Drag a GameObject from the Hierarchy into this field.  It will be used as the parent of new placed models"),
            targetParent, typeof(GameObject), true);
        MAST_Settings.placement.targetParentName = targetParent.name;
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Snap to Grid
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        GUILayout.BeginHorizontal();
        
        MAST_Settings.placement.snapToGrid =
            GUILayout.Toggle(MAST_Settings.placement.snapToGrid, "Snap to Grid");
        
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Offset
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        placementOffsetFoldout = EditorGUILayout.Foldout(placementOffsetFoldout, "Offset Settings");
        if (placementOffsetFoldout) 
        {
            EditorGUILayout.Space();
            
            MAST_Settings.placement.offset.pos =
                EditorGUILayout.Vector3Field("Position Offset", MAST_Settings.placement.offset.pos);
            
            EditorGUILayout.Space();
            
            MAST_Settings.placement.offset.overridePrefab =
                GUILayout.Toggle(MAST_Settings.placement.offset.overridePrefab, "Override Prefab Settings");
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Rotation
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        placementRotationFoldout = EditorGUILayout.Foldout(placementRotationFoldout, "Rotation Settings");
        if (placementRotationFoldout)
        {
            EditorGUILayout.Space();
            
            MAST_Settings.placement.rotation.factor =
                EditorGUILayout.Vector3Field("Rotation Factor", MAST_Settings.placement.rotation.factor);
            
            EditorGUILayout.Space();
            
            MAST_Settings.placement.rotation.overridePrefab =
                GUILayout.Toggle(MAST_Settings.placement.rotation.overridePrefab, "Override Prefab Settings");
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Randomizer
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        placementRandomizerFoldout = EditorGUILayout.Foldout(placementRandomizerFoldout, "Randomizer Settings");
        if (placementRandomizerFoldout) 
        {
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Randomize Rotation (0 = no rotation)", EditorStyles.boldLabel);
            
            MAST_Settings.placement.randomizer.rotate =
                EditorGUILayout.Vector3Field("Rotation Factor", MAST_Settings.placement.randomizer.rotate);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Randomize Scale (1 = no scaling)", EditorStyles.boldLabel);
            
            MAST_Settings.placement.randomizer.scaleLock =
                (MAST_Placement_ScriptableObject.AxisLock)EditorGUILayout.EnumPopup(
                "Lock", MAST_Settings.placement.randomizer.scaleLock);
            
            MAST_Settings.placement.randomizer.scaleMin =
                EditorGUILayout.Vector3Field("Minimum", MAST_Settings.placement.randomizer.scaleMin);
            MAST_Settings.placement.randomizer.scaleMax =
                EditorGUILayout.Vector3Field("Maximum", MAST_Settings.placement.randomizer.scaleMax);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Randomize Position (0 = no offset)", EditorStyles.boldLabel);
            
            MAST_Settings.placement.randomizer.posMin =
                EditorGUILayout.Vector3Field("Minimum", MAST_Settings.placement.randomizer.posMin);
            MAST_Settings.placement.randomizer.posMax =
                EditorGUILayout.Vector3Field("Maximum", MAST_Settings.placement.randomizer.posMax);
            
            EditorGUILayout.Space();
            
            MAST_Settings.placement.randomizer.overridePrefab =
                GUILayout.Toggle(MAST_Settings.placement.randomizer.overridePrefab, "Override Prefab Settings");
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // Get placement settings object path from selected item in project view
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        if (GUILayout.Button(
            new GUIContent("Load Placement Settings from selected file in Project",
            "Use the selected (Placement Settings) file in project")))
        {
            string path = MAST_Asset_Loader.GetPathOfSelectedObjectTypeOf(typeof(MAST_Placement_ScriptableObject));
            
            if (path != "")
            {
                MAST_Settings.core.placementSettingsPath = path;
                MAST_Settings.Load_Placement_Settings();
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        EditorGUILayout.EndScrollView();
    }
#endregion
// ---------------------------------------------------------------------------

// ---------------------------------------------------------------------------
#region GUI Tab GUI
// ---------------------------------------------------------------------------
    private static void GUIGUI(GUISkin guiSkin)
    {
        // Verical scroll view for palette items
        gridScrollPos = EditorGUILayout.BeginScrollView(gridScrollPos);
        
        
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        guiToolbarFoldout = EditorGUILayout.Foldout(guiToolbarFoldout, "Toolbar Settings");
        if (guiToolbarFoldout)
        {
            MAST_Settings.gui.toolbar.position =
                (MAST_GUI_ScriptableObject.ToolbarPos)EditorGUILayout.EnumPopup(
                "Position", MAST_Settings.gui.toolbar.position);
            
            MAST_Settings.gui.toolbar.scale = EditorGUILayout.Slider(
                "Scale", MAST_Settings.gui.toolbar.scale, 0.5f, 1f);
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        guiPaletteFoldout = EditorGUILayout.Foldout(guiPaletteFoldout, "Palette Settings");
        if (guiPaletteFoldout)
        {
            MAST_Settings.gui.palette.bgColor =
                (MAST_GUI_ScriptableObject.PaleteBGColor)EditorGUILayout.EnumPopup(
                "Background Color", MAST_Settings.gui.palette.bgColor);
            
            EditorGUILayout.Space();
            
            MAST_Settings.gui.palette.snapshotCameraPitch =
                Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent(
                "Thumbnail Pitch (0-360)", "Rotation around the Y axis"),
                MAST_Settings.gui.palette.snapshotCameraPitch), 0f, 360f);
            
            MAST_Settings.gui.palette.snapshotCameraYaw =
                Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent(
                "Thumbnail Yaw (0-90)", "Rotation around the X axis"),
                MAST_Settings.gui.palette.snapshotCameraYaw), 0f, 90f);
                
            EditorGUILayout.Space();
            MAST_Settings.gui.palette.overwriteThumbnails = GUILayout.Toggle(MAST_Settings.gui.palette.overwriteThumbnails, "Recreate Thumbnails when Loading from Folder");
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        guiGridFoldout = EditorGUILayout.Foldout(guiGridFoldout, "Grid Settings");
        if (guiGridFoldout)
        {
            EditorGUILayout.LabelField("Grid Dimensions", EditorStyles.boldLabel);
            
            MAST_Settings.gui.grid.xzUnitSize =
                EditorGUILayout.FloatField(new GUIContent(
                "X/Z Unit Size", "Size of an individual grid square for snapping"),
                MAST_Settings.gui.grid.xzUnitSize);
            MAST_Settings.gui.grid.yUnitSize =
                EditorGUILayout.FloatField(new GUIContent(
                "Y Unit Size", "Y step for grid raising/lowering"),
                MAST_Settings.gui.grid.yUnitSize);
            MAST_Settings.gui.grid.cellCount =
                EditorGUILayout.IntField(new GUIContent(
                "Count (Center to Edge)", "Count of squares from center to each edge"),
                MAST_Settings.gui.grid.cellCount);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Grid Cosmetics", EditorStyles.boldLabel);
            
            GUI.skin = null;
            
            MAST_Settings.gui.grid.tintColor =
                EditorGUILayout.ColorField("Tint Color", MAST_Settings.gui.grid.tintColor);
            
            GUI.skin = guiSkin;
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        // Get grid settings object path from selected item in project view
        if (GUILayout.Button(
            new GUIContent("Load GUI Settings from selected file in Project",
            "Use the selected (GUI Settings) file in project")))
        {
            string path = MAST_Asset_Loader.GetPathOfSelectedObjectTypeOf(typeof(MAST_GUI_ScriptableObject));
            
            if (path != "")
            {
                MAST_Settings.core.guiSettingsPath = path;
                MAST_Settings.Load_GUI_Settings();
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        EditorGUILayout.EndScrollView();
    }
#endregion
// ---------------------------------------------------------------------------

// ---------------------------------------------------------------------------
#region Hotkey Tab GUI
// ---------------------------------------------------------------------------
    private static void HotkeyGUI()
    {
        // Verical scroll view for palette items
        hotkeyScrollPos = EditorGUILayout.BeginScrollView(hotkeyScrollPos);
        
        // ----------------------------------
        // Toggle grid On/Off
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Toggle Grid On/Off", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.toggleGridKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.toggleGridKey);
        MAST_Settings.hotkey.toggleGridMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.toggleGridMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Move grid up
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Move Grid Up", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.moveGridUpKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.moveGridUpKey);
        MAST_Settings.hotkey.moveGridUpMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.moveGridUpMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Move grid down
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Move Grid Down", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.moveGridDownKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.moveGridDownKey);
        MAST_Settings.hotkey.moveGridDownMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.moveGridDownMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Deselect prefab in palette
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Deselect Draw Tool and Palette Selection", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.deselectPrefabKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.deselectPrefabKey);
        MAST_Settings.hotkey.deselectPrefabMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.deselectPrefabMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Draw single
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Select Draw Single Tool", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.drawSingleKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.drawSingleKey);
        MAST_Settings.hotkey.drawSingleMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.drawSingleMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Draw continuous
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Select Draw Continuous Tool", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.drawContinuousKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.drawContinuousKey);
        MAST_Settings.hotkey.drawContinuousMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.drawContinuousMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Paint square
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Select Paint Square Tool", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.paintSquareKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.paintSquareKey);
        MAST_Settings.hotkey.paintSquareMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.paintSquareMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Randomizer
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Select Randomizer Tool", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.randomizerKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.randomizerKey);
        MAST_Settings.hotkey.randomizerMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.randomizerMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Erase
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Select Erase Tool", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.eraseKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.eraseKey);
        MAST_Settings.hotkey.eraseMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.eraseMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // New random seed
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Generate New Random(izer) Seed", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.newRandomSeedKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.newRandomSeedKey);
        MAST_Settings.hotkey.newRandomSeedMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.newRandomSeedMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Rotate prefab
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Rotate Prefab", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.rotatePrefabKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.rotatePrefabKey);
        MAST_Settings.hotkey.rotatePrefabMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.rotatePrefabMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Flip prefab
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Flip Prefab", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.flipPrefabKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.flipPrefabKey);
        MAST_Settings.hotkey.flipPrefabMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.flipPrefabMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Paint material
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Paint Material", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.paintMaterialKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.paintMaterialKey);
        MAST_Settings.hotkey.paintMaterialMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.paintMaterialMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Restore material
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        EditorGUILayout.LabelField("Restore Material", EditorStyles.boldLabel);
        
        MAST_Settings.hotkey.restoreMaterialKey =
            (KeyCode)EditorGUILayout.EnumPopup(
            "Key", MAST_Settings.hotkey.restoreMaterialKey);
        MAST_Settings.hotkey.restoreMaterialMod =
            (MAST_Hotkey_ScriptableObject.Modifier)EditorGUILayout.EnumPopup(
            "Modifier", MAST_Settings.hotkey.restoreMaterialMod);
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        // ----------------------------------
        // Change scriptable object
        // ----------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        // Get hotkey settings object path from selected item in project view
        if (GUILayout.Button(
            new GUIContent("Load Hotkey Settings from selected file in Project",
            "Use the selected (Hotkey Settings) file in project")))
        {
            string path = MAST_Asset_Loader.GetPathOfSelectedObjectTypeOf(typeof(MAST_Hotkey_ScriptableObject));
            
            if (path != "")
            {
                MAST_Settings.core.hotkeySettingsPath = path;
                MAST_Settings.Load_Hotkey_Settings();
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5f);
        
        EditorGUILayout.EndScrollView();
    }
#endregion
// ---------------------------------------------------------------------------
    
    // Return whether preferences have changed and set "preferences changed" back to false
    public static bool PreferencesChanged()
    {
        if (prefChanged)
        {
            MAST_Grid_Manager.UpdateGridSettings();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            
            prefChanged = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}

#endif