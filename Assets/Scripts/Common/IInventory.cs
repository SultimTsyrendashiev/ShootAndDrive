using System.Collections.Generic;
using UnityEngine;

namespace SD
{
    interface IInventory
    {

        IWeaponsHolder      Weapons { get; }
        IAmmoHolder         Ammo { get; }
        int                 Money { get; set; }

        event PlayerBalanceChange OnBalanceChange;
    }

    interface IWeaponsHolder
    {
        bool                IsAvailable(WeaponIndex weapon);
        List<WeaponIndex>   GetAvailableWeapons();

        bool                IsAvailableInGame(WeaponIndex weapon);
        List<WeaponIndex>   GetAvailableWeaponsInGame();

        IWeaponItem         Get(WeaponIndex weapon);
        // ICollection<IWeaponItem>    GetAll();
    }

    interface IAmmoHolder
    {
        List<AmmunitionType> GetAvailableAmmo();
        /// <summary>
        /// Get available ammo, based on given weapons list
        /// </summary>
        List<AmmunitionType> GetAvailableAmmo(List<WeaponIndex> weapons);

        IAmmoItem Get(AmmunitionType ammoType);
        // ICollection<IAmmoItem> GetAll();
    }

    /// <summary>
    /// Represents ammo item in player's inventory
    /// </summary>
    interface IAmmoItem
    {
        /// <summary>
        /// Get / set current amount. Setted value will be in range [0..MaxAmount]
        /// </summary>
        int                 CurrentAmount { get; set; }


        string              TranslationKey { get; }
        AmmunitionType      This { get; }

        Sprite              Icon { get; }

        int                 MaxAmount { get; }

        int                 AmountToBuy { get; }
        int                 Price { get; }
    }

    /// <summary>
    /// Represents weapon item in player's inventory
    /// </summary>
    interface IWeaponItem
    {
        bool                IsBought { get; set; }
        int                 Health { get; set; }
        /// <summary>
        /// If true, can be used in a game
        /// </summary>
        bool                IsSelected { get; set; }


        WeaponIndex         Index { get; }

        string              EditorName { get; }
        string              TranslationKey { get; }

        AmmunitionType      AmmoType { get; }

        bool                IsAmmo { get; }
        bool                CanBeDamaged { get; }

        int                 Price { get; }
        int                 RepairCost { get; }

        int                 Durability { get; }
        float               Damage { get; }
        float               Accuracy { get; }
        int                 RoundsPerMinute { get; }
    } 
}
