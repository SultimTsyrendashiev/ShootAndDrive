using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SD.UI.Indicators
{
    class AmmoList : MonoBehaviour
    {
        class ImageText
        {
            public Image   Img;
            public Text    Txt;

            public ImageText(Image i, Text t)
            {
                Img = i;
                Txt = t;
            }
        }

        [SerializeField]
        GameObject ammoPrefab;
        
        [SerializeField]
        Color ammoAvailable;

        [SerializeField]
        Color ammoEmpty;

        [SerializeField]
        Color ammoHighlighted;

        IInventory inventory;
        Dictionary<AmmunitionType, ImageText> ammo;

        List<WeaponIndex> availableWeapons;
        List<AmmunitionType> availableAmmo;

        void Start()
        {
            ammo = new Dictionary<AmmunitionType, ImageText>();
            inventory = GameController.Instance.Inventory;
            var stats = GameController.Instance.AmmoStats;

            GameController.Instance.CurrentPlayer.OnPlayerStateChange += ProcessPlayerStateChange;

            // destroy in scene
            foreach (Transform t in transform)
            {
                if (t != transform)
                {
                    Destroy(t.gameObject);
                }
            }

            // create new
            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                GameObject o = Instantiate(ammoPrefab, transform);

                Image image = o.GetComponentInChildren<Image>();
                image.sprite = stats[a].Icon;

                ammo.Add(a, new ImageText(
                    image, o.GetComponentInChildren<Text>()));
            }

            UpdateAvailableAmmoList();
            UpdateList();
        }

        void ProcessPlayerStateChange(PlayerLogic.PlayerState state)
        {
            // update available ammo list when player is respawned
            if (state == PlayerLogic.PlayerState.Ready)
            {
                UpdateAvailableAmmoList();
            }
        }

        void OnEnable()
        {
            if (ammo == null)
            {
                return;
            }

            UpdateList();
        }

        void UpdateList()
        {
            foreach (AmmunitionType a in availableAmmo)
            {
                Set(a, inventory.Ammo.Get(a).CurrentAmount);
            }
        }

        void Set(AmmunitionType t, int amount)
        {
            ammo[t].Txt.text = amount.ToString();
            ammo[t].Img.color = 
                amount > 0 ? ammoAvailable : ammoEmpty;
        }

        void UpdateAvailableAmmoList()
        {
            availableWeapons = inventory.Weapons.GetAvailableWeaponsInGame();
            availableAmmo = inventory.Ammo.GetNecessaryAmmo(availableWeapons);
        }

        public void HighlightAmmo(AmmunitionType type)
        {
            // reset all to default colors
            UpdateList();

            // set new color for specific type
            Image img = ammo[type].Img;

            if (img.color != ammoEmpty)
            {
                img.color = ammoHighlighted;
            }
        }
    }
}
