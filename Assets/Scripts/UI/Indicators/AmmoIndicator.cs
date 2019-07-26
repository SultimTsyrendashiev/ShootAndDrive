using UnityEngine;
using UnityEngine.UI;
using SD.Weapons;

namespace SD.UI.Indicators
{
    class AmmoIndicator : MonoBehaviour
    {
        [SerializeField]
        Text currentAmmoText;

        void Awake()
        {
            // default
            SetAmmoAmount(-1);
            Weapon.OnAmmoChange += SetAmmoAmount;
        }

        void OnDestroy()
        {
            Weapon.OnAmmoChange -= SetAmmoAmount;
        }

        public void SetAmmoAmount(int amount)
        {
            currentAmmoText.text = amount >= 0 ? amount.ToString() : "";
        }
    }
}
