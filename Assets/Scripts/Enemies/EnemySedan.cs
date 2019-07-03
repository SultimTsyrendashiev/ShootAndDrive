using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    class EnemySedan : EnemyVehicle
    {
        [SerializeField]
        float speed;
        float attackLoopTime = 5;

        Rigidbody vehicleRigidbody;

        protected override void Activate()
        {
            vehicleRigidbody.velocity = transform.forward * speed;
            StartCoroutine(AttackLoop());
        }

        protected override void Die()
        {
            // TODO
        }

        IEnumerator AttackLoop()
        {
            while (State == EnemyVehicleState.Active)
            {
                yield return new WaitForSeconds(attackLoopTime);
                Attack();
            }
        }

        void Attack()
        {
            // TODO
        }
    }
}
