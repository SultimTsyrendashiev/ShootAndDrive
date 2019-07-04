using System.Collections;
using UnityEngine;

namespace SD.Enemies
{
    class EnemySedan : EnemyVehicle
    {
        [SerializeField]
        float speed;
        [SerializeField]
        float attackLoopTime = 2;

        Rigidbody vehicleRigidbody;

        protected override void Activate()
        {
            vehicleRigidbody.velocity = transform.forward * speed;
            StartCoroutine(AttackLoop());
        }

        protected override void Die()
        {
            StopAllCoroutines();

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
            // each passenger starts to shoot
            foreach (var p in Passengers)
            {
                p.StartAttack(Target);
            }
        }
    }
}
