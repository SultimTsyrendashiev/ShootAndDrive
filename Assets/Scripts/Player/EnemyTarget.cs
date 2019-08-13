using UnityEngine;

namespace SD.PlayerLogic
{
    /// <summary>
    /// Attach this script to show enemies their targer
    /// </summary>
    class EnemyTarget : MonoBehaviour, IEnemyTarget
    {
        public Transform Target => transform;

        void Start()
        {
            // register this target in game controller
            GameController.Instance.AddEnemyTarget(this);
        }
    }
}
