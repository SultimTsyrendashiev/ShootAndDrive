using UnityEngine;

namespace SD.Enemies
{
    [RequireComponent(typeof(Collider))]
    class EnemyVehicleDamageReceiver : MonoBehaviour, IDamageable
    {
        EnemyVehicle vehicle;
        MeshCollider meshCollider;

        public float Health => ((IDamageable)vehicle).Health;

        public void Init(EnemyVehicle vehicle)
        {
            this.vehicle = vehicle;
            meshCollider = GetComponent<MeshCollider>();
        }

        public void ActivateMeshCollider(bool active)
        {
            // disable mesh collider 
            // as rigidbody can work only with convex colliders
            if (meshCollider != null)
            {
                meshCollider.enabled = active;
            }
        }

        public void ReceiveDamage(Damage damage)
        {
            ((IDamageable)vehicle).ReceiveDamage(damage);
        }
    }
}
