using UnityEngine;

namespace SD.PlayerLogic
{
    /// <summary>
    /// Special class to provide player to receive damage.
    /// There MUST be a player class in one of parent objects.
    /// Actual player class doesn't have collider on damageable layer,
    /// so this class just transfers info.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    class PlayerDamageReceiver : MonoBehaviour, IDamageable
    {
        private Player player;
        public float Health => player.Health;

        void Start()
        {
            player = GetComponentInParent<Player>();
            Debug.Assert(player != null, "There must be a player class in one of parent objects", this);
        }

        public void ReceiveDamage(Damage damage)
        {
            player.ReceiveDamage(damage);
        }
    }
}
