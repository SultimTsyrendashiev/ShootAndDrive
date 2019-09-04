using UnityEngine;
using UnityEngine.UI;
using SD.Weapons;

namespace SD.UI.Indicators
{
    class AmmoIndicatorColor : MonoBehaviour
    {
        [SerializeField]
        Text currentAmmoText;

        [SerializeField]
        Color readyStateColor;
        [SerializeField]
        Color notReadyStateColor;

        void Awake()
        {
            Weapon.OnStateChange += ChangeColor;
        }

        void OnDestroy()
        {
            Weapon.OnStateChange -= ChangeColor;
        }

        void ChangeColor(WeaponState prev, WeaponState current)
        {
            currentAmmoText.color = current == WeaponState.Ready ?
                readyStateColor : notReadyStateColor;
        }
    }
}
