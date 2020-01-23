using UnityEngine;

namespace SD.PlayerLogic
{
    // Represents weapon item in player's inventory
    class WeaponItem : IWeaponItem
    {
        public bool         IsBought { get; set; }
        public bool         IsSelected => holder.IsSelected(This);
        public RefInt       HealthRef { get; }
        public WeaponIndex  This { get; }

        WeaponsHolder holder;

        public WeaponItem(WeaponsHolder holder, WeaponIndex weapon, int health, bool isBought)
        {
            this.This = weapon;
            this.holder = holder;
            this.HealthRef = new RefInt(health);
            this.IsBought = isBought;
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
