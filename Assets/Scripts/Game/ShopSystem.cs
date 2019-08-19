using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Game.Shop
{
    class ShopSystem : IShop
    {
        IInventory inventory;

        public ShopSystem(IInventory inventory)
        {
            this.inventory = inventory;
        }

        public bool BuyAmmo(AmmunitionType type, bool buyAll)
        {
            IAmmoItem item = inventory.Ammo.Get(type);

            if (CanBeBought(type) == 0)
            {
                Debug.Log("ShopSystem: ammo must be buyable: " + type);
                return false;
            }

            int current = item.CurrentAmount;

            int newAmount;
            int diff;

            if (buyAll)
            {
                newAmount = item.MaxAmount;
                diff = newAmount - current;
            }
            else
            {
                newAmount = current + item.AmountToBuy;

                if (newAmount > item.MaxAmount)
                {
                    newAmount = item.MaxAmount;
                    diff = newAmount - current;
                }
                else
                {
                    diff = newAmount - current;
                }
            }

            if (!EnoughMoneyToBuy(item, newAmount))
            {
                return false;
            }

            inventory.Money -= GetAmmoPrice(item, diff);

            item.CurrentAmount = newAmount;

            return true;
        }

        public bool BuyWeapon(WeaponIndex type)
        {
            IWeaponItem item = inventory.Weapons.Get(type);

            if (!CanBeBought(type))
            {
                Debug.Log("ShopSystem: weapon must be buyable: " + type);
                return false;
            }

            if (!EnoughMoneyToBuy(item))
            {
                return false;
            }

            inventory.Money -= GetWeaponPrice(item);

            item.IsBought = true;
            item.Health = item.Durability;

            return true;
        }

        public bool RepairWeapon(WeaponIndex type)
        {
            IWeaponItem item = inventory.Weapons.Get(type);

            if (!CanBeRepaired(type))
            {
                Debug.Log("ShopSystem: weapon must be buyable: " + type);
                return false;
            }

            if (!EnoughMoneyToRepair(item))
            {
                return false;
            }

            int currentRepairCost = GetRepairCost(item);
            inventory.Money -= currentRepairCost;

            item.Health = item.Durability;

            return true;
        }

        //public List<ishop>

        public int CanBeBought(AmmunitionType type)
        {
            IAmmoItem item = inventory.Ammo.Get(type);

            Debug.Assert(item.MaxAmount >= item.CurrentAmount, "Ammo item must be <= than max amount" + item.This);
            return item.MaxAmount - item.CurrentAmount;
        }

        public bool EnoughMoneyToBuy(IAmmoItem item, int amount)
        {
            return GetAmmoPrice(item, amount) <= inventory.Money;
        }

        public bool EnoughMoneyToBuy(IWeaponItem item)
        {
            return GetWeaponPrice(item) <= inventory.Money;
        }

        public bool EnoughMoneyToRepair(IWeaponItem item)
        {
            return GetRepairCost(item) <= inventory.Money;
        }

        public bool CanBeBought(WeaponIndex type)
        {
            IWeaponItem item = inventory.Weapons.Get(type);

            return (!item.IsBought || item.Health == 0)
                && inventory.Money >= item.Price;
        }

        public bool CanBeRepaired(WeaponIndex type)
        {
            IWeaponItem item = inventory.Weapons.Get(type);

            int currentRepairCost = GetRepairCost(item);
            return item.IsBought
                && !item.CanBeDamaged
                && inventory.Money >= currentRepairCost;
        }

        public virtual int GetWeaponPrice(IWeaponItem item)
        {
            return item.Price;
        }

        public virtual int GetRepairCost(IWeaponItem item)
        {
            float healthPercentage = (float)item.Health / item.Durability;
            return (int)((1.0f - healthPercentage) * item.RepairCost);
        }

        /// <summary>
        /// Get price for any amount of ammo
        /// </summary>
        public virtual int GetAmmoPrice(IAmmoItem item, int amount)
        {
            if (amount % item.AmountToBuy == 0)
            {
                return item.Price * (amount / item.AmountToBuy);
            }

            int priceForOne = (int)((float)item.Price / item.AmountToBuy);
            return amount * priceForOne;
        }
    }
}
