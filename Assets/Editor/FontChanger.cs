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

    public bool SetBestFit = false;
    public int MinSizeFont;
    public int MaxSizeFont;

    [MenuItem("GameObject/Font Changer")]
    static void CreateWizard()
    {
        DisplayWizard<FontChanger>("Change text fonts", "Change font");
    }

    void OnWizardCreate()
    {
        if (TextsParent != null)
        {
            var texts = TextsParent.GetComponentsInChildren<Text>(IncludeInactive);

            if (ForcedFont != null)
            {
                foreach (var t in texts)
                {
                    t.font = ForcedFont;
                }
            }
            
            if (SetBestFit && MinSizeFont >= 0 && MaxSizeFont >= 0 && MinSizeFont <= MaxSizeFont)
            {
                foreach (var t in texts)
                {
                    t.resizeTextForBestFit = true;
                    t.resizeTextMinSize = MinSizeFont;
                    t.resizeTextMaxSize = MaxSizeFont;
                }
            }
        }
    }
}
