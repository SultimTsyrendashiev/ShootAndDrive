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


        public bool             IsBroken => HealthRef.Value <= 0;
        public Weapons.WeaponData   Stats => GameController.Instance.WeaponsStats[This];

        public int              Health { get => HealthRef.Value; set => HealthRef.Value = value; }

        public WeaponIndex      Index => Stats.Index;
        public string           EditorName => Stats.EditorName;
        public string           TranslationKey => Stats.TranslationKey;
        public AmmunitionType   AmmoType => Stats.AmmoType;
        public int              Cost => Stats.Cost;
        public int              Durability => Stats.Durability;
        public float            Damage => Stats.Damage;
        public float            Accuracy => Stats.Accuracy;
        public int              RoundsPerMinute => Stats.RoundsPerMinute;
        public bool             IsAmmo => Stats.IsAmmo;
        public bool             CanBeDamaged => Stats.CanBeDamaged;
    }
}
