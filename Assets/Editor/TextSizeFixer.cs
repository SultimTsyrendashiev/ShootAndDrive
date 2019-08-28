using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class TextSizeFixer : ScriptableWizard
{
    public Transform TextsParent;
    public bool IncludeInactive = true;

    [MenuItem("GameObject/Text Size Fixer")]
    static void CreateWizard()
    {
        DisplayWizard<FontChanger>("Text Size Fixer", "Process");
    }

    void OnWizardCreate()
    {
        if (TextsParent != null)
        {
            
        }
    }
}
