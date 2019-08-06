using UnityEngine;

namespace SD.PlayerLogic
{
    class EnemyTarget : MonoBehaviour, IEnemyTarget
    {
        public Transform Target => transform;

        void Awake()
        {
            // register this target in game controller
            FindObjectOfType<GameController>().AddEnemyTarget(this);
        }
    }
}
