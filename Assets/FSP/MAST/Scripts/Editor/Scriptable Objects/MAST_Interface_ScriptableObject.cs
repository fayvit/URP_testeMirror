using System;
using UnityEngine;

#if (UNITY_EDITOR)

[Serializable]
public class MAST_Interface_ScriptableObject : ScriptableObject
{
    [SerializeField] public int selectedInterfaceTab = 0;
    
    [SerializeField] public bool gridExists = false;
    
    [SerializeField] public int previousBuildToolIndex = -1;
    [SerializeField] public int selectedBuildToolIndex = -1;
    [SerializeField] public int selectedPrefabIndex = -1;
    
    [SerializeField] public string prefabPath = "Assets";
    [SerializeField] public int selectedPrefabPaletteFolderIndex = 0;
    [SerializeField] public int prefabPaletteColumnCount = 3;
    
    [SerializeField] public int previousPaintToolIndex = -1;
    [SerializeField] public int selectedPaintToolIndex = -1;
    [SerializeField] public int selectedMaterialIndex = -1;
    
    [SerializeField] public string materialPath = "Assets";
    [SerializeField] public int selectedMaterialPaletteFolderIndex = 0;
    [SerializeField] public int materialPaletteColumnCount = 3;
}

#endif