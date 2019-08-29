using System;

namespace SD
{
    interface IShop
    {
        void Init(IInventory inventory);

        bool BuyAmmo(AmmunitionType type, bool buyAll);
        bool BuyWeapon(WeaponIndex type);
        bool RepairWeapon(WeaponIndex type);

        int CanBeBought(AmmunitionType type);
        bool CanBeBought(WeaponIndex type);
        bool CanBeRepaired(WeaponIndex type);

        bool EnoughMoneyToBuy(IAmmoItem item, int amount);
        bool EnoughMoneyToBuy(IWeaponItem item);
        bool EnoughMoneyToRepair(IWeaponItem item);

        int GetAmmoPrice(IAmmoItem item, int amount);
        int GetRepairCost(IWeaponItem item);
        int GetWeaponPrice(IWeaponItem item);

        event BuyWeapon     OnWeaponBuy;
        event BuyWeapon     OnWeaponRepair;
        event BuyAmmo       OnAmmoBuy;
    }
}
