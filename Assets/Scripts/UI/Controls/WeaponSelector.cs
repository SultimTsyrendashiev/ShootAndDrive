using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SD.PlayerLogic;
using SD.Weapons;

namespace SD.UI
{
    public class WeaponSelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        const float Threshold = 100;

        [SerializeField]
        Canvas              canvas;
        [SerializeField]
        InputController     inputController;
        [SerializeField]
        ItemsPosition       itemsPosition;

        // TODO: remove, use instead WeaponData
        [SerializeField]
        Sprite              icon;

        IInventory          inventory;

        bool                wasSelected;
        WeaponIndex         selectedWeapon;
        int                 count;

        void Awake()
        {
            Player.OnPlayerSpawn += Init;
        }

        void Init(Player player)
        {
            inventory = player.Inventory;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            // enable hud
            inputController.OnWeaponSelectorDown();

            // get available weapons
            var availableWeapons = inventory.Weapons.GetAvailableWeapons();
            count = availableWeapons.Count > 5 ? 5 : availableWeapons.Count;

            // place them on hud
            Transform weaponIcons = itemsPosition.transform;
            for (int i = 0; i < count; i++)
            {
                Image weaponImage = weaponIcons.GetChild(i).GetComponentInChildren<Image>();
                weaponImage.sprite = icon;
            }

            itemsPosition.SetPositions(count);

            wasSelected = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            int index = -1;
            float minLength = Threshold * canvas.scaleFactor;

            for (int i = 0; i < count; i++)
            {
                Vector2 pos = itemsPosition.transform.GetChild(i).position;
                float l = (pos - eventData.position).magnitude;

                if (l < minLength)
                {
                    minLength = l;
                    index = i;
                }

                itemsPosition.transform.GetChild(i).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f);
            }

            if (index == -1)
            {
                wasSelected = false;
                return;
            }

            itemsPosition.transform.GetChild(index).GetComponentInChildren<Image>().color = Color.white;

            // temp
            wasSelected = true;
            selectedWeapon = (WeaponIndex)index;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (wasSelected)
            {
                inputController.SelectWeapon(selectedWeapon);
            }

            inputController.OnWeaponSelectorUp();
        }
    }
}
