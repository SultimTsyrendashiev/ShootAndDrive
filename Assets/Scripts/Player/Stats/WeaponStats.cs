//using System;
//using System.Collections.Generic;
//using SD.Weapons;

//namespace SD.PlayerLogic
//{
//    // Represents stats of a weapon in a shop
//    // Used in weapons system
//    // Values are constant
//    class WeaponStats
//    {
//        string name;            // name
//        AmmunitionType ammoType;      // what ammo type this weapon uses

//        int cost;               // cost in a shop
//        int durability;         // how many shots is needed to destroy weapon

//        float damage;           // in health points
//        float reloadingTime;    // in seconds
//        float accuracy;         // accuracy in percents

//        public WeaponStats(string name, AmmunitionType ammoType, int cost, int durability, float damage, float reloadingTime, float accuracy)
//        {
//            this.name = name;
//            this.ammoType = ammoType;

//            this.cost = cost;
//            this.durability = durability;

//            this.damage = damage;
//            this.reloadingTime = reloadingTime;
//            this.accuracy = accuracy;
//        }

//        #region getters
//        public string Name { get { return name; } }
//        public AmmunitionType AmmoType { get { return ammoType; } }
//        public int Cost { get { return cost; } }
//        /// <summary>
//        /// How many shots is needed to destory weapon
//        /// </summary>
//        public int Durability { get { return durability; } }
//        /// <summary>
//        /// In health points
//        /// </summary>
//        public float Damage { get { return damage; } }
//        public float ReloadingTime { get { return reloadingTime; } }
//        /// <summary>
//        /// In percents
//        /// </summary>
//        public float Accuracy { get { return accuracy; } }

//        /// <summary>
//        /// Get fire rate in rounds per minute
//        /// </summary>
//        public int GetFireRate()
//        {
//            return (int)(60.0f / reloadingTime);
//        }
//        #endregion
//    }
//}
