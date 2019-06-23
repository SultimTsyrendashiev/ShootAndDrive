using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Weapons
{
    abstract class HitscanWeapon : Weapon
    {
        protected float spread;
        protected float range;
        protected int ammoConsumption;

        protected void CheckRay()
        {

        }
    }
}
