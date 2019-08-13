using System;
using SD.Weapons;
using SD.PlayerLogic;

namespace SD.Game.Data
{
    [Serializable]
    struct InvWeapon
    {
        public WeaponIndex  Index;
        public int          Health;
        public bool         IsBought;

        public InvWeapon(WeaponIndex index, int health, bool isBought)
        {
            Index = index;
            Health = health;
            IsBought = isBought;
        }
    }

    [Serializable]
    struct InvAmmo
    {
        public AmmunitionType   Type;
        public int              Amount;

        public InvAmmo(AmmunitionType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }

    [Serializable]
    struct InvItem
    {
        public ItemType         Type;
        public int              Amount;

        public InvItem(ItemType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}
