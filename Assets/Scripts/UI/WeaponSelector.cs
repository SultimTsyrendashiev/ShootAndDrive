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
        InputController     inputController;
        [SerializeField]
        ItemsPosition       itemsPosition;

        // TODO: remove, use instead WeaponData
        [SerializeField]
        Sprite              icon;

        PlayerInventory     inventory;  // inventory of current player
        bool                wasSelected;
        WeaponIndex         selectedWeapon;
        List<WeaponIndex>   availableWeapons;
        int                 count;

        void Start()
        {
            var p = FindObjectOfType<GameController>().CurrentPlayer;
            Debug.Assert(p != null, "Can't find player", this);

            inventory = p.Inventory;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            // enable hud
            inputController.OnWeaponSelectorDown();

            // get available weapons
            availableWeapons = inventory.GetAvailableWeapons();
            count = availableWeapons.Count > 5 ? 5 : availableWeapons.Count;

            // place them on hud
            Transform weaponIcons = itemsPosition.transform;
            for (int i = 0; i < count; i++)
            {
                Image weaponImage = weaponIcons.GetChild(i).GetComponent<Image>();
                weaponImage.sprite = icon;
            }

            itemsPosition.SetPositions(count);

            wasSelected = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            int index = -1;
            float minLength = Threshold * inputController.GetComponent<Canvas>().scaleFactor;

            for (int i = 0; i < count; i++)
            {
                Vector2 pos = itemsPosition.transform.GetChild(i).position;
                float l = (pos - eventData.position).magnitude;

                if (l < minLength)
                {
                    minLength = l;
                    index = i;
                }

                itemsPosition.transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }

            if (index == -1)
            {
                wasSelected = false;
                return;
            }

            itemsPosition.transform.GetChild(index).GetComponent<Image>().color = Color.white;

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
