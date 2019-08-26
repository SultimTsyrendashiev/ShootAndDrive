using UnityEngine;
using UnityEngine.UI;
using SD.PlayerLogic;

namespace SD.UI.Indicators
{
    class PlayerHealthBar : MonoBehaviour
    {
        // player; use this field ONLY for events
        Player player;

        // indicator image itself
        [SerializeField]
        Image healthImage;
        
        // for animations
        Animation healthImageAnim;
        Color healthDefaultColor;

        [SerializeField]
        float maxHealthImageWidth = 160;

        float maxPlayerHealth;
        float minHealthForRegeneration;

        void Awake()
        {
            Player.OnPlayerSpawn += Init;

            healthImageAnim = healthImage.GetComponent<Animation>();
            healthDefaultColor = healthImage.color;
        }

        void Init(Player player)
        {
            this.player = player;

            // get all variables from player
            maxPlayerHealth = Player.MaxHealth; // player.MaxHealth;
            minHealthForRegeneration = Player.MinHealthForRegeneration;

            // default
            SetHealth(player.Health);

            player.OnHealthChange += SetHealth;
        }

        void OnDestroy()
        {
            if (player != null)
            {
                player.OnHealthChange -= SetHealth;
            }

            Player.OnPlayerSpawn -= Init;
        }

        /// <summary>
        /// Set health in HUD
        /// </summary>
        /// <param name="health">health in [0..100]</param>
        public void SetHealth(float health)
        {
            Vector2 d = healthImage.rectTransform.sizeDelta;
            d.x = health / maxPlayerHealth * maxHealthImageWidth;

            healthImage.rectTransform.sizeDelta = d;

            if (health <= minHealthForRegeneration)
            {
                healthImageAnim?.Play();
            }
            else
            {
                healthImageAnim?.Stop();
                healthImage.color = healthDefaultColor;
            }
        }
    }
}
