using UnityEngine;

namespace SD.Enemies
{
    /// <summary>
    /// Passenger's animations.
    /// Attack animation name must contain "Attack".
    /// Damage animation name must contain "Damage".
    /// Death animation name must contain "Death".
    /// </summary>
    [RequireComponent(typeof(Animation))]
    class EnemyPassengerAnimation : MonoBehaviour
    {
        VehiclePassenger passenger;
        Animation passengerAnimation;

        const string AttackNamePart = "Attack";
        const string DamageNamePart = "Damage";
        const string DeathNamePart = "Death";

        string attackAnimationName;
        string damageAnimationName;
        string dieAnimationName;

        public void Init(VehiclePassenger passenger)
        {
            this.passenger = passenger;

            passengerAnimation = GetComponent<Animation>();

            foreach (AnimationState s in passengerAnimation)
            {
                if (s.name.Contains(AttackNamePart))
                {
                    attackAnimationName = s.name;
                }
                else if (s.name.Contains(DamageNamePart))
                {
                    damageAnimationName = s.name;
                }
                else if (s.name.Contains(DeathNamePart))
                {
                    dieAnimationName = s.name;
                }
            }
        }

        /// <summary>
        /// Play attack animation
        /// </summary>
        public void Attack()
        {
            if (!string.IsNullOrEmpty( attackAnimationName))
            passengerAnimation.Play(attackAnimationName, PlayMode.StopAll);
        }

        /// <summary>
        /// Play damaging animation. Duration of it
        /// should be less than 'VehiclePassenger.TimeForDamage'
        /// </summary>
        public void Damage()
        {
            if (!string.IsNullOrEmpty(attackAnimationName))
                passengerAnimation.Play(damageAnimationName, PlayMode.StopAll);
        }

        /// <summary>
        /// Play dieing animation
        /// </summary>
        public void Die()
        {
            if (!string.IsNullOrEmpty(attackAnimationName))
                passengerAnimation.Play(dieAnimationName, PlayMode.StopAll);
        }

        public void Reset()
        {
            passengerAnimation.Stop();
        }
    }
}
