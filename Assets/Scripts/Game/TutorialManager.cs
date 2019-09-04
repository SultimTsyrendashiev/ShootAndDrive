using System;
using UnityEngine;
using SD.PlayerLogic;

namespace SD.Game
{
    class TutorialManager : MonoBehaviour
    {
        public static event Action OnTutorialStart;

        bool isStarted;

        public void StartTutorial(Player player, Action onTutorialEnd)
        {
            if (isStarted)
            {
                return;
            }

            isStarted = true;
            Debug.Log("Tutorial invoked", this);

            player.Vehicle.Accelerate();

            onTutorialEnd?.Invoke();
            isStarted = false;
        }
    }
}
