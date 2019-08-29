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
    delegate void ScoreSet(GameScore score, int prevBestScore);
    delegate void PlayerDeath(GameScore score);
    delegate void PlayerBalanceChange(int oldBalance, int newBalance);
    delegate void PlayerStateChange(PlayerState state);
    delegate void PlayerSpawn(Player player);
    delegate void PlayerKills(Transform enemyTransform);

    // game
    delegate void GameControllerCreate(GameController controller);
    delegate void InventoryCreate(IInventory inventory);

    // vehicles
    delegate void CollideVehicle(IVehicle other, float damage);

    // enemies
    delegate void EnemyDeath(EnemyData data, Transform enemyPosition, GameObject initiator); // for player's score
    delegate void VehicleDestroyed(EnemyVehicleData data, Transform vehiclePosition, GameObject initiator);   // for player's score

    delegate void PassengerDeath(EnemyData data, Transform enemyPosition, GameObject initiator);     // from passengers of vehicles

    // shop
    delegate void BuyWeapon(WeaponIndex index, int price);
    delegate void BuyAmmo(AmmunitionType index, int price);
}
