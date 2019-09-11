using UnityEngine;
using SD.UI.Menus;

namespace SD.UI.Controls
{
    /// <summary>
    /// Settings toggle class, but can be used out of settings menu.
    /// Note: SettingsToggle must be contained by a child object of SettingMenu,
    /// this script can be anywhere
    /// </summary>
    class SettingsToggleExternal : SettingsToggle
    {
        [SerializeField]
        SettingsMenu settingsMenu;

        protected override SettingsMenu FindSettingsMenu()
        {
            Debug.Assert(settingsMenu != null, "'SettingsMenu' is not set", this);

            return settingsMenu;
        }
    }
}
