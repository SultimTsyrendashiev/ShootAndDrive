namespace SD.Enemies
{
    enum EnemyVehicleState
    {
        Nothing,    // not in a scene
        Active,     // people in a vehicle are ready to attack
        NotActive,  // f.e. giving up, but not dead
        Dead,       // everybody in a vehicle are dead
        DeadDriver, // driver of the vehicle is dead, 
                        // but there are still alive passengers
        Destroyed   // vehicle is destroyed
    }
}
