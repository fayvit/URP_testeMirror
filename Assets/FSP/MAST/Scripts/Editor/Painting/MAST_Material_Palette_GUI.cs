using UnityEngine;
using UnityEditor;

public static class MAST_Material_Palette_GUI
{
    private static Vector2 paletteScrollPos = new Vector2();  // Current scroll position
    const int scrollBarWidth = 19;  // Subtracted from scroll area width or height when calculating visible area
    
// ---------------------------------------------------------------------------
#region Palette GUI
// ---------------------------------------------------------------------------
    public static void DisplayPaletteGUI(float toolBarIconSize)
    {
        // Only draw material palette if it is ready
        if (MAST_Material_Palette.IsReady())
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
            "No materials to display!  Select your materials folder and click Load Materials",
            EditorStyles.wordWrappedLabel);
        GUILayout.Space(5f);
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }
    
    // ---------------------------------------------------------------------------
    // GUI to display the material palette
    // ---------------------------------------------------------------------------
    private static void DisplayPaletteGUIPopulated(float toolBarIconSize)
    {
        GUILayout.BeginVertical("MAST Toolbar BG");  // Begin toolbar vertical layout
        
        // ---------------------------------------------
        // Display Palette folder selection PopUp
        // ---------------------------------------------
        
        // Show material folder popup
        int newSelectedFolder = EditorGUILayout.Popup(MAST_Material_Palette.selectedFolderIndex, MAST_Material_Palette.GetFolderNameArray());
        
        // Remove focus from all controls.  Otherwise pressing SPACE will trigger this PopUp, even if clicking an item in the palette
        GUI.FocusControl(null);
        
        // If the palette folder was changed, change the palette items as well
        if (newSelectedFolder != MAST_Material_Palette.selectedFolderIndex)
        {
            RemoveMaterialSelection();
            MAST_Material_Palette.ChangeActivePaletteFolder(newSelectedFolder);
        }
        
        GUILayout.BeginHorizontal();
        
        // ---------------------------------------------
        // Calculate Palette SelectionGrid size
        // ---------------------------------------------
        
        // Vertical scroll view for palette items
        paletteScrollPos = EditorGUILayout.BeginScrollView(paletteScrollPos);
        
        // Get scrollview width and height of scrollview if is resized
        float scrollViewWidth = EditorGUIUtility.currentViewWidth - scrollBarWidth - toolBarIconSize - 20;
        
        int rowCount = Mathf.CeilToInt(MAST_Material_Palette.GetGUIContentArray().Length / (float)MAST_Interface_Data_Manager.state.materialPaletteColumnCount);
        float scrollViewHeight = rowCount * ((scrollViewWidth) / MAST_Interface_Data_Manager.state.materialPaletteColumnCount);
        
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
            MAST_Material_Palette.selectedItemIndex,
            MAST_Material_Palette.GetGUIContentArray(),
            MAST_Interface_Data_Manager.state.materialPaletteColumnCount,
            paletteGUISkin,
            GUILayout.Width((float)scrollViewWidth),
            GUILayout.Height(scrollViewHeight)
            );
        
        // If changes to UI value ocurred, update the grid
        if (EditorGUI.EndChangeCheck ()) {
            
            // If palette item was deselected by being clicked again
            if (newSelectedPaletteItemIndex == MAST_Material_Palette.selectedItemIndex)
            {
                RemoveMaterialSelection();
            }
            
            // If palette item selection has changed
            else
            {
                // Select the material in the material palette by index
                MAST_Material_Palette.selectedItemIndex = newSelectedPaletteItemIndex;
                
                // Make sure the paint material tool is selected
                MAST_Settings.gui.toolbar.selectedPaintToolIndex = 0;
            }
        }
        
        EditorGUILayout.EndScrollView();
        
        GUILayout.EndHorizontal();
        
        // Palette Column Count Slider
        MAST_Interface_Data_Manager.state.materialPaletteColumnCount =
            (int)GUILayout.HorizontalSlider(MAST_Interface_Data_Manager.state.materialPaletteColumnCount, 1, 10);
        
        GUILayout.Space(toolBarIconSize / 10);
        
        GUILayout.EndVertical();
    }
    
    // ---------------------------------------------------------------------------
    // Remove any active material selection and handle events
    // ---------------------------------------------------------------------------
    public static void RemoveMaterialSelection()
    {
        // Deselect any prefab
        if (MAST_Material_Palette.selectedItemIndex != -1)
            MAST_Material_Palette.selectedItemIndex = -1;
        
        // If the paint tool is selected, then clear the paint preview and deselect the paint tool
        if (MAST_Settings.gui.toolbar.selectedPaintToolIndex == 0)
        {
            MAST_Painting.ClearCurrentMaterialPaintPreview();
            MAST_Settings.gui.toolbar.selectedPaintToolIndex = -1;
        }
    }

#endregion
}
