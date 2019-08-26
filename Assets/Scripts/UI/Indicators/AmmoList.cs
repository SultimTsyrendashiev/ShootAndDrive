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
            public GameObject   Obj;
            public Image        Img;
            public Text         Txt;

            public ImageText(Image i, Text t, GameObject obj)
            {
                Img = i;
                Txt = t;
                Obj = obj;
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

        [SerializeField]
        Color ammoHighlightedEmpty;

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
                    image, o.GetComponentInChildren<Text>(), o));
            }

            UpdateAvailableAmmoList();
            UpdateList();
        }

        void OnDestro()
        {
            GameController.Instance.CurrentPlayer.OnPlayerStateChange -= ProcessPlayerStateChange;
        }

        /// <summary>
        /// Updates available ammo list when player is respawned
        /// </summary>
        void ProcessPlayerStateChange(PlayerLogic.PlayerState state)
        {
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
            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                if (!availableAmmo.Contains(a))
                {
                    Disable(a);
                }
            }

            foreach (AmmunitionType a in availableAmmo)
            {
                Set(a, inventory.Ammo.Get(a).CurrentAmount);
            }
        }

        void Set(AmmunitionType t, int amount)
        {
            var a = ammo[t];

            a.Obj.SetActive(true);

            // set text 
            a.Txt.text = amount.ToString();

            // set color
            a.Img.color = a.Txt.color =
                amount > 0 ? ammoAvailable : ammoEmpty;
        }

        void Disable(AmmunitionType t)
        {
            ammo[t].Obj.SetActive(false);
        }

        void UpdateAvailableAmmoList()
        {
            availableWeapons = inventory.Weapons.GetAvailableWeaponsInGame();
            availableAmmo = inventory.Ammo.GetNecessaryAmmo(availableWeapons);
        }

        public void HighlightAmmo(AmmunitionType type)
        {
            UnhighlightAll();

            // if ammo indicator is hidden
            if (!ammo[type].Obj.activeSelf)
            {
                // then ignore
                return;
            }

            // set new color for specific type
            Image img = ammo[type].Img;
            Text txt = ammo[type].Txt;

            if (img.color != ammoEmpty)
            {
                img.color = txt.color = ammoHighlighted;
            }
            else
            {
                img.color = txt.color = ammoHighlightedEmpty;
            }
        }

        public void UnhighlightAll()
        {
            UpdateList();
        }
    }
}
