using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct Damage
{
    public float Value;
    public DamageType Type;
    public Vector3 Source;
    public Vector3 Point;

    public GameObject Initiator;
}