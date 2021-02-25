using UnityEngine;
using UnityEditor;

public static class MAST_Prefab_Palette_GUI
{
    private static Vector2 paletteScrollPos = new Vector2();  // Current scroll position
    const int scrollBarWidth = 19;  // Subtracted from scroll area width or height when calculating visible area
    
// ---------------------------------------------------------------------------
#region Palette GUI
// ---------------------------------------------------------------------------
    public static void DisplayPaletteGUI(float toolBarIconSize)
    {
        // Only draw prefab palette if it is ready
        if (MAST_Prefab_Palette.IsReady())
            DisplayPaletteGUIPopulated(toolBarIconSize);
        else
            DisplayPaletteGUIPlaceholder();
    }
    
    private static void DisplayPaletteGUIPlaceholder()
    {
        GUILayout.BeginVertical("MAST Toolbar BG");
        GUILayout.Space(4f);
        GUILayout.BeginHorizontal();
        GUILayout.Space(5f);
        EditorGUILayout.LabelField(
            "No prefabs to display!  Select your prefabs folder and click Load Prefabs",
            EditorStyles.wordWrappedLabel);
        GUILayout.Space(5f);
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }
    
    // ---------------------------------------------------------------------------
    // GUI to display the prefab palette
    // ---------------------------------------------------------------------------
    private static void DisplayPaletteGUIPopulated(float toolBarIconSize)
    {
        GUILayout.BeginVertical("MAST Toolbar BG");  // Begin toolbar vertical layout
        
        // ---------------------------------------------
        // Display Palette folder selection PopUp
        // ---------------------------------------------
        
        // Show prefab folder popup
        int newSelectedFolder = EditorGUILayout.Popup(MAST_Prefab_Palette.selectedFolderIndex, MAST_Prefab_Palette.GetFolderNameArray());
        
        // Remove focus from all controls.  Otherwise pressing SPACE will trigger this PopUp, even if clicking an item in the palette
        GUI.FocusControl(null);
        
        // If the palette folder was changed, change the palette items as well
        if (newSelectedFolder != MAST_Prefab_Palette.selectedFolderIndex)
        {
            RemovePrefabSelection();
            MAST_Prefab_Palette.ChangeActivePaletteFolder(newSelectedFolder);
        }
        
        GUILayout.BeginHorizontal();
        
        // ---------------------------------------------
        // Calculate Palette SelectionGrid size
        // ---------------------------------------------
        
        // Vertical scroll view for palette items
        paletteScrollPos = EditorGUILayout.BeginScrollView(paletteScrollPos);
        
        // Get scrollview width and height of scrollview if is resized
        float scrollViewWidth = EditorGUIUtility.currentViewWidth - scrollBarWidth - toolBarIconSize - 20;
        
        int rowCount = Mathf.CeilToInt(MAST_Prefab_Palette.GetGUIContentArray().Length / (float)MAST_Interface_Data_Manager.state.prefabPaletteColumnCount);
        float scrollViewHeight = rowCount * ((scrollViewWidth) / MAST_Interface_Data_Manager.state.prefabPaletteColumnCount);
        
        // ---------------------------------------------
        // Get palette background image
        // ---------------------------------------------
        string paletteGUISkin = null;
        
        switch (MAST_Settings.gui.palette.bgColor)
        {
            case MAST_GUI_ScriptableObject.PaleteBGColor.Dark:
                paletteGUISkin = "MAST Palette Item Dark";
                break;
            case MAST_GUI_ScriptableObject.PaleteBGColor.Gray:
                paletteGUISkin = "MAST Palette Item Gray";
                break;
            case MAST_GUI_ScriptableObject.PaleteBGColor.Light:
                paletteGUISkin = "MAST Palette Item Light";
                break;
        }
        
        EditorGUI.BeginChangeCheck ();
        
        // ---------------------------------------------
        // Draw Palette SelectionGrid
        // ---------------------------------------------
        
        int newSelectedPaletteItemIndex = GUILayout.SelectionGrid(
            MAST_Prefab_Palette.selectedItemIndex,
            MAST_Prefab_Palette.GetGUIContentArray(),
            MAST_Interface_Data_Manager.state.prefabPaletteColumnCount,
            paletteGUISkin,
            GUILayout.Width((float)scrollViewWidth),
            GUILayout.Height(scrollViewHeight)
            );
        
        // If changes to UI value ocurred, update the grid
        if (EditorGUI.EndChangeCheck ()) {
            
            // If palette item was deselected by being clicked again
            if (newSelectedPaletteItemIndex == MAST_Prefab_Palette.selectedItemIndex)
            {
                RemovePrefabSelection();
            }
            
            // If palette item selection has changed
            else
            {
                MAST_Prefab_Palette.selectedItemIndex = newSelectedPaletteItemIndex;
                
                // If no draw tool is selected, then select the draw single tool
                if (MAST_Settings.gui.toolbar.selectedDrawToolIndex == -1)
                {
                    MAST_Settings.gui.toolbar.selectedDrawToolIndex = 0;
                    MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.DrawSingle);
                }
                
                // If erase draw tool isn't selected, change the visualizer prefab
                if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 4)
                    MAST_Placement_Interface.ChangeSelectedPrefab();
            }
        }
        
        EditorGUILayout.EndScrollView();
        
        GUILayout.EndHorizontal();
        
        // Palette Column Count Slider
        MAST_Interface_Data_Manager.state.prefabPaletteColumnCount =
            (int)GUILayout.HorizontalSlider(MAST_Interface_Data_Manager.state.prefabPaletteColumnCount, 1, 10);
        
        GUILayout.Space(toolBarIconSize / 10);
        
        GUILayout.EndVertical();
    }
    
    // ---------------------------------------------------------------------------
    // Remove any active prefab selection and handle events
    // ---------------------------------------------------------------------------
    public static void RemovePrefabSelection()
    {
        // Deselect any prefab
        if (MAST_Prefab_Palette.selectedItemIndex != -1)
            MAST_Prefab_Palette.selectedItemIndex = -1;
        
        // If erase draw tool isn't selected, remove the draw tool and visualizer
        if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 4)
        {
            MAST_Settings.gui.toolbar.selectedDrawToolIndex = -1;
            MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.None);
        }
    }

#endregion
}
