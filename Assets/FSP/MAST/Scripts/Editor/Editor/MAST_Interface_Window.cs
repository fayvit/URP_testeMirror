using System;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

[Serializable]
public class MAST_Interface_Window : EditorWindow
{
    // ---------------------------------------------------------------------------
    // Add menu named "Open MAST Palette" to the Window menu
    // ---------------------------------------------------------------------------
    [MenuItem("Tools/MAST/Open MAST Window", false, 16)]
    private static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow(typeof(MAST_Interface_Window)).Show();
    }
    
// ---------------------------------------------------------------------------
#region Variable Declaration
// ---------------------------------------------------------------------------
    
    // ------------------------------
    // Persisent Class Variables
    // ------------------------------
    
    // Initialize Hotkeys Class if needed and return HotKeysClass
    [SerializeField] private MAST_Hotkeys HotKeysClass;
    private MAST_Hotkeys HotKeys
    {
        get
        {
            if(HotKeysClass == null)
                HotKeysClass = new MAST_Hotkeys();
            return HotKeysClass;
        }
    }
    
    // ------------------------------
    // Editor Window Variables
    // ------------------------------
    [SerializeField] private bool inPlayMode = false;
    [SerializeField] private bool isCleanedUp = false;
    
    // ------------------------------
    // GUIStyle Variables
    // ------------------------------
    [SerializeField] private GUISkin guiSkin;
    
    // ------------------------------
    // Draw Tool Variables
    // ------------------------------
    [SerializeField] private float toolBarIconSize;
    
#endregion
    
    // ---------------------------------------------------------------------------
    // Perform Initializations
    // ---------------------------------------------------------------------------
    void Awake() // This runs on the first time open
    {
        //Debug.Log("Interface - Awake");
    }
    
    void OnFocus()
    {
        //Debug.Log("Interface - On Focus");
    }
    
    // ---------------------------------------------------------------------------
    // MAST Window is Enabled
    // ---------------------------------------------------------------------------
    private void OnEnable()
    {
        
        //Debug.Log("Interface - On Enable");
        
        // Initialize Preference Manager
        MAST_Settings.Initialize();
        
        // Initialize the MAST State Manager
        MAST_Interface_Data_Manager.Initialize();
        
        // Set up deletegates so that OnScene is called automatically
        #if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= this.OnScene;
            SceneView.duringSceneGui += this.OnScene;
        #else
            SceneView.onSceneGUIDelegate -= this.OnScene;
            SceneView.onSceneGUIDelegate += this.OnScene;
        #endif
        
        // Set up deletegates for exiting editor mode and returning to editor mode from play mode
        MAST_PlayModeStateListener.onExitEditMode -= this.ExitEditMode;
        MAST_PlayModeStateListener.onExitEditMode += this.ExitEditMode;
        MAST_PlayModeStateListener.onEnterEditMode -= this.EnterEditMode;
        MAST_PlayModeStateListener.onEnterEditMode += this.EnterEditMode;
        
        
        // Set scene to be updated by mousemovement
        wantsMouseMove = true;
        
        // If Enabled in Editor Mode
        if (!inPlayMode)
        {
            // Load custom gui styles
            if (guiSkin == null)
                guiSkin = MAST_Asset_Loader.GetGUISkin();
            
            // Load interface data back from saved state
            MAST_Interface_Data_Manager.Load_Interface_State();
            
            // Create a new grid if needed
            if (MAST_Interface_Data_Manager.state.gridExists)
            {
                MAST_Grid_Manager.gridExists = true;
                MAST_Grid_Manager.ChangeGridVisibility();
            }
            
            // Change placement mode back to what was saved
            MAST_Placement_Interface.ChangePlacementMode(
                (MAST_Placement_Interface.PlacementMode)MAST_Settings.gui.toolbar.selectedDrawToolIndex);
            
            // TODO:  Reselect item in palette
            //MAST_Palette.selectedItemIndex = newSelectedPaletteItemIndex;
            
            // If palette images were lost due to serialization, load the palette from saved state
            //if (MAST_Prefab_Palette.ArePaletteImagesLost())
            //{
                MAST_Interface_Data_Manager.Restore_Palette_Items();
            //}
            
        }
        
        // If Enabled in Run Mode
        else
        {
            // Nothing so far, because everything is being triggered in ExitEditMode event method
        }
    }
    
    // ---------------------------------------------------------------------------
    // Save and Restore MAST Interface variables to keep state on play
    // ---------------------------------------------------------------------------
    private void ExitEditMode()
    {
        //Debug.Log("Interface - Exit Edit Mode");
        
        // Don't allow this method to run twice
        if (inPlayMode)
            return;
        
        // If the grid exists
        if (MAST_Grid_Manager.gridExists)
        {
            // Destroy the grid so it doesn't show while playing
            MAST_Grid_Manager.DestroyGrid();
            
            // Make sure the grid is restored after returning to editor
            MAST_Grid_Manager.gridExists = true;
        }
        
        inPlayMode = true;
        isCleanedUp = false;
    }
    
    private void EnterEditMode()
    {
        //Debug.Log("Interface - Enter Edit Mode");
        
        // Don't allow this method to run twice
        if (!inPlayMode)
            return;
        
        // Load interface data back from saved state
        MAST_Interface_Data_Manager.Load_Interface_State();
        MAST_Grid_Manager.ChangeGridVisibility();
        
        // Restore the interface saved state
        MAST_Interface_Data_Manager.Restore_Palette_Items();
        
        // Change placement mode back to what was saved
        MAST_Placement_Interface.ChangePlacementMode(
            (MAST_Placement_Interface.PlacementMode)MAST_Settings.gui.toolbar.selectedDrawToolIndex);
        
        // Repaint all views
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        
        inPlayMode = false;
    }
    
    // ---------------------------------------------------------------------------
    // Perform Cleanup when MAST Window is Disabled
    // ---------------------------------------------------------------------------
    private void OnDisable()
    {
        //Debug.Log("Interface - On Disable");
        
        // Save MAST Settings to Scriptable Objects
        MAST_Settings.Save_Settings();
        
        // If OnDisable triggered by going fullscreen, closing MAST, or changing scenes
        if (!inPlayMode)
            MAST_Interface_Data_Manager.Save_Interface_State();
        
        // If OnDisable is triggered by the user hitting play button
        else
        {
            // If cleanup hasn't already ocurred
            if (!isCleanedUp)
            {
                // Load interface and palette data state
                MAST_Interface_Data_Manager.Save_Interface_State();
                MAST_Interface_Data_Manager.Save_Palette_Items();
                
                CleanUpInterface();
                
                isCleanedUp = true;
            }
        }
        
        // Remove SceneView delegate
        #if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= this.OnScene;
        #else
            SceneView.onSceneGUIDelegate -= this.OnScene;
        #endif
    }
    
    // ---------------------------------------------------------------------------
    // Perform Cleanup when MAST Window is Closed
    // ---------------------------------------------------------------------------
    private void OnDestroy()
    {
        //Debug.Log("Interface - On Destroy");
    }
    
    // ---------------------------------------------------------------------------
    // Clean-up
    // ---------------------------------------------------------------------------
    private void CleanUpInterface()
    {
        //Debug.Log("Cleaning Up Interface");
        
        // Delete placement grid
        MAST_Grid_Manager.DestroyGrid();
        
        // Deselect palette item and delete visualizer
        MAST_Prefab_Palette.selectedItemIndex = -1;
        MAST_Placement_Visualizer.RemoveVisualizer();
        
        // Deselect draw tool and change placement mode to none
        MAST_Settings.gui.toolbar.selectedDrawToolIndex = -1;
        MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.None);
        
        MAST_Settings.gui.toolbar.selectedPaintToolIndex = -1;
        MAST_Material_Palette_GUI.RemoveMaterialSelection();
        
        // Cancel any drawing or painting
        MAST_Interface_Data_Manager.state.previousBuildToolIndex = -1;
        MAST_Interface_Data_Manager.state.previousPaintToolIndex = -1;
        MAST_Placement_PaintArea.DeletePaintArea();
    }
    
    // ---------------------------------------------------------------------------
    // Runs every frame
    // ---------------------------------------------------------------------------
    private void Update()
    {
        
    }
    
// ---------------------------------------------------------------------------
#region SceneView
// ---------------------------------------------------------------------------
    private void OnScene(SceneView sceneView)
    {
        // Exit now if game is playing
        //if (inPlayMode)
        //     return;
        
        // Handle SceneView GUI
        SceneviewGUI(sceneView);
        
        // Handle view focus
        ProcessMouseEnterLeaveSceneview();
        
        // Process HotKeys and repaint all views if any changes were made
        if (HotKeys.ProcessHotkeys())
        {
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
        
        switch (MAST_Interface_Data_Manager.state.selectedInterfaceTab)
        {
            // Build tab selected
            case 0:
                // If draw tool was deselected by a hotkey
                if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 1)
                    // Stop any drawing
                    if (MAST_Interface_Data_Manager.state.previousBuildToolIndex == 1)
                        MAST_Interface_Data_Manager.state.previousBuildToolIndex = -1;
                
                // If paint area tool was deselected by a hotkey
                if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 2)
                    // Stop any painting
                    if (MAST_Interface_Data_Manager.state.previousBuildToolIndex == 2)
                    {
                        MAST_Placement_PaintArea.DeletePaintArea();
                        MAST_Interface_Data_Manager.state.previousBuildToolIndex = -1;
                    }
                
                // If erase tool was deselected by a hotkey
                if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 4)
                    // Stop any erasing
                    if (MAST_Interface_Data_Manager.state.previousBuildToolIndex == 4)
                        MAST_Interface_Data_Manager.state.previousBuildToolIndex = -1;
                
                // If a palette item is selected or erase tool is selected, handle object placement
                if (MAST_Prefab_Palette.selectedItemIndex > -1 ||
                    MAST_Settings.gui.toolbar.selectedDrawToolIndex == 4)
                    ObjectPlacement();
                break;
            
            // Paint tab selected
            case 1:
                // If paint material tool selected
                if (MAST_Settings.gui.toolbar.selectedPaintToolIndex != -1)
                {
                    MaterialPaint();
                }
                break;
        }
    }
    
    // Handle SceneView GUI
    private void SceneviewGUI(SceneView sceneView)
    {
        bool scrollWheelUsed = false;
        
        // If SHIFT key is held down
        if (Event.current.shift)
        {
            // If mouse scrollwheel was used
            if (Event.current.type == EventType.ScrollWheel)
            {
                // If scrolling wheel down
                if (Event.current.delta.y > 0)
                {
                    // Select next prefab and cycle back to top of prefabs if needed
                    MAST_Prefab_Palette.selectedItemIndex++;
                    if (MAST_Prefab_Palette.selectedItemIndex >= MAST_Prefab_Palette.GetPrefabArray().Length)
                        MAST_Prefab_Palette.selectedItemIndex = 0;
                    
                    scrollWheelUsed = true;
                }
                
                // If scrolling wheel up
                else if (Event.current.delta.y < 0)
                {
                    // Select previous prefab and cycle back to bottom of prefabs if needed
                    MAST_Prefab_Palette.selectedItemIndex--;
                    if (MAST_Prefab_Palette.selectedItemIndex < 0)
                        MAST_Prefab_Palette.selectedItemIndex = MAST_Prefab_Palette.GetPrefabArray().Length - 1;
                    
                    scrollWheelUsed = true;
                }
            }
        }
        
        // If successfully scrolled wheel
        if (scrollWheelUsed)
        {
            // If no draw tool is selected, then select the draw single tool
            if (MAST_Settings.gui.toolbar.selectedDrawToolIndex == -1)
            {
                MAST_Settings.gui.toolbar.selectedDrawToolIndex = 0;
                MAST_Placement_Interface.ChangePlacementMode(MAST_Placement_Interface.PlacementMode.DrawSingle);
            }
            
            // If erase draw tool isn't selected, change the visualizer prefab
            if (MAST_Settings.gui.toolbar.selectedDrawToolIndex != 4)
                MAST_Placement_Interface.ChangeSelectedPrefab();
            
            // Repaint all views
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            
            // Keep mouseclick from selecting other objects
            GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
            Event.current.Use();
        }
        
        
    }
    
    // Handle events when mouse point enter or leaves the SceneView
    void ProcessMouseEnterLeaveSceneview()
    {
        // If mouse enters SceneView window, show visualizer
        if (Event.current.type == EventType.MouseEnterWindow)
            MAST_Placement_Visualizer.SetVisualizerVisibility(true);
        
        // If mouse leaves SceneView window
        else if (Event.current.type == EventType.MouseLeaveWindow)
        {
            // Hide visualizer
            MAST_Placement_Visualizer.SetVisualizerVisibility(false);
            
            // Stop any drawing single
            if (MAST_Interface_Data_Manager.state.previousBuildToolIndex == 1)
                MAST_Interface_Data_Manager.state.previousBuildToolIndex = -1;
            
            // Stop any drawing continuous
            if (MAST_Interface_Data_Manager.state.previousBuildToolIndex == 2)
            {
                MAST_Placement_PaintArea.DeletePaintArea();
                MAST_Interface_Data_Manager.state.previousBuildToolIndex = -1;
            }
            
            // Stop any erasing
            if (MAST_Interface_Data_Manager.state.previousBuildToolIndex == 4)
                MAST_Interface_Data_Manager.state.previousBuildToolIndex = -1;
        }
    }
    
    void MaterialPaint()
    {
        // Get mouse events for object placement when in the Scene View
        Event currentEvent = Event.current;
        
        switch (MAST_Settings.gui.toolbar.selectedPaintToolIndex)
        {
            // Paint material tool
            case 0:
                // If no material is selected, exit without attempting to paint
                if (MAST_Material_Palette.selectedItemIndex == -1)
                    return;
                
                // Preview the material paint
                MAST_Painting.PreviewPaint(MAST_Material_Palette.GetSelectedMaterial());
                
                // If not already painting and the left mouse button pressed
                if (MAST_Interface_Data_Manager.state.previousPaintToolIndex != 1)
                    if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                    {
                        // Start painting
                        MAST_Interface_Data_Manager.state.previousPaintToolIndex = 1;
                        
                        MAST_Painting.StartPainting();
                        
                        // Keep mouseclick from selecting other objects
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                        Event.current.Use();
                    }
                
                // If painting and left mouse button not released
                if (MAST_Interface_Data_Manager.state.previousPaintToolIndex == 1)
                {
                    
                    // If left mouse button released, stop painting
                    if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
                    {
                        MAST_Painting.StopPainting();
                        MAST_Interface_Data_Manager.state.previousPaintToolIndex = -1;
                    }
                }
                break;
            
            // Restore material tool
            case 1:
                // If any material in the palette was selected, deselect it
                if (MAST_Material_Palette.selectedItemIndex != -1)
                    MAST_Material_Palette.selectedItemIndex = -1;
                
                // Preview the material restore "providing a null material triggers restore mode"
                MAST_Painting.PreviewPaint((Material)null);
                
                // If not already painting and the left mouse button pressed
                if (MAST_Interface_Data_Manager.state.previousPaintToolIndex != 2)
                    if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                    {
                        // Start restoring
                        MAST_Interface_Data_Manager.state.previousPaintToolIndex = 2;
                        
                        MAST_Painting.StartPainting();
                        
                        // Keep mouseclick from selecting other objects
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                        Event.current.Use();
                    }
                
                // If painting and left mouse button not released
                if (MAST_Interface_Data_Manager.state.previousPaintToolIndex == 2)
                {
                    // If left mouse button released, stop painting
                    if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
                    {
                        MAST_Painting.StopPainting();
                        MAST_Interface_Data_Manager.state.previousPaintToolIndex = -1;
                    }
                }
                break;
        }
        
        
    }
    
    // Handle object placement
    private void ObjectPlacement()
    {
        // Get mouse events for object placement when in the Scene View
        Event currentEvent = Event.current;
        
        // Change position of visualizer
        MAST_Placement_Visualizer.UpdateVisualizerPosition();
        
        switch (MAST_Settings.gui.toolbar.selectedDrawToolIndex)
        {
            // Draw single tool
            case 0:
                
            // Draw continuous tool
            case 1:
                // If not already drawing and the left mouse button pressed
                if (MAST_Interface_Data_Manager.state.previousBuildToolIndex != 1)
                    if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                    {
                        // Start drawing
                        MAST_Interface_Data_Manager.state.previousBuildToolIndex = 1;
                        
                        // Keep mouseclick from selecting other objects
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                        Event.current.Use();
                    }
                
                // If drawing and left mouse button not released
                if (MAST_Interface_Data_Manager.state.previousBuildToolIndex == 1)
                {
                    // Place selected prefab on grid
                    MAST_Placement_Place.PlacePrefabInScene();
                    
                    // If left mouse button released
                    if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
                        MAST_Interface_Data_Manager.state.previousBuildToolIndex = -1;
                }
                break;
                
            // Paint area tool
            case 2:
                // If not already painting and the left mouse button pressed
                if (MAST_Interface_Data_Manager.state.previousBuildToolIndex != 2)
                    if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                    {
                        // Start drawing
                        MAST_Interface_Data_Manager.state.previousBuildToolIndex = 2;
                        
                        // Start paint area at current mouse location
                        MAST_Placement_PaintArea.StartPaintArea();
                        
                        // Keep mouseclick from selecting other objects
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                        Event.current.Use();
                    }
                
                // If drawing and left mouse button not released
                if (MAST_Interface_Data_Manager.state.previousBuildToolIndex == 2)
                {
                    // Update the paint area as the mouse moves
                    MAST_Placement_PaintArea.UpdatePaintArea();
                    
                    // If left mouse button released
                    if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
                    {
                        MAST_Placement_PaintArea.CompletePaintArea();
                        MAST_Interface_Data_Manager.state.previousBuildToolIndex = -1;
                    }
                }
                break;
            
            // Randomizer tool
            case 3:
                // If left mouse button was clicked
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                {
                    // Keep mouseclick from selecting other objects
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Event.current.Use();
                    
                    // Place selected prefab on grid
                    MAST_Placement_Place.PlacePrefabInScene();
                }
                break;
            
            // Erase tool
            case 4:
                // If not already erasing and the left mouse button pressed
                if (MAST_Interface_Data_Manager.state.previousBuildToolIndex != 4)
                    if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                    {
                        // Start drawing
                        MAST_Interface_Data_Manager.state.previousBuildToolIndex = 4;
                        
                        // Keep mouseclick from selecting other objects
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                        Event.current.Use();
                    }
                
                // If erasing and left mouse button not released
                if (MAST_Interface_Data_Manager.state.previousBuildToolIndex == 4)
                {
                    // Place selected prefab on grid
                    MAST_Placement_Interface.ErasePrefab();
                    
                    // If left mouse button released
                    if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
                        MAST_Interface_Data_Manager.state.previousBuildToolIndex = -1;
                }
                break;
        }
        
    }
#endregion
    
// ---------------------------------------------------------------------------
#region Custom Editor Window Interface
// ---------------------------------------------------------------------------
    void OnGUI()
    {
        // Exit now if game is playing
        //if (inPlayMode)
        //    return;
        
        // Load custom skin
        GUI.skin = guiSkin;
        
        string[] tabCaptions = {"Build", "Paint", "Settings", "Tools"};
        
        // MAST interface tabs
        MAST_Interface_Data_Manager.state.selectedInterfaceTab
            = GUILayout.Toolbar (MAST_Interface_Data_Manager.state.selectedInterfaceTab, tabCaptions);
        
        // Depending on tab selected, show the appropriate interface
        switch (MAST_Interface_Data_Manager.state.selectedInterfaceTab) {
            case 0:
                DisplayStagingGUI();
                break;
            case 1:
                DisplayPaintingGUI();
                break;
            case 2:
                MAST_Settings_GUI.DisplaySettingsGUI(guiSkin);
                break;
            case 3:
                MAST_Tools_GUI.DisplayToolsGUI();
                break;
        }
        
        // ----------------------------------------------
        // Redraw MAST window if mouse is moved
        // ----------------------------------------------
        if (Event.current.type == EventType.MouseMove)
            Repaint();
        
        // ----------------------------------------------
        // Process Hotkeys
        // ----------------------------------------------
        if (HotKeys.ProcessHotkeys())
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }
    
    private void DisplayStagingGUI()
    {
        GUILayout.BeginHorizontal("MAST Toolbar BG");  // Begin entire window horizontal layout
        
        // Calculate toolbar icon size
        toolBarIconSize = (position.height / 15.3f) * MAST_Settings.gui.toolbar.scale;
        
        // If toolbar is on the left
        if (MAST_Settings.gui.toolbar.position == MAST_GUI_ScriptableObject.ToolbarPos.Left)
        {
            MAST_Building_Toolbar_GUI.DisplayToolbarGUI(toolBarIconSize);
            MAST_Prefab_Palette_GUI.DisplayPaletteGUI(toolBarIconSize);
        }
        
        // If toolbar is on the right
        else
        {
            MAST_Prefab_Palette_GUI.DisplayPaletteGUI(toolBarIconSize);
            MAST_Building_Toolbar_GUI.DisplayToolbarGUI(toolBarIconSize);
        }
        
        GUILayout.EndHorizontal(); // End of entire window horizontal layout
    }
    
    private void DisplayPaintingGUI()
    {
        GUILayout.BeginHorizontal("MAST Toolbar BG");  // Begin entire window horizontal layout
        
        // Calculate toolbar icon size
        toolBarIconSize = (position.height / 15.3f) * MAST_Settings.gui.toolbar.scale;
        
        // If toolbar is on the left
        if (MAST_Settings.gui.toolbar.position == MAST_GUI_ScriptableObject.ToolbarPos.Left)
        {
            MAST_Painting_Toolbar_GUI.DisplayToolbarGUI(toolBarIconSize);
            MAST_Material_Palette_GUI.DisplayPaletteGUI(toolBarIconSize);
        }
        
        // If toolbar is on the right
        else
        {
            MAST_Material_Palette_GUI.DisplayPaletteGUI(toolBarIconSize);
            MAST_Painting_Toolbar_GUI.DisplayToolbarGUI(toolBarIconSize);
        }
        
        GUILayout.EndHorizontal(); // End of entire window horizontal layout
    }
    
    
#endregion

}

#endif