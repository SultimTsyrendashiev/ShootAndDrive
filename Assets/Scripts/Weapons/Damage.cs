using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SD.Weapons
{
    struct Damage
    {
        public float Value;
        public DamageType Type;
        public Vector3 Source;
        public Vector3 Point;
        public Vector3 Normal;

        public GameObject Initiator;

        public Damage(float value, DamageType type, Vector3 source, Vector3 point, Vector3 normal, GameObject initiator)
        {
            this.Value = value;
            this.Type = type;
            this.Source = source;
            this.Point = point;
            this.Normal = normal;
            this.Initiator = initiator;
        }
    }
}