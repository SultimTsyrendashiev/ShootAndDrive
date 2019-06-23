using System;
using System.Collections.Generic;
using SD.Weapons;

namespace SD
{
    interface IDamageable
    {
        float Health { get; }
        void ReceiveDamage(Damage damage);
    }
}
