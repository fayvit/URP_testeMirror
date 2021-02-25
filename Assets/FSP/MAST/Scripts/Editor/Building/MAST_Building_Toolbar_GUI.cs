using UnityEngine;
using UnityEditor;

public static class MAST_Building_Toolbar_GUI
{
    // ------------------------------
    // Image Variables
    // ------------------------------
    private static Texture2D iconGridToggle;
    private static Texture2D iconGridUp;
    private static Texture2D iconGridDown;
    private static GUIContent[] guiContentDrawTool;
    private static Texture2D iconRotate;
    private static Texture2D iconFlip;
    private static Texture2D iconAxisX;
    private static Texture2D iconAxisY;
    private static Texture2D iconAxisZ;
    private static Texture2D iconLoadFromFolder;
    private static Texture2D iconSettings;
    private static Texture2D iconTools;
    
// ---------------------------------------------------------------------------
#region Load Images
// ---------------------------------------------------------------------------
    private static void LoadImages()
    {
        iconGridToggle = MAST_Asset_Loader.GetImage("Grid_Toggle.png");
        iconGridUp = MAST_Asset_Loader.GetImage("Grid_Up.png");
        iconGridDown = MAST_Asset_Loader.GetImage("Grid_Down.png");
        
        guiContentDrawTool = new GUIContent[5];
        guiContentDrawTool[0] = new GUIContent(MAST_Asset_Loader.GetImage("Pencil.png"), "Draw Single Tool");
        guiContentDrawTool[1] = new GUIContent(MAST_Asset_Loader.GetImage("Paint_Roller.png"), "Draw Continuous Tool");
        guiContentDrawTool[2] = new GUIContent(MAST_Asset_Loader.GetImage("Paint_Bucket.png"), "Paint Area Tool");
        guiContentDrawTool[3] = new GUIContent(MAST_Asset_Loader.GetImage("Randomizer.png"), "Randomizer Tool");
        guiContentDrawTool[4] = new GUIContent(MAST_Asset_Loader.GetImage("Eraser.png"), "Eraser Tool");
        
        iconRotate = MAST_Asset_Loader.GetImage("Rotate.png");
        iconFlip = MAST_Asset_Loader.GetImage("Flip.png");
        iconAxisX = MAST_Asset_Loader.GetImage("Axis_X.png");
        iconAxisY = MAST_Asset_Loader.GetImage("Axis_Y.png");
        iconAxisZ = MAST_Asset_Loader.GetImage("Axis_Z.png");
        iconLoadFromFolder = MAST_Asset_Loader.GetImage("Load_From_Folder.png");
        iconSettings = MAST_Asset_Loader.GetImage("Settings.png");
        iconTools = MAST_Asset_Loader.GetImage("Tools.png");
    }
#endregion
    
#region Toolbar GUI
// ---------------------------------------------------------------------------
    public static void DisplayToolbarGUI(float toolBarIconSize)
    {
        if (iconGridToggle == null)
        {
            LoadImages();
        }
        
        GUILayout.Space(toolBarIconSize / 10);
        
        GUILayout.BeginVertical();
        
        GUILayout.Space(toolBarIconSize / 7.5f);
        
        // ----------------------------------------------
        // Grid Section
        // ----------------------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        // ------------------------------------
        // Grid Up Button
        // ------------------------------------
        if (GUILayout.Button(new GUIContent(iconGridUp, "Move Grid Up"),
            "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
        {
            MAST_Grid_Manager.MoveGridUp();
        }
        
        // ------------------------------------
        // Toggle Grid Button
        // ------------------------------------
        EditorGUI.BeginChangeCheck ();
        
        // OnScene Enable/Disable Randomizer Icon Button
        MAST_Grid_Manager.gridExists = GUILayout.Toggle(
            MAST_Grid_Manager.gridExists,
            new GUIContent(iconGridToggle, "Toggle Scene Grid"),
            "MAST Toggle", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize));
        
        // If randomizer enabled value changed, process the change
        if (EditorGUI.EndChangeCheck ())
        {
            MAST_Grid_Manager.ChangeGridVisibility();
        }
        
        // ------------------------------------
        // Grid Down Button
        // ------------------------------------
        if (GUILayout.Button(new GUIContent(iconGridDown, "Move Grid Down"),
            "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
        {
            MAST_Grid_Manager.MoveGridDown();
        }
        
        GUILayout.EndVertical();
        
        GUILayout.Space(toolBarIconSize / 5);
        
        // ----------------------------------------------
        // Draw Tool Section
        // ----------------------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        // ------------------------------------
        // Add Draw Tool Toggle Group
        // ------------------------------------
        EditorGUI.BeginChangeCheck ();
        
        // Create drawtools SelectionGrid
        int newSelectedDrawToolIndex = GUILayout.SelectionGrid(
            MAST_Settings.gui.toolbar.selectedDrawToolIndex, 
            guiContentDrawTool, 1, "MAST Toggle",
            GUILayout.Width(toolBarIconSize), 
            GUILayout.Height(toolBarIconSize * 5));
        
        // If the draw tool was changed
        if (EditorGUI.EndChangeCheck ()) {
            
            // If the draw tool was clicked again, deselect it
            if (newSelectedDrawToolIndex == MAST_Settings.gui.toolbar.selectedDrawToolIndex)
            {
                MAST_Settings.gui.toolbar.selectedDrawToolIndex = -1;
                
                MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.None);
                
                MAST_Interface_Data_Manager.state.previousBuildToolIndex = -1;
            }
            
            // If a different draw tool was clicked, change to it
            else
            {
                MAST_Settings.gui.toolbar.selectedDrawToolIndex = newSelectedDrawToolIndex;
                
                switch (MAST_Settings.gui.toolbar.selectedDrawToolIndex)
                {
                    // Draw Single Tool selected
                    case 0:
                        MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.DrawSingle);
                        break;
                    
                    // Draw Continuous Tool selected
                    case 1:
                        MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.DrawContinuous);
                        break;
                    
                    // Flood Fill Tool selected
                    case 2:
                        MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.PaintArea);
                        break;
                    
                    // Randomizer Tool selected
                    case 3:
                        MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.Randomize);
                        SceneView.RepaintAll();
                        break;
                    
                    // Eraser Tool selected
                    case 4:
                        MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.Erase);
                        SceneView.RepaintAll();
                        break;
                }
                
            }
        }
        
        GUILayout.EndVertical();
        
        GUILayout.Space(toolBarIconSize / 5);
        
        // ----------------------------------------------
        // Manipulate Section
        // ----------------------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        // ------------------------------------
        // Rotate Button
        // ------------------------------------
        if (GUILayout.Button(new GUIContent(iconRotate, "Rotate Prefab/Selection"),
            "MAST Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize)))
        {
            MAST_Placement_Manipulate.RotateObject();
        }
        
        // OnScene Change Rotate Axis Icon Button
        switch (MAST_Placement_Manipulate.GetCurrentRotateAxis())
        {
            case MAST_Placement_Manipulate.Axis.X:
                if (GUILayout.Button(new GUIContent(iconAxisX, "Change Rotate Axis"),
                    "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                    MAST_Placement_Manipulate.ToggleRotateAxis();
                break;
            case MAST_Placement_Manipulate.Axis.Y:
                if (GUILayout.Button(new GUIContent(iconAxisY, "Change Rotate Axis"),
                    "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                    MAST_Placement_Manipulate.ToggleRotateAxis();
                break;
            case MAST_Placement_Manipulate.Axis.Z:
                if (GUILayout.Button(new GUIContent(iconAxisZ, "Change Rotate Axis"),
                    "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                    MAST_Placement_Manipulate.ToggleRotateAxis();
                break;
        }
        
        GUILayout.Space(toolBarIconSize / 10);
        
        // ------------------------------------
        // Flip Button
        // ------------------------------------
        if (GUILayout.Button(new GUIContent(iconFlip, "Flip Prefab/Selection"), 
            "MAST Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize)))
        {
            MAST_Placement_Manipulate.FlipObject();
        }
        
        // OnScene Change Flip Axis Icon Button
        switch (MAST_Placement_Manipulate.GetCurrentFlipAxis())
        {
            case MAST_Placement_Manipulate.Axis.X:
                if (GUILayout.Button(new GUIContent(iconAxisX, "Change Flip Axis"),
                    "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                    MAST_Placement_Manipulate.ToggleFlipAxis();
                break;
            case MAST_Placement_Manipulate.Axis.Y:
                if (GUILayout.Button(new GUIContent(iconAxisY, "Change Flip Axis"),
                    "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                    MAST_Placement_Manipulate.ToggleFlipAxis();
                break;
            case MAST_Placement_Manipulate.Axis.Z:
                if (GUILayout.Button(new GUIContent(iconAxisZ, "Change Flip Axis"),
                    "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                    MAST_Placement_Manipulate.ToggleFlipAxis();
                break;
        }
        
        GUILayout.EndVertical();
        
        GUILayout.Space(toolBarIconSize / 5);
        
        // ----------------------------------------------
        // Misc Section
        // ----------------------------------------------
        GUILayout.BeginVertical("MAST Toolbar BG Inset");
        
        // ------------------------------------
        // Load Palette Button
        // ------------------------------------
        if (GUILayout.Button(new GUIContent(iconLoadFromFolder, "Load prefabs from a project folder"),
            "MAST Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize)))
        {
            // Show choose folder dialog
            string chosenPath =
                EditorUtility.OpenFolderPanel("Choose the Folder that Contains your Prefabs",
                MAST_Interface_Data_Manager.state.prefabPath, "");
            
            // If a folder was chosen "Cancel was not clicked"
            if (chosenPath != "")
            {
                // Generate thumbnails, load prefabs, and select the first folder found (0)
                MAST_Prefab_Palette.GenerateThumbnailsAndLoadMaterials(chosenPath, 0, MAST_Settings.gui.palette.overwriteThumbnails);
                
                // Refresh the AssetDatabase incase any new thumbnails were created
                AssetDatabase.Refresh();
                
                // Save the prefab path and currently selected prefab folder index
                MAST_Interface_Data_Manager.state.prefabPath = chosenPath;
                MAST_Interface_Data_Manager.state.selectedPrefabPaletteFolderIndex = MAST_Prefab_Palette.selectedFolderIndex;
                
                //MAST_Interface_Data_Manager.Save_Palette_Items(true);
                MAST_Interface_Data_Manager.Save_Changes_To_Disk();
            }
        }
        
        GUILayout.EndVertical();
        
        GUILayout.EndVertical();
    }
#endregion
}