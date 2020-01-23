using System;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Game.Shop
{
    class ShopSystem : IShop
    {
        IInventory inventory;

        public event BuyWeapon OnWeaponBuy;
        public event BuyWeapon OnWeaponRepair;
        public event BuyAmmo OnAmmoBuy;

        public void Init(IInventory inventory)
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

            if (buyAll)
            {
                newAmount = item.MaxAmount;
            }
            else
            {
                newAmount = current + item.AmountToBuy;

                if (newAmount > item.MaxAmount)
                {
                    newAmount = item.MaxAmount;
                }
            }

            // how many ammo to buy
            int diff = newAmount - current;

            if (!EnoughMoneyToBuy(item, diff))
            {
                return false;
            }

            int price = GetAmmoPrice(item, diff);
            inventory.Money -= price;

            OnAmmoBuy?.Invoke(item.This, price);

            item.CurrentAmount = newAmount;

            return true;
        }

        public bool BuyWeapon(WeaponIndex type)
        {
            IWeaponItem item = inventory.Weapons.Get(type);

            if (item.IsAmmo)
            {
                bool boughtAsAmmo = BuyAmmo(item.AmmoType, false);

                if (boughtAsAmmo)
                {
                    item.IsBought = true;
                }

                return boughtAsAmmo;
            }

            if (!CanBeBought(type))
            {
                Debug.Log("ShopSystem: weapon must be buyable: " + type);
                return false;
            }

            if (!EnoughMoneyToBuy(item))
            {
                return false;
            }

            int price = GetWeaponPrice(item);
            inventory.Money -= price;

            item.IsBought = true;
            item.Health = item.Durability;

            // weapon is bought, most likely that player
            // wants to add it to selected
            inventory.Weapons.Select(item.Index);

            OnWeaponBuy?.Invoke(item.Index, price);

            return true;
        }

        public bool RepairWeapon(WeaponIndex type)
        {
            IWeaponItem item = inventory.Weapons.Get(type);

            if (!CanBeRepaired(type))
            {
                Debug.Log("ShopSystem: weapon must be repairable: " + type);
                return false;
            }

            if (!EnoughMoneyToRepair(item))
            {
                return false;
            }

            int currentRepairCost = GetRepairCost(item);
            inventory.Money -= currentRepairCost;

            item.Health = item.Durability;

            OnWeaponRepair?.Invoke(item.Index, currentRepairCost);

            return true;
        }

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
            if (!item.IsAmmo)
            {
                return GetWeaponPrice(item) <= inventory.Money;
            }
            else
            {
                IAmmoItem ammoItem = inventory.Ammo.Get(item.AmmoType);
                return EnoughMoneyToBuy(ammoItem, ammoItem.AmountToBuy);
            }
        }

        public bool EnoughMoneyToRepair(IWeaponItem item)
        {
            if (item.IsAmmo)
            {
                Debug.LogError("ShopSystem::Trying to repair weapon that is ammo");
            }

            return GetRepairCost(item) <= inventory.Money;
        }

        public bool CanBeBought(WeaponIndex type)
        {
            IWeaponItem item = inventory.Weapons.Get(type);

            return !item.IsBought && inventory.Money >= item.Price;
        }

        public bool CanBeRepaired(WeaponIndex type)
        {
            IWeaponItem item = inventory.Weapons.Get(type);

            int currentRepairCost = GetRepairCost(item);
            return item.IsBought
                && item.CanBeDamaged
                && !item.IsAmmo
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
