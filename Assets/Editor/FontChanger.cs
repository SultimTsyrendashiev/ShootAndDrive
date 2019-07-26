using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FontChanger : ScriptableWizard
{
    public Transform TextsParent;
    public Font ForcedFont;
    public bool IncludeInactive = true;

    [MenuItem("GameObject/Font Changer")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<FontChanger>("Change text fonts", "Change font");
    }

    void OnWizardCreate()
    {
        if (TextsParent != null && ForcedFont != null)
        {
            var texts = TextsParent.GetComponentsInChildren<Text>(IncludeInactive);

            foreach (var t in texts)
            {
                t.font = ForcedFont;
            }
        }
    }
}
