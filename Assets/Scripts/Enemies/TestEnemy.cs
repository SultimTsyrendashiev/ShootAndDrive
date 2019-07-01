using System;
using System.Collections.Generic;
using UnityEngine;
using SD.Weapons;

namespace SD.Enemies
{
    class TestEnemy : MonoBehaviour, IDamageable
    {
        public float Health => 10;

        [SerializeField]
        private ParticleSystem particles;

        public void ReceiveDamage(Damage damage)
        {
            if (damage.Type == DamageType.Bullet)
            {
                particles.transform.position = damage.Point;
                particles.transform.rotation = Quaternion.LookRotation(damage.Normal);

                particles.Emit(15);
            }
        }
    }
}
