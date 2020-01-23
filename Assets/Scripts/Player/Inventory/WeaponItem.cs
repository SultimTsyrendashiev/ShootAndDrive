using UnityEngine;

namespace SD.PlayerLogic
{
    // Represents weapon item in player's inventory
    class WeaponItem : IWeaponItem
    {
        public bool         IsBought { get; set; }
        public bool         IsSelected { get; set; }
        public RefInt       HealthRef { get; }
        public WeaponIndex  This { get; }

        public WeaponItem(WeaponIndex weapon, int health, bool isBought, bool isSelected)
        {
            this.This = weapon;
            this.HealthRef = new RefInt(health);
            this.IsBought = isBought;
            this.IsSelected = isSelected;
        }


        /// <summary>
        /// Weapon is broken, if it's not an ammo and health == 0.
        /// Weapon is not broken, if it's an ammo or health > 0.
        /// </summary>
        public bool                 IsBroken => !IsAmmo && HealthRef.Value <= 0;
        public Weapons.WeaponData   Stats => GameController.Instance.WeaponsStats[This];

        public int Health
        {
            get
            {
                return HealthRef.Value;
            }
            set
            {
                HealthRef.Value = value;

                if (value == 0 && !IsAmmo)
                {
                    IsBought = false;
                }
            }
        }

        public WeaponIndex      Index => Stats.Index;
        public Sprite           Icon => Stats.Icon;
        public string           EditorName => Stats.EditorName;
        public string           TranslationKey => Stats.TranslationKey;
        public AmmunitionType   AmmoType => Stats.AmmoType;

        public int              Price => Stats.Cost;
        public int              RepairCost => Stats.RepairCost;

        public int              Durability => Stats.Durability;
        public float            Damage => Stats.Damage;
        public float            Accuracy => Stats.Accuracy;
        public int              RoundsPerMinute => Stats.RoundsPerMinute;
        public bool             IsAmmo => Stats.IsAmmo;
        public bool             CanBeDamaged => Stats.CanBeDamaged;
    }
}
