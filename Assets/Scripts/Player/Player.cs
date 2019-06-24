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

        [SerializeField]
        private PlayerInventory inventory;

        private static Player instance;
        public static Player Instance => instance;

        public Camera MainCamera => playerCamera;
        internal PlayerInventory Inventory => inventory;

        void Start()
        {
            Debug.Assert(instance == null, "Several players in a scene");
          
            instance = this;

            playerCamera = GetComponentInChildren<Camera>();

            inventory = new PlayerInventory();
            inventory.Load();

            // for testing
            inventory.GiveAll();
        }

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
