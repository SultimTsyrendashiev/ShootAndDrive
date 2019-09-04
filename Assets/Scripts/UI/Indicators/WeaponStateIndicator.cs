using UnityEngine;
using UnityEngine.UI;
using SD.Weapons;

namespace SD.UI.Indicators
{
    class WeaponStateIndicator : MonoBehaviour
    {
        [SerializeField]
        GameObject jammedText;

        [SerializeField]
        GameObject unjammingText;

        [SerializeField]
        GameObject breakText;

        [SerializeField]
        GameObject noAmmoText;

        void Start()
        {
            Weapon.OnAmmoRunOut += SetNoAmmo;
            Weapon.OnStateChange += ActivateNeededText;
        }

        void OnDestroy()
        {
            Weapon.OnAmmoRunOut -= SetNoAmmo;
            Weapon.OnStateChange -= ActivateNeededText;
        }

        void ActivateNeededText(WeaponState prev, WeaponState current)
        {
            switch (current)
            {
                case WeaponState.Breaking:
                    Activate(breakText);
                    break;

                case WeaponState.Jamming:
                case WeaponState.ReadyForUnjam:
                    Activate(jammedText);
                    break;

                case WeaponState.Unjamming:
                    Activate(unjammingText);
                    break;

                default:
                    Activate(null);
                    break;
            }
        }

        void SetNoAmmo(WeaponIndex obj)
        {
            Activate(noAmmoText);
        }

        void Activate(GameObject obj)
        {
            jammedText.SetActive(obj == jammedText);
            unjammingText.SetActive(obj == unjammingText);
            breakText.SetActive(obj == breakText);
            noAmmoText.SetActive(obj == noAmmoText);
        }
}
}
