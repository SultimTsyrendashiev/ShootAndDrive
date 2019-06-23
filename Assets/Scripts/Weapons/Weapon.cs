using UnityEngine;

public abstract class Weapon
{
    protected string name;
    protected Damage damage;
    protected AmmoType ammoType;
    protected float reloadingTime;

    public string Name { get { return name; } }
    public Damage Damage { get { return damage; } }
    public AmmoType AmmoType { get { return ammoType; } }
    public float ReloadingTime { get { return reloadingTime; } }

    public abstract void PrimaryAttack();
}
