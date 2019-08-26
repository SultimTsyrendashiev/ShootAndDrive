using UnityEngine;

namespace SD.Weapons
{
    [CreateAssetMenu(menuName = "Ammo Data", order = 51)]
    class AmmoData : ScriptableObject
    {
        [SerializeField]
        AmmunitionType type;
        [SerializeField]
        string editorName;
        [SerializeField]
        string translationKey;

        [SerializeField]
        Sprite icon;

        [SerializeField]
        int maxAmount;

        [SerializeField]
        int amountToBuy;
        [SerializeField]
        int price;

        [SerializeField]
        bool isWeapon = false;


        public string       TranslationKey => translationKey;
        public string       EditorName => editorName;
        public AmmunitionType Type => type;

        public Sprite       Icon => icon;

        public int          MaxAmount => maxAmount;

        public int          AmountToBuy => amountToBuy;
        public int          Price => price;
    }
}
