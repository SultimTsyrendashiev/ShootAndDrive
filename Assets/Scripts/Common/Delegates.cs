using SD.Enemies;
using SD.PlayerLogic;
using UnityEngine;

namespace SD
{
    delegate void Void();
    delegate void FloatChange(float f);

    // player
    delegate bool RegenerateHealth();
    delegate void ScoreChange(GameScore score);
    delegate void PlayerDeath(GameScore score);
    delegate void PlayerBalanceChange(int oldBalance, int newBalance);
    delegate void PlayerStateChange(PlayerState state);
    delegate void PlayerSpawn(Player player);

    // game
    delegate void GameControllerCreate(GameController controller);
    delegate void InventoryCreate(IInventory inventory);

    // vehicles
    delegate void CollideVehicle(IVehicle other, float damage);

    // enemies
    delegate void EnemyDied(EnemyData data, GameObject initiator);                 // for player's score
    delegate void VehicleDestroyed(EnemyVehicleData data, GameObject initiator);   // for player's score

    delegate void VehicleDeath();                                       // from damage receiver
    delegate void PassengerDeath(EnemyData data, GameObject initiator);     // from passengers of vehicles

    // delegate void PassengerStateChacnge(PassengerState passengerState); // for animations, sounds etc

    // weapons
    delegate void WeaponShootFinish(WeaponIndex finishedWeapon);    // to weapons controller
    delegate void WeaponBreak(WeaponIndex brokenWeapon);    // to weapons controller
    delegate void WeaponAmmoChange(int currentAmount);      // for UI (ammo amount)
    delegate void WeaponSwitch(WeaponIndex switchTo);

    // spawners
    //delegate void SpawnerRegister(ISpawner s);
}
