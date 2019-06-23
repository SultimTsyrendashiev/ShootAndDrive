using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SD.Weapons;
using UnityEngine;

namespace SD.Player
{
    class Player : MonoBehaviour, IDamageable
    {
        private Camera playerCamera;
        private float health;

        PlayerInventory inventory;

        private static Player instance;
        public static Player Instance { get { return instance; } }

        public Camera MainCamera { get { return playerCamera; } }


        #region inherited
        float IDamageable.Health
        {
            get
            {
                return health;
            }
        }

        void IDamageable.ReceiveDamage(Damage damage)
        {
        
        }
        #endregion
    }
}
