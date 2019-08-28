using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.Game.Settings
{
    abstract class ASetting
    {
        public GlobalSettings Settings { get; }

        public ASetting(GlobalSettings settings)
        {
            Settings = settings;
        }

        string GetTranslated(string key)
        {
            try
            {
                return GameController.Instance.Languages.GetValue(Settings.GameLanguage, key);
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
        /// Change value for this setting
        /// </summary>
        public abstract void ChangeValue();
    }
}
