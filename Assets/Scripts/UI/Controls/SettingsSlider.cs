using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SD.UI.Menus;
using SD.Game.Settings;

namespace SD.UI.Controls
{
    [RequireComponent(typeof(Slider))]
    class SettingsSlider : MonoBehaviour
    {
        [SerializeField]
        string settingName;

        SettingsMenu settingsMenu;
        Slider slider;

        void Start()
        {
            settingsMenu = GetComponentInParent<SettingsMenu>();
            slider = GetComponent<Slider>();

            slider.onValueChanged.AddListener(SetValue);

            UpdateSlider();
        }

        void OnDestroy()
        {
            slider?.onValueChanged.RemoveAllListeners();
        }

        void SetValue(float value)
        {
            settingsMenu.ChangeFloatSetting(settingName, value);
        }

        void OnEnable()
        {
            UpdateSlider();
        }

        /// <summary>
        /// Slider must be udpdated on enabling and start
        /// </summary>
        void UpdateSlider()
        {
            if (slider != null && settingsMenu != null)
            {
                slider.value = settingsMenu.GetFloatSettingValue(settingName);
            }
        }
    }
}
