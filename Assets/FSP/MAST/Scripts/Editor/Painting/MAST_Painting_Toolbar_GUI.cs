using UnityEngine;
using UnityEditor;

public static class MAST_Painting_Toolbar_GUI
{
    // ------------------------------
    // Image Variables
    // ------------------------------
    private static GUIContent[] guiContentDrawTool;
    private static Texture2D iconLoadFromFolder;
    
// ---------------------------------------------------------------------------
#region Load Images
// ---------------------------------------------------------------------------
    private static void LoadImages()
    {
        guiContentDrawTool = new GUIContent[2];
        guiContentDrawTool[0] = new GUIContent(MAST_Asset_Loader.GetImage("Paint_Brush.png"), "Paint Material Tool");
        guiContentDrawTool[1] = new GUIContent(MAST_Asset_Loader.GetImage("Cleaning_Brush.png"), "Restore Material Tool");
        
        iconLoadFromFolder = MAST_Asset_Loader.GetImage("Load_From_Folder.png");
    }
#endregion
    
#region Toolbar GUI
// ---------------------------------------------------------------------------
    public static void DisplayToolbarGUI(float toolBarIconSize)
    {
        if (iconLoadFromFolder == null)
        {
            LoadImages();
        }
        
        GUILayout.Space(toolBarIconSize / 10);
        
        GUILayout.BeginVertical();
        
        GUILayout.Space(toolBarIconSize / 7.5f);
        
        // ----------------------------------------------
        // Draw Tool Section
        // ----------------------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        // ------------------------------------
        // Add Draw Tool Toggle Group
        // ------------------------------------
        EditorGUI.BeginChangeCheck ();
        
        // Create drawtools SelectionGrid
        int newSelectedPaintToolIndex = GUILayout.SelectionGrid(
            MAST_Settings.gui.toolbar.selectedPaintToolIndex, 
            guiContentDrawTool, 1, "MAST Toggle",
            GUILayout.Width(toolBarIconSize), 
            GUILayout.Height(toolBarIconSize * 2));
        
        // If the draw tool was changed
        if (EditorGUI.EndChangeCheck ()) {
            
            // If the paint tool was clicked again, deselect it
            if (newSelectedPaintToolIndex == MAST_Settings.gui.toolbar.selectedPaintToolIndex)
            {
                MAST_Settings.gui.toolbar.selectedPaintToolIndex = -1;
                
                MAST_Interface_Data_Manager.state.previousPaintToolIndex = -1;
                
                MAST_Painting.ClearCurrentMaterialPaintPreview();
            }
            
            // If a different paint tool was selected, change it
            else
            {
                MAST_Settings.gui.toolbar.selectedPaintToolIndex = newSelectedPaintToolIndex;
            }
        }
        
        GUILayout.EndVertical();
        
        GUILayout.Space(toolBarIconSize / 5);
        
        // ----------------------------------------------
        // Misc Section
        // ----------------------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        // ------------------------------------
        // Load Material Palette Button
        // ------------------------------------
        if (GUILayout.Button(new GUIContent(iconLoadFromFolder, "Load materials from a project folder"),
            "MAST Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize)))
        {
            // Show choose folder dialog
            string chosenPath =
                EditorUtility.OpenFolderPanel("Choose the Folder that Contains your Materials",
                MAST_Interface_Data_Manager.state.materialPath, "");
            
            // If a folder was chosen "Cancel was not clicked"
            if (chosenPath != "")
            {
                // Generate thumbnails, load materials, and select the first folder found (0)
                MAST_Material_Palette.GenerateThumbnailsAndLoadMaterials(chosenPath, 0, MAST_Settings.gui.palette.overwriteThumbnails);
                
                // Refresh the AssetDatabase incase any new thumbnails were created
                AssetDatabase.Refresh();
                
                // Save the material path and currently selected material folder index
                MAST_Interface_Data_Manager.state.materialPath = chosenPath;
                MAST_Interface_Data_Manager.state.selectedMaterialPaletteFolderIndex = MAST_Material_Palette.selectedFolderIndex;
                
                //MAST_Interface_Data_Manager.Save_Palette_Items(true);
                MAST_Interface_Data_Manager.Save_Changes_To_Disk();
            }
        }
        
        GUILayout.EndVertical();
        
        GUILayout.EndVertical();
    }
#endregion
}