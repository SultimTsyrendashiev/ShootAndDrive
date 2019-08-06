using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShadowModeChanger : ScriptableWizard
{
    public UnityEngine.Rendering.ShadowCastingMode Mode = UnityEngine.Rendering.ShadowCastingMode.Off;
    public Transform Parent;
    public bool IncludeInactive = true;

    [MenuItem("GameObject/Shadow mode changer")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ShadowModeChanger>("Change shadow casting mode on all children", "Process");
    }

    void OnWizardCreate()
    {
        if (Parent != null)
        {
            var ts = Parent.GetComponentsInChildren<MeshRenderer>(IncludeInactive);

            foreach (var t in ts)
            {
                t.shadowCastingMode = Mode;
            }
        }
    }
}
