using System;

namespace SD.Game.Settings
{
    abstract class ASetting
    {
        public event Action<GlobalSettings> OnSettingUpdate;

        public GlobalSettings Settings { get; }

        public ASetting(GlobalSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// Called after initializing all settings.
        /// In this method should be all subscription to other settings.
        /// </summary>
        public virtual void Init(SettingsSystem settingsSystem) { }

        string GetTranslated(string key)
        {
            try
            {
                return GameController.Instance.Localization.GetValue(Settings.GameLanguage, key);
            }
            catch
            {
                return key;
            }
        }


        /// <summary>
        /// Get translated name of this setting
        /// </summary>
        //public string GetTranslatedName()
        //{
        //    return GetTranslated(GetNameKey());
        //}

        /// <summary>
        /// Get translated value of this setting
        /// </summary>
        public string GetTranslatedValue()
        {
            return GetTranslated(GetValueTranslationKey());
        }


        /// <summary>
        /// Get settings key for this setting
        /// </summary>
        public abstract string GetSettingsKey();

        /// <summary>
        /// Get translation key of value for this setting
        /// </summary>
        public abstract string GetValueTranslationKey();

        /// <summary>
        /// Change value for this setting and call event
        /// </summary>
        public void ChangeSetting()
        {
            ChangeValue();

            OnSettingUpdate?.Invoke(Settings);
        }

        /// <summary>
        /// Change value for this setting
        /// </summary>
        protected abstract void ChangeValue();
    }
}
