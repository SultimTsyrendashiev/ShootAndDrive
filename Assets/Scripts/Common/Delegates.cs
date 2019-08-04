using SD.Enemies;
using SD.PlayerLogic;
using SD.Weapons;

namespace SD
{
    delegate void Void();
    delegate void FloatChange(float f);

    // player
    delegate bool RegenerateHealth();
    delegate void ScoreChange(GameScore score);
    delegate void PlayerDeath(GameScore score);
    delegate void PlayerStateChange(PlayerState state);
    delegate void PlayerSpawn(Player player);

    // vehicles
    delegate void CollideVehicle(IVehicle other, float damage);

    // enemies
    delegate void EnemyDied(EnemyData data);                // for player's score
    delegate void VehicleDeath();                           // from damage receiver
    delegate void VehicleDestroyed(EnemyVehicleData data);  // for player's score
    delegate void PassengerDied(EnemyData data);            // from passengers of vehicles
    // delegate void PassengerStateChacnge(PassengerState passengerState); // for animations, sounds etc

    // weapons
    delegate void WeaponBreak(WeaponIndex brokenWeapon);    // to weapons controller
    delegate void WeaponAmmoChange(int currentAmount);      // for UI (ammo amount)
    delegate void WeaponSwitch(WeaponIndex switchTo);

    // spawners
    //delegate void SpawnerRegister(ISpawner s);
}
