using System;
using UnityEngine;
using SD.PlayerLogic;

namespace SD.Game
{
    class TutorialManager : MonoBehaviour
    {
        public static event Action OnTutorialStart;

        public void StartTutorial(Player player, Action onTutorialEnd)
        {
            Debug.Log("Tutorial invoked", this);

            player.Vehicle.Accelerate();

            onTutorialEnd?.Invoke();
        }
    }
}
