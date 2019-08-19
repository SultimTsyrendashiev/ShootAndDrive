using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD
{
    interface IShop
    {
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
    }
}
