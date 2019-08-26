using UnityEngine;

namespace SD.Enemies
{
    [RequireComponent(typeof(Collider))]
    class EnemyVehicleDamageReceiver : MonoBehaviour, IDamageable
    {
        [SerializeField]
        bool enableOnlyIfNonKinematic = false;

        EnemyVehicle vehicle;
        MeshCollider meshCollider;

        Collider[] allColliders;

        public float Health => ((IDamageable)vehicle).Health;

        public void Init(EnemyVehicle vehicle)
        {
            this.vehicle = vehicle;
            meshCollider = GetComponent<MeshCollider>();
            allColliders = GetComponents<Collider>();
        }

        public void ActivateNonKinematicCollider(bool active)
        {
            if (meshCollider != null)
            {
                if (!meshCollider.convex)
                {
                    // disable mesh collider 
                    // as rigidbody can work only with convex colliders
                    meshCollider.enabled = active;
                }
            }

            if (enableOnlyIfNonKinematic)
            {
                foreach (Collider c in allColliders)
                {
                    if (c != meshCollider)
                    {
                        c.enabled = !active;
                    }
                }
            }
        }

        public void ReceiveDamage(Damage damage)
        {
            ((IDamageable)vehicle).ReceiveDamage(damage);
        }
    }
}
