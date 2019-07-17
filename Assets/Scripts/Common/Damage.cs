using UnityEngine;

namespace SD
{
    /// <summary>
    /// Represents damage.
    /// This class is readonly.
    /// </summary>
    class Damage
    {
        private float       value;
        private float       radius;
        private DamageType  type;
        private Vector3     direction;
        private Vector3     point;
        private Vector3     normal;
        private GameObject  initiator;

        /// <summary>
        /// Damage value in health points
        /// </summary>
        public float Value => value;

        /// <summary>
        /// Type of this damage
        /// </summary>
        public DamageType Type => type;

        /// <summary>
        /// Direction of this damage.
        /// Note: used only in bullet damage
        /// </summary>
        public Vector3 Direction => direction;

        /// <summary>
        /// Position of this damage
        /// </summary>
        public Vector3 Point => point;

        /// <summary>
        /// Normal of damage.
        /// Note: used only in bullet damage
        /// </summary>
        public Vector3 Normal => normal;

        /// <summary>
        /// Radius of this damage.
        /// Note: used only in explosions
        /// </summary>
        public float Radius => radius;

        /// <summary>
        /// Initiator entity of this damage (player, enemy)
        /// </summary>
        public GameObject Initiator => initiator;

        private Damage(float value, float radius, DamageType type, Vector3 direction, Vector3 point, Vector3 normal, GameObject initiator)
        {
            this.value = value;
            this.radius = radius;
            this.type = type;
            this.direction = direction;
            this.point = point;
            this.normal = normal;
            this.initiator = initiator;
        }

        /// <summary>
        /// Bullet damage constructor
        /// </summary>
        public static Damage CreateBulletDamage(float value, Vector3 direction, Vector3 point, Vector3 normal, GameObject initiator)
        {
            return new Damage(value, 0, DamageType.Bullet, direction, point, normal, initiator);
        }

        /// <summary>
        /// Explosion damage constructor
        /// </summary>
        public static Damage CreateExpolosionDamage(float value, float radius, Vector3 point, GameObject initiator)
        {
            return new Damage(value, radius, DamageType.Explosion, Vector3.zero, point, Vector3.zero, initiator);
        }

        public static Damage CreateFireDamage(float value, GameObject initiator)
        {
            return new Damage(value, 0, DamageType.Fire, Vector3.zero, Vector3.zero, Vector3.zero, initiator);
        }

        /// <summary>
        /// Calculate damage value for damaging entity according to its position
        /// </summary>
        public float CalculateDamageValue(Vector3 position)
        {
            switch (Type)
            {
                case DamageType.Explosion:
                    float length = (position - this.Point).magnitude;

                    if (length < radius / 2)
                    {
                        // if less that half of radius 
                        // take full damage
                        return value;
                    }
                    else if (length >= radius)
                    {
                        return 0.0f;
                    }

                    float dmgValue =  this.Value * (1.0f - length / radius);

                    return dmgValue;
                default:
                    return this.Value;
            }
        }
    }
}