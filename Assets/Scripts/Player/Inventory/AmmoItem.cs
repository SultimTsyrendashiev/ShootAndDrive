using UnityEngine;

namespace SD.PlayerLogic 
{
    class AmmoItem : IAmmoItem
    {
        public AmmunitionType This { get; private set; }
        int currentAmount;

        /// <summary>
        /// Get / set current amount. Setted value will be in range [0..MaxAmount]
        /// </summary>
        public int CurrentAmount
        {
            get
            {
                return currentAmount;
            }
            set
            {
                currentAmount = Mathf.Clamp(value, 0, MaxAmount);
            }
        }

        public AmmoItem(AmmunitionType type, int currentAmount)
        {
            this.This = type;
            this.currentAmount = currentAmount;
        }


        public Weapons.AmmoData Stats => GameController.Instance.AmmoStats[This];

        public string           TranslationKey => Stats.TranslationKey;
        public string           EditorName => Stats.EditorName;
        public Sprite           Icon => Stats.Icon;
        public int              MaxAmount => Stats.MaxAmount;
        public int              AmountToBuy => Stats.AmountToBuy;
        public int              Price => Stats.Price;
    }
}
